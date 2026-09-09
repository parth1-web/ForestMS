using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Account.Service.Services.Implementations
{
    public class ReportServiceImpl : ReportService
    {
        private readonly LedgerRepository ledgerRepository;
        private LedgerGroupRepository ledgerGroupRepo;
        private TransactionDetailRepository transactionDetailRepo;
        private TransactionRepository _transactionRepo;

        public ReportServiceImpl(TransactionRepository transactionRepo, LedgerRepository _ledgerRepository, LedgerGroupRepository _ledgerGroupRepo, TransactionDetailRepository _transactionDetailRepo)
        {
            ledgerRepository = _ledgerRepository;
            ledgerGroupRepo = _ledgerGroupRepo;
            transactionDetailRepo = _transactionDetailRepo;
            _transactionRepo = transactionRepo;
        }

        private List<Ledger> getAllLedgersBelongingToLedgerGroup(List<LedgerGroup> ledger_groups)
        {
            List<Ledger> led = new List<Ledger>();
            foreach (var ledgersGroup in ledger_groups)
            {
                foreach (var ledger in ledgersGroup.ledgers)
                {
                    led.Add(ledger);
                }
            }
            return led;
        }

        private List<Ledger> getAllLedgersBelongingToAssetsGroup()
        {
            var assetLedger = ledgerGroupRepo.getLedgerGroupByAccountType(LedgerGroupType.asset);
            return getAllLedgersBelongingToLedgerGroup(assetLedger);
        }

        private List<Ledger> getAllLedgerBelongingToLiabilitiesGroup()
        {
            var liaLedger = ledgerGroupRepo.getLedgerGroupByAccountType(LedgerGroupType.liability);
            return getAllLedgersBelongingToLedgerGroup(liaLedger);
        }

        private List<Ledger> getAllLedgerBelongingToIncomeGroup()
        {
            var incomeLedger = ledgerGroupRepo.getLedgerGroupByAccountType(LedgerGroupType.income);
            return getAllLedgersBelongingToLedgerGroup(incomeLedger);

        }

        private List<Ledger> getAllLedgerBelongingToExpensesGroup()
        {
            var expLedger = ledgerGroupRepo.getLedgerGroupByAccountType(LedgerGroupType.expenses);
            return getAllLedgersBelongingToLedgerGroup(expLedger);
        }


        public TrialBalanceDto getTrialBalanceDto(DateTime start_date, DateTime end_date)
        {
            TrialBalanceDto trialBalanceDto = new TrialBalanceDto();
            List<LedgerAmountDto> liabilyLedgerAmountDto = getLiabilyLedgerAmountDtosof(start_date, end_date);
            List<LedgerAmountDto> assetsLedgerAmountDto = getAssetsLedgerAmountDtosof(start_date, end_date);
            List<LedgerAmountDto> incomeLedgerAmountDto = getIncomeLedgerAmountDtosof(start_date, end_date);
            List<LedgerAmountDto> expensesLedgerAmountDto = getExpensesLedgerAmountDtosof(start_date, end_date);
            trialBalanceDto.addAssetsLedgerBalanceDto(assetsLedgerAmountDto);
            trialBalanceDto.addLiabilityLedgerBalanceDto(liabilyLedgerAmountDto);
            trialBalanceDto.addIncomeLedgerBalanceDto(incomeLedgerAmountDto);
            trialBalanceDto.addExpensesLedgerBalanceDto(expensesLedgerAmountDto);

            // decimal netAmountLastYear = getNetAmountOf(start_date);

            // trialBalanceDto.opening_balance = netAmountLastYear;

            return trialBalanceDto;
        }

        public ProfitAndLossDto getProfitAndLossDto(DateTime start_date, DateTime end_date)
        {
            ProfitAndLossDto profitLossDto = new ProfitAndLossDto();
            List<LedgerAmountDto> incomeLedgerAmountDto = getIncomeLedgerAmountDtosof(start_date, end_date);
            correctIncomeLedgerDtoBalances(incomeLedgerAmountDto);
            List<LedgerAmountDto> expensesLedgerAmountDto = getExpensesLedgerAmountDtosof(start_date, end_date);
            List<LedgerAmountDto> liabilitesLedgerDto = getLiabilyLedgerAmountDtosof(start_date, end_date);


            profitLossDto.addIncomeLedgerAmountDto(incomeLedgerAmountDto);
            profitLossDto.addExpensesLedgerAmountDto(expensesLedgerAmountDto);
            profitLossDto.addLiabilitiesLedgerAmountDto(liabilitesLedgerDto);


            decimal netAmountLastYear = getNetAmountOf(start_date);

            profitLossDto.opening_balance = netAmountLastYear;
            return profitLossDto;
        }


        public BalanceSheetDto getBalanceSheetDto(DateTime start_date, DateTime end_date)
        {
            BalanceSheetDto balanceSheetDto = new BalanceSheetDto();
            List<LedgerAmountDto> assetLedgerDto = getAssetsLedgerAmountDtosof(start_date, end_date);
            List<LedgerAmountDto> liabilitesLedgerDto = getLiabilyLedgerAmountDtosof(start_date, end_date);

            correctLiabilityLedgerDtoBalance(liabilitesLedgerDto);
            decimal netAmountLastyear = getNetAmountOf(start_date);
            decimal profitOrLossAmount = getProfitAndLossDto(start_date, end_date).opening_balance;

            balanceSheetDto.profit_loss_amount = profitOrLossAmount;
            balanceSheetDto.closing_balance = netAmountLastyear;
            balanceSheetDto.addAssetsLedgerAmountDto(assetLedgerDto);
            balanceSheetDto.addLiabilitiesLedgerAmountDto(liabilitesLedgerDto);

            return balanceSheetDto;

        }

        private void correctLiabilityLedgerDtoBalance(List<LedgerAmountDto> liabilitesLedgerDto)
        {
            liabilitesLedgerDto.Select(c => { c.amount = c.amount * -1; return c; }).ToList();
        }

        private decimal getNetAmountOf(DateTime start_date)
        {
            List<Ledger> assetLedgers = getAllLedgersBelongingToAssetsGroup();
            List<Ledger> liabilityLedgers = getAllLedgerBelongingToLiabilitiesGroup();

            decimal assetBalance = 0;
            decimal liabilityBalance = 0;
            foreach (var assetLedger in assetLedgers)
            {
                assetBalance += getLedgerBalanceAmountTillDate(assetLedger.ledger_id, start_date);
            }

            foreach (var liabilityLedger in liabilityLedgers)
            {
                liabilityBalance += getLedgerBalanceAmountTillDate(liabilityLedger.ledger_id, start_date);
            }

            decimal netAmount = assetBalance - liabilityBalance;
            return netAmount;
        }

        private List<LedgerAmountDto> getExpensesLedgerAmountDtosof(DateTime start_date, DateTime end_date)
        {
            List<LedgerAmountDto> dtos = new List<LedgerAmountDto>();
            List<Ledger> expensesLedger = getAllLedgerBelongingToExpensesGroup();
            AddDtoInDtosWithAmountBetweenTwoDates(start_date, end_date, dtos, expensesLedger);
            return dtos;
        }

        private List<LedgerAmountDto> getIncomeLedgerAmountDtosof(DateTime start_date, DateTime end_date)
        {
            List<LedgerAmountDto> dtos = new List<LedgerAmountDto>();
            List<Ledger> incomeLedger = getAllLedgerBelongingToIncomeGroup();
            AddDtoInDtosWithAmountBetweenTwoDates(start_date, end_date, dtos, incomeLedger);
            return dtos;
        }

        private List<LedgerAmountDto> getAssetsLedgerAmountDtosof(DateTime start_date, DateTime end_date)
        {
            List<LedgerAmountDto> dtos = new List<LedgerAmountDto>();
            List<Ledger> assetsLedger = getAllLedgersBelongingToAssetsGroup();
            AddDtoInDtosWithAmountBetweenTwoDates(start_date, end_date, dtos, assetsLedger);
            return dtos;
        }

        private List<LedgerAmountDto> getLiabilyLedgerAmountDtosof(DateTime start_date, DateTime end_date)
        {
            List<LedgerAmountDto> dtos = new List<LedgerAmountDto>();
            List<Ledger> liabilityLedger = getAllLedgerBelongingToLiabilitiesGroup();
            AddDtoInDtosWithAmountBetweenTwoDates(start_date, end_date, dtos, liabilityLedger);
            return dtos;
        }


        private void AddDtoInDtosWithAmountBetweenTwoDates(DateTime start_date, DateTime end_date, List<LedgerAmountDto> dtos, List<Ledger> ledgers)
        {
            foreach (var ledger in ledgers)
            {
                LedgerAmountDto dto = new LedgerAmountDto();
                dto.ledger = ledger;
                decimal latestBalanceAmt = getLedgerBalanceAmountBetweenDates(ledger.ledger_id, start_date, end_date);
                decimal startDateBalance = getLedgerBalanceAmountTillDate(ledger.ledger_id, start_date);
                dto.amount = latestBalanceAmt;
                dtos.Add(dto);
            }
        }

        private void addDtoInDtosWithAmountCalculated(DateTime end_date, List<LedgerAmountDto> dtos, List<Ledger> ledgers)
        {
            foreach (var ledger in ledgers)
            {
                LedgerAmountDto dto = new LedgerAmountDto();
                dto.ledger = ledger;
                dto.amount = getLedgerBalanceAmountTillDate(ledger.ledger_id, end_date);
                dtos.Add(dto);
            }
        }
        private decimal getLedgerBalanceAmountBetweenDates(long ledger_id, DateTime start_date, DateTime end_date)
        {
            decimal balance = transactionDetailRepo.getLedgerBalanceAmountBetweenDates(ledger_id, start_date, end_date);
            return balance;
        }

        private decimal getLedgerBalanceAmountTillDate(long ledger_id, DateTime end_date)
        {
            decimal balance = transactionDetailRepo.getOldBalance(ledger_id, end_date);
            return balance;
        }

        public void correctIncomeLedgerDtoBalances(List<LedgerAmountDto> incomeLedgerBalanceDtos)
        {
            incomeLedgerBalanceDtos.Select(c => { c.amount = c.amount * -1; return c; }).ToList();
        }

        public List<Transaction> getTransactionWithin(DateTime start_date, DateTime end_date)
        {
            var transactions = _transactionRepo.getQueryable().Where(a => a.transaction_date.Date >= start_date && a.transaction_date.Date <= end_date).ToList();
            return transactions;
        }

        public List<TransactionDetail> getTransactionDetailsWithinForLedger(DateTime start_date, DateTime end_date, long ledger_id)
        {
            var transactionDetails = transactionDetailRepo.getQueryable().Where(a => a.transaction_date.Date >= start_date.Date && a.transaction_date.Date <= end_date.Date && a.ledger_id == ledger_id).OrderBy(a => a.transaction_date).ToList();
            return transactionDetails;
        }

        public List<TransactionDetail> getTransactionDetailsWithLedgerAndTransactionId(DateTime start_date, DateTime end_date, long ledger_id)
        {
            List<TransactionDetail> dto = new List<TransactionDetail>();

            var transactionDetails = transactionDetailRepo.getQueryable().Where(a => a.transaction_date.Date >= start_date.Date && a.transaction_date.Date <= end_date.Date && a.ledger_id == ledger_id).OrderBy(a => a.transaction_date).ToList();
            foreach (var item in transactionDetails)
            {
                var transDetails = transactionDetailRepo.getQueryable().Where(a => a.transaction_id == item.transaction_id && a.ledger_id != ledger_id).ToList();
                dto.AddRange(transDetails);
            }
            return dto;
        }

        public List<TransactionDetail> getTransactionDetailsForLedgerWithDate(DateTime date, long ledger_id)
        {
            var transactionDetails = transactionDetailRepo.getQueryable().Where(a => a.transaction_date.Date == date && a.ledger_id == ledger_id).ToList();
            return transactionDetails;
        }

    }
}
