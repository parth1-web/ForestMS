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

namespace LE.Billing.Service.Services.Implementations
{
    public class FurnitureSalesServiceImpl : FurnitureSalesService
    {
        private readonly FurnitureSalesRepository _furnitureSalesRepo;
        private readonly DayCloseRepository _dayCloseRepository;
        private readonly FurnitureSalesAssembler _furnitureSalesAssembler;
        private readonly LedgerIdProvider _ledgerIdProvider;
        private readonly FurnitureSalesDetailService _furnitureSalesDetailService;
        private readonly TransactionService _transactionService;
        private readonly StockMovementAssembler _stockMovementAssembler;
        private readonly StockMovementRepository _stockMovementRepo;
        private readonly Movement_ItemAvailabilityAdapter _movement_ItemAvailabilityAdapter;

        public FurnitureSalesServiceImpl(FurnitureSalesRepository furnitureSalesRepo, DayCloseRepository dayCloseRepository, FurnitureSalesAssembler furnitureSalesAssembler, LedgerIdProvider ledgerIdProvider, FurnitureSalesDetailService furnitureSalesDetailService, TransactionService transactionService, StockMovementAssembler stockMovementAssembler, StockMovementRepository stockMovementRepo, Movement_ItemAvailabilityAdapter movement_ItemAvailabilityAdapter)
        {
            _furnitureSalesRepo = furnitureSalesRepo;
            _dayCloseRepository = dayCloseRepository;
            _furnitureSalesAssembler = furnitureSalesAssembler;
            _ledgerIdProvider = ledgerIdProvider;
            _furnitureSalesDetailService = furnitureSalesDetailService;
            _transactionService = transactionService;
            _stockMovementAssembler = stockMovementAssembler;
            _stockMovementRepo = stockMovementRepo;
            _movement_ItemAvailabilityAdapter = movement_ItemAvailabilityAdapter;
        }

