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
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Service.Services.Implementations
{
    public class DayCloseServiceImpl : DayCloseService
    {
        private readonly CounterSalesRepository _counterSalesRepo;
        private readonly DayCloseRepository _dayCloseRepo;
        private readonly DayCloseAssembler _dayCloseAssembler;
        private readonly LedgerIdProvider _ledgerIdProvider;
        private readonly TransactionService _transactionService;
        private readonly ServiceRepository _serviceRepo;


        public DayCloseServiceImpl(DayCloseRepository dayCloseRepo, DayCloseAssembler dayCloseAssembler, LedgerIdProvider ledgerIdProvider, TransactionService transactionService, CounterSalesRepository counterSalesRepo, ServiceRepository serviceRepo)
        {
            _dayCloseRepo = dayCloseRepo;
            _dayCloseAssembler = dayCloseAssembler;
            _ledgerIdProvider = ledgerIdProvider;
            _transactionService = transactionService;
            _counterSalesRepo = counterSalesRepo;
            _serviceRepo = serviceRepo;
        }


        public void insert(DayCloseDto day_close_dto)
        {
            // P1 fix: ambient TransactionScope was a no-op for EF Core; the day-close row
            // and the counter-sales ledger posting now run in one real database transaction.
            using (var tx = _dayCloseRepo.beginTransaction())
            {
                var dayClose = _dayCloseRepo.getByDate(day_close_dto.eng_close_date.Date);

                if (dayClose != null)
                {
                    throw new DuplicateItemException($"Day is already Closed");
                }

                if (day_close_dto.eng_close_date.Date > DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date)
                {
                    throw new InvalidValueException("You are not allowed to close the future date.");
                }

                dayClose = new DayClose();
                _dayCloseAssembler.copy(dayClose, day_close_dto);
                _dayCloseRepo.insert(dayClose);

                //entry to account
                makeEntryToAccount(day_close_dto);
                _dayCloseRepo.saveChanges();
                tx.Commit();
            }
        }

        private void makeEntryToAccount(DayCloseDto day_close_dto)
        {
            var salesDetails = _counterSalesRepo.getSalesOnDate(day_close_dto.eng_close_date.Date);

            List<CounterSalesDetail> detail = new List<CounterSalesDetail>();

            foreach (var sales in salesDetails)
            {
                foreach (var salesDetail in sales.counter_sales_details)
                {
                    detail.Add(salesDetail);
                }
            }

            var grouped = detail.GroupBy(a => a.service_id).Select(g => new { id = g.Key, total = g.Sum(x => x.rate * x.qty), tax = g.Sum(x => x.tax_amount), ledger_id = g.Select(a => a.service.ledger_id).First() });

            long taxLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.Tax);
            if (taxLedgerId <= 0)
            {
                throw new ItemNotFoundException("Tax Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            long counterCashLedgerId = _ledgerIdProvider.getLedgerIdOfLedger(LedgerSetup.cash);
            if (counterCashLedgerId < 0)
            {
                throw new ItemNotFoundException("Counter Cash Ledger is not Defined. Please Define it in Ledger Setup and try again.");
            }

            decimal totalTaxAmount = 0;
            decimal totalAmount = 0;
            var transactionDto = new TransactionDto();

            foreach (var groupData in grouped)
            {
                totalAmount += groupData.total;
                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = groupData.total,
                    ledger_id = groupData.ledger_id,
                });

                if (groupData.tax > 0)
                {
                    totalTaxAmount += groupData.tax;
                }
            }
            if (totalTaxAmount > 0)
            {
                transactionDto.addCreditData(new LedgerTransactionDto()
                {
                    amount = totalTaxAmount,
                    ledger_id = taxLedgerId
                });
            }

            if (totalAmount > 0)
            {
                transactionDto.addDebitData(new LedgerTransactionDto()
                {
                    amount = totalAmount + totalTaxAmount,
                    ledger_id = counterCashLedgerId
                });
                transactionDto.remarks = "काउन्टर बिलि बाट प्राप्त नगद";
                transactionDto.transaction_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                _transactionService.addTransaction(transactionDto);
            }
        }

    }
}
