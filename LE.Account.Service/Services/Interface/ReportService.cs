using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using System;
using System.Collections.Generic;

namespace LE.Account.Service.Services.Interface
{
    public interface ReportService
    {
        void correctIncomeLedgerDtoBalances(List<LedgerAmountDto> incomeLedgerBalanceDtos);
        BalanceSheetDto getBalanceSheetDto(DateTime start_date, DateTime end_date);
        ProfitAndLossDto getProfitAndLossDto(DateTime start_date, DateTime end_date);
        TrialBalanceDto getTrialBalanceDto(DateTime start_date, DateTime end_date);
        List<Transaction> getTransactionWithin(DateTime start_date, DateTime end_date);
        List<TransactionDetail> getTransactionDetailsWithinForLedger(DateTime start_date, DateTime end_date, long ledger_id);
        List<TransactionDetail> getTransactionDetailsWithLedgerAndTransactionId(DateTime start_date, DateTime end_date, long ledger_id);
        List<TransactionDetail> getTransactionDetailsForLedgerWithDate(DateTime date, long ledger_id);
    }
}