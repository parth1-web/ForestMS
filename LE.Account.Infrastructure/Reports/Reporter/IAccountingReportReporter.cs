using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;

namespace LE.Account.Infrastructure.Reports.Reporter
{
    public interface IAccountingReportReporter
    {
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<AccountingReportVo>> GetBalanceSheet(DateTime fromDate, DateTime toDate);

        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<AccountingReportVo>> GetProfitAndLoss(DateTime fromDate, DateTime toDate);

        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<AccountingReportVo>> GetTrialBalance(DateTime fromDate, DateTime toDate);

        Task<List<AccountingReportVo>> GetTradingReport(DateTime fromDate, DateTime toDate);

        Task<List<AccountingReportVo>> GetDayVoucher(DateTime fromDate, DateTime toDate);

        Task<List<AccountingReportVo>> GetCashBook(DateTime fromDate, DateTime toDate, long? ledgerId);
    }
}