        public long makeSales(FurnitureSalesDto sales_dto)
        {
            // P1 fix: ambient TransactionScope was a no-op for EF Core; bill + details +
            // ledger entry now run in one real database transaction.
            using (var tx = _furnitureSalesRepo.beginTransaction())
            {
                checkIfDayClosedorNot(sales_dto);

                var sales = new FurnitureSales();
                _furnitureSalesAssembler.copy(sales, sales_dto);
                _furnitureSalesRepo.insert(sales);

                sales_dto.furniture_sales_details.ForEach(a => a.furniture_sales_id = sales.furniture_sales_id);
                _furnitureSalesDetailService.save(sales_dto.furniture_sales_details);

                sales_dto.furniture_sales_id = sales.furniture_sales_id;
                makeAccountSalesTransaction(sales_dto);

                //recordStockMovement(sales_dto, sales.furniture_sales_id);
                _furnitureSalesRepo.saveChanges();
                tx.Commit();
                return sales.furniture_sales_id;
            }
        }
        public void cancel(long furniture_sales_id, long user_id)
        {
            using (var tx = _furnitureSalesRepo.beginTransaction())
            {
                var furnitureSales = _furnitureSalesRepo.getById(furniture_sales_id);
                validateFurnitureSalesData(furnitureSales);
                updateFurnitureSales(furnitureSales, user_id);
                makeAccountSalesCancelTransaction(furnitureSales);
                //recordReverseStockMovement(furnitureSales);
                _furnitureSalesRepo.saveChanges();
                tx.Commit();
            }
        }
        private void makeAccountSalesTransaction(FurnitureSalesDto sales_dto)
        {
            long salesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.furniture_sales);
            if (salesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Furniture Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            createAccountSalesTransaction(sales_dto, salesLedgerId, cashLedgerId);
        }
        private void createAccountSalesTransaction(FurnitureSalesDto sales_dto, long salesLedgerId, long cashLedgerId)
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
                transactionDto.remarks = "फर्निचर बिक्रीबाट प्राप्त नगद";
                transactionDto.voucher_no = sales_dto.furniture_sales_id;
                transactionDto.voucher_type = VoucherType.FurnitureSales;
                _transactionService.addTransaction(transactionDto);
            }
        }
        private void checkIfDayClosedorNot(FurnitureSalesDto sales_dto)
        {
            var IsClosed = _dayCloseRepository.getByDate(sales_dto.sales_date.Date);
            if (IsClosed != null)
            {
                throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
            }
        }
        private void recordReverseStockMovement(FurnitureSales sales)
        {
            StockMovementDto stockMovementDto = new StockMovementDto();
            stockMovementDto.movement_type = StockMovementType.delete;
            stockMovementDto.movement_type_id = sales.furniture_sales_id;
            var salesDetailsForStockSales = sales.furniture_sales_detail;
            foreach (var stockSales in salesDetailsForStockSales)
            {
                stockMovementDto.addStockMovementDetail(new StockMovementDetailDto()
                {
                    stock_item_id = stockSales.furniture_id,
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
        //private void recordStockMovement(FurnitureSalesDto sales_dto, long furniture_sales_id)
        //{
        //    StockMovementDto stockMovementDto = new StockMovementDto();
        //    stockMovementDto.movement_type = StockMovementType.sales;
        //    stockMovementDto.movement_type_id = furniture_sales_id;
        //    var salesDetailsForStockSales = sales_dto.furniture_sales_details.ToList();
        //    foreach (var stockSales in salesDetailsForStockSales)
        //    {
        //        stockMovementDto.addStockMovementDetail(new StockMovementDetailDto()
        //        {
        //            stock_item_id = stockSales.furniture_id,
        //            qty = stockSales.quantity,
        //            operation = MovementOperation.decrease
        //        });
        //    }
        //    if (stockMovementDto.isMovementValid())
        //    {
        //        var stockMovement = new StockMovement();
        //        _stockMovementAssembler.copy(stockMovement, stockMovementDto);
        //        _stockMovementRepo.insert(stockMovement);
        //        updateItemAvailability(stockMovementDto);
        //    }
        //}
        private void updateItemAvailability(StockMovementDto stock_movement_dto)
        {
            _movement_ItemAvailabilityAdapter.updateItemAvailability(stock_movement_dto);
        }
        private void makeAccountSalesCancelTransaction(FurnitureSales furnitureSales)
        {
            long salesLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.furniture_sales);
            if (salesLedgerId <= 0)
            {
                throw new ItemNotFoundException("Furniture Sales Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            long cashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (cashLedgerId <= 0)
            {
                throw new ItemNotFoundException("Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }
            createAccountSalesCancelTransaction(furnitureSales, salesLedgerId, cashLedgerId);
        }
        private void createAccountSalesCancelTransaction(FurnitureSales furnitureSales, long salesLedgerId, long cashLedgerId)
        {
            var transactionDto = new TransactionDto();

            if (furnitureSales.total_amount > 0)
            {
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = furnitureSales.total_amount,
                    ledger_id = salesLedgerId,
                });

                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = furnitureSales.total_amount,
                    ledger_id = cashLedgerId
                });
                transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                transactionDto.remarks = "फर्निचर बिक्री रद्द";
                transactionDto.voucher_type = VoucherType.FurnitureSales;
                transactionDto.voucher_no = furnitureSales.furniture_sales_id;
                _transactionService.addTransaction(transactionDto);

            }
        }
        private void updateFurnitureSales(FurnitureSales furnitureSales, long user_id)
        {
            furnitureSales.cancel();
            furnitureSales.cancelled_by = user_id;
            furnitureSales.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
            _furnitureSalesRepo.update(furnitureSales);
        }
        private void validateFurnitureSalesData(FurnitureSales furnitureSales)
        {
            if (furnitureSales == null)
            {
                throw new ItemNotFoundException("Furniture Sales Detail not found");
            }
            if (furnitureSales.is_cancelled)
            {
                throw new ItemUsedException("This bill is already cancelled.");
            }
        }
    }
}
