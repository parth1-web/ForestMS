using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using LE.Inventory.Common.Enums;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Adapter.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using System;
using System.Linq;
using System.Transactions;

namespace LE.Billing.Service.Services.Implementations
{
    public class FirewoodSalesServiceImpl : FirewoodSalesService
    {
        private readonly FirewoodSalesRepository _firewoodSalesRepo;
        private readonly DayCloseRepository _dayCloseRepository;
        private readonly FirewoodSalesAssembler _firewoodSalesAssembler;
        private readonly LedgerIdProvider _ledgerIdProvider;
        private readonly FirewoodSalesDetailService _firewoodSalesDetailService;
        private readonly TransactionService _transactionService;
        private readonly StockMovementAssembler _stockMovementAssembler;
        private readonly StockMovementRepository _stockMovementRepo;
        private readonly Movement_ItemAvailabilityAdapter _movement_ItemAvailabilityAdapter;


        public FirewoodSalesServiceImpl(FirewoodSalesRepository counterSalesRepo, FirewoodSalesAssembler counterSalesAssembler, FirewoodSalesDetailService counterSalesDetailService, LedgerIdProvider ledgerIdProvider, TransactionService transactionService, StockMovementAssembler stockMovementAssembler, StockMovementRepository stockMovementRepo, Movement_ItemAvailabilityAdapter movement_ItemAvailabilityAdapter, DayCloseRepository dayCloseRepository)
        {
            _firewoodSalesAssembler = counterSalesAssembler;
            _firewoodSalesRepo = counterSalesRepo;
            _ledgerIdProvider = ledgerIdProvider;
            _firewoodSalesDetailService = counterSalesDetailService;
            _transactionService = transactionService;
            _stockMovementAssembler = stockMovementAssembler;
            _stockMovementRepo = stockMovementRepo;
            _movement_ItemAvailabilityAdapter = movement_ItemAvailabilityAdapter;
            _dayCloseRepository = dayCloseRepository;
        }

