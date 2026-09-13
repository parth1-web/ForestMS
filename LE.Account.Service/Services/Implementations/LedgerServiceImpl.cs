using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Account.Service.Services.Implementations
{
    public class LedgerServiceImpl : LedgerService
    {
        private readonly LedgerRepository _ledgerRepo;
        private readonly LedgerAssembler _ledgerMaker;
        private TransactionDtoAssembler _transactionDtoMaker;
        private TransactionService _transactionService;
        private readonly AccountSettingsRepository _accountSettingRepo;

        public LedgerServiceImpl(LedgerRepository ledgerRepo, LedgerAssembler ledgerMaker, TransactionDtoAssembler transactionDtoMaker, TransactionService transactionService, AccountSettingsRepository accountSettingsRepository)
        {
            _ledgerRepo = ledgerRepo;
            _ledgerMaker = ledgerMaker;
            _transactionDtoMaker = transactionDtoMaker;
            _transactionService = transactionService;
            _accountSettingRepo = accountSettingsRepository;
        }

        public void delete(long ledger_id)
        {
            var ledger = _ledgerRepo.getById(ledger_id);
            if (ledger == null)
                throw new ItemNotFoundException($"The ledger with id {ledger_id} doesnot exist.");

            if (ledger.hasTransactions())
                throw new ItemUsedException("Specified Ledger has already performed transactions.You cannot delete at this moment.");

            _ledgerRepo.delete(ledger);
        }

        public void update(LedgerDto ledgerDto)
        {
            // P1 fix: real EF transaction (the ambient scope was a no-op for EF Core).
            using (var tx = _ledgerRepo.beginTransaction())
            {
                var ledger = _ledgerRepo.getById(ledgerDto.ledger_id);
                if (ledger == null)
                    throw new ItemNotFoundException($"Ledger with id {ledgerDto.ledger_id} doesnot exist.");

                var ledgerWithSamename = _ledgerRepo.getByName(ledgerDto.name);
                bool isNameAllowed = (ledgerWithSamename == null || ledgerWithSamename.ledger_id == ledgerDto.ledger_id);

                if (!isNameAllowed)
                    throw new DuplicateItemException($"Ledger with name {ledgerDto.name} already exists.");

                var ledgerWithSameCode = _ledgerRepo.getQueryable().Where(a => a.code == ledgerDto.code).SingleOrDefault();
                bool isCodeAllowed = (ledgerWithSameCode == null || ledgerWithSameCode.ledger_id == ledgerDto.ledger_id);

                if (!isCodeAllowed)
                {
                    throw new ItemUsedException("The provided code is already in use. Try another one.");
                }

                if (!ledger.hasTransactions())
                {
                    if (ledgerDto.opening_balance > 0)
                    {
                        var dateFiscalYearService = FiscalYearFactory.getFiscalYearService();
                        ledgerDto.ledger_id = ledger.ledger_id;
                        TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(ledgerDto);
                        transactionDto.transaction_date = getFiscalYearFirstDate();
                        transactionDto.voucher_type = VoucherType.Journal;
                        _transactionService.addTransaction(transactionDto);
                    }
                }
                _ledgerMaker.copy(ledger, ledgerDto);
                _ledgerRepo.update(ledger);
                tx.Commit();
            }
        }

        public Ledger save(LedgerDto ledgerDto)
        {
            // P1 fix: real EF transaction (the ambient scope was a no-op for EF Core).
            using (var tx = _ledgerRepo.beginTransaction())
            {
                var ledgerWithSameName = _ledgerRepo.getByName(ledgerDto.name);
                bool isNameAllowed = ledgerWithSameName == null;
                //if (!isNameAllowed)
                //    throw new DuplicateItemException($"Ledger {ledgerDto.name} already exist.");

                Ledger _ledger = new Ledger();

                if (ledgerDto.code == null || ledgerDto.code == "")
                {
                    var ledgerList = _ledgerRepo.getLedgersByLedgerGroup(ledgerDto.ledger_group_id);
                    var newCode = string.Concat(ledgerDto.ledger_group_id, ".1");

                    if (ledgerList.Count != 0)
                    {
                        var lastLedgerGroupIdonParticularType = ledgerList[ledgerList.Count - 1].code;
                        string[] parts = lastLedgerGroupIdonParticularType.Split('.');
                        var front = int.Parse(parts[0]);
                        int last = int.Parse(parts[1]) + 1;
                        newCode = string.Concat(front, ".", last);
                    }
                    ledgerDto.code = newCode;
                }
                else
                {
                    var dublicateCode = _ledgerRepo.getQueryable().Where(a => a.code == ledgerDto.code).ToList();
                    if (dublicateCode.Count > 0)
                    {
                        throw new ItemUsedException("The provided code is already in use. Try another one.");
                    }
                }

                _ledgerMaker.copy(_ledger, ledgerDto);
                _ledgerRepo.insert(_ledger);
                if (ledgerDto.opening_balance > 0)
                {
                    ledgerDto.ledger_id = _ledger.ledger_id;
                    TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(ledgerDto);
                    transactionDto.transaction_date = getFiscalYearFirstDate();
                    transactionDto.voucher_type = VoucherType.Journal;
                    _transactionService.addTransaction(transactionDto);
                }

                tx.Commit();
                return _ledger;
            }
        }

        private DateTime getFiscalYearFirstDate()
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            var currentNpDate = dateConverterService.ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date).getFormattedDate().ToString();

            int month = Convert.ToInt32(currentNpDate.Split("-")[0]);

            var fiscalYearMonth = _accountSettingRepo.getByKey(AccountSetting.fiscal_year_month.ToString());
            var fiscalYearDay = _accountSettingRepo.getByKey(AccountSetting.fiscal_year_day.ToString());

            if (fiscalYearMonth == null)
            {
                throw new ItemNotFoundException("Fiscal Year Month is not set up.Please setup and try again.");
            }

            if (fiscalYearDay == null)
            {
                throw new ItemNotFoundException("Fiscal Year Day is not set up.Please setup and try again.");
            }

            var fiscalYearMonthValue = fiscalYearMonth.value;
            var fiscalYearDayValue = fiscalYearDay.value;

            var fiscalYear = Convert.ToInt32(dateConverterService.ToBS(DateTime.UtcNow).getFormattedDate().ToString().Split("-")[2]);

            if (month < 4)
            {
                fiscalYear = fiscalYear - 1;
            }

            DateTime fiscalYearStartDate = dateConverterService.ToAD(fiscalYear + "-" + fiscalYearMonthValue + "-" + fiscalYearDayValue).getFormattedDate().AddSeconds(1);
            return fiscalYearStartDate;
        }
    }

}