        public long makeSales(FirewoodSalesDto sales_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    checkIfDayClosedorNot(sales_dto);

                    var sales = new FirewoodSales();
                    _firewoodSalesAssembler.copy(sales, sales_dto);
                    _firewoodSalesRepo.insert(sales);

                    sales_dto.firewood_sales_details.ForEach(a => a.firewood_sales_id = sales.firewood_sales_id);
                    _firewoodSalesDetailService.save(sales_dto.firewood_sales_details);

                    sales_dto.firewood_sales_id = sales.firewood_sales_id;
                    makeAccountSalesTransaction(sales_dto);

                    recordStockMovement(sales_dto, sales.firewood_sales_id);
                    tx.Complete();
                    return sales.firewood_sales_id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void cancel(long firewood_sales_id, long user_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var firewoodSales = _firewoodSalesRepo.getById(firewood_sales_id);
                    validateFirewoodSalesData(firewoodSales);
                    updateFirewoodSales(firewoodSales, user_id);
                    makeAccountSalesCancelTransaction(firewoodSales);
                    recordReverseStockMovement(firewoodSales);
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void makeAccountSalesTransaction(FirewoodSalesDto sales_dto)
        {
            long salesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.firewood_sales);
            if (salesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Firewood Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            createAccountSalesTransaction(sales_dto, salesLedgerId, cashLedgerId);
        }
        private void createAccountSalesTransaction(FirewoodSalesDto sales_dto, long salesLedgerId, long cashLedgerId)
        {
            var transactionDto = new TransactionDto();
            if (sales_dto.total_amount > 0)
            {
                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = sales_dto.total_amount,
                    ledger_id = salesLedgerId,
                });
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = sales_dto.total_amount,
                    ledger_id = cashLedgerId
                });
                transactionDto.transaction_date = sales_dto.sales_date;
                transactionDto.remarks = "दाउरा बिक्रीबाट प्राप्त नगद";
                transactionDto.voucher_no = sales_dto.firewood_sales_id;
                transactionDto.voucher_type = VoucherType.FirewoodSales;
                _transactionService.addTransaction(transactionDto);
            }
        }
        private void checkIfDayClosedorNot(FirewoodSalesDto sales_dto)
        {
            var IsClosed = _dayCloseRepository.getByDate(sales_dto.sales_date.Date);
            if (IsClosed != null)
            {
                throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
            }
        }
        private void recordReverseStockMovement(FirewoodSales sales)
        {
            StockMovementDto stockMovementDto = new StockMovementDto();
            stockMovementDto.movement_type = StockMovementType.delete;
            stockMovementDto.movement_type_id = sales.firewood_sales_id;
            var salesDetailsForStockSales = sales.firewood_sales_detail;
            foreach (var stockSales in salesDetailsForStockSales)
            {
                stockMovementDto.addStockMovementDetail(new StockMovementDetailDto()
                {
                    stock_item_id = stockSales.stock_item_id,
                    qty = stockSales.quantity,
                    operation = MovementOperation.increase
                });
            }
            if (stockMovementDto.isMovementValid())
            {
                var stockMovement = new StockMovement();
                _stockMovementAssembler.copy(stockMovement, stockMovementDto);
                _stockMovementRepo.insert(stockMovement);
                updateItemAvailability(stockMovementDto);
            }
        }
        private void recordStockMovement(FirewoodSalesDto sales_dto, long firewood_sales_id)
        {
            StockMovementDto stockMovementDto = new StockMovementDto();
            stockMovementDto.movement_type = StockMovementType.sales;
            stockMovementDto.movement_type_id = firewood_sales_id;
            var salesDetailsForStockSales = sales_dto.firewood_sales_details.ToList();
            foreach (var stockSales in salesDetailsForStockSales)
            {
                stockMovementDto.addStockMovementDetail(new StockMovementDetailDto()
                {
                    stock_item_id = stockSales.stock_item_id,
                    qty = stockSales.quantity,
                    operation = MovementOperation.decrease
                });
            }
            if (stockMovementDto.isMovementValid())
            {
                var stockMovement = new StockMovement();
                _stockMovementAssembler.copy(stockMovement, stockMovementDto);
                _stockMovementRepo.insert(stockMovement);
                updateItemAvailability(stockMovementDto);
            }
        }
        private void updateItemAvailability(StockMovementDto stock_movement_dto)
        {
            _movement_ItemAvailabilityAdapter.updateItemAvailability(stock_movement_dto);
        }
        private void makeAccountSalesCancelTransaction(FirewoodSales firewoodSales)
        {
            long salesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.firewood_sales);
            if (salesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Firewood Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            createAccountSalesCancelTransaction(firewoodSales, salesLedgerId, cashLedgerId);
        }
        private void createAccountSalesCancelTransaction(FirewoodSales firewoodSales, long salesLedgerId, long cashLedgerId)
        {
            var transactionDto = new TransactionDto();

            if (firewoodSales.total_amount > 0)
            {
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = firewoodSales.total_amount,
                    ledger_id = salesLedgerId,
                });

                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = firewoodSales.total_amount,
                    ledger_id = cashLedgerId
                });
                transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                transactionDto.remarks = "दाउरा बिक्री रद्द";
                transactionDto.voucher_type = VoucherType.FirewoodSales;
                transactionDto.voucher_no = firewoodSales.firewood_sales_id;
                _transactionService.addTransaction(transactionDto);

            }
        }
        private void updateFirewoodSales(FirewoodSales firewoodSales, long user_id)
        {
            firewoodSales.cancel();
            firewoodSales.cancelled_by = user_id;
            firewoodSales.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            _firewoodSalesRepo.update(firewoodSales);
        }
        private void validateFirewoodSalesData(FirewoodSales firewoodSales)
        {
            if (firewoodSales == null)
            {
                throw new ItemNotFoundException("Firewood Sales Detail not found");
            }
            if (firewoodSales.is_cancelled)
            {
                throw new ItemUsedException("This bill is already cancelled.");
            }
        }
    }
}
