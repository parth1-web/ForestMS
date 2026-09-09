using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LE.Account.Infrastructure.Reports.Dto;
using LE.Account.Infrastructure.Reports.QueryService;
using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;

namespace LE.Account.Infrastructure.Reports.Reporter
{
    public class AccountingReportReporter : IAccountingReportReporter
    {
        private readonly IAccountingBaseQueryService _accountingBaseQueryService;

        public AccountingReportReporter(IAccountingBaseQueryService accountingBaseQueryService)
        {
            _accountingBaseQueryService = accountingBaseQueryService;
        }

        public async Task<List<AccountingReportVo>> GetBalanceSheet(DateTime fromDate, DateTime toDate)
        {
            var report =
                await _accountingBaseQueryService.GetBaseAccountReportAsync(fromDate: fromDate, toDate: toDate);
            var filtered = report.Where(x => x.ParentId == 1 || x.ParentId == 2).ToList();
            return DecorateAccountingReport(items: filtered);
        }

        public async Task<List<AccountingReportVo>> GetProfitAndLoss(DateTime fromDate, DateTime toDate)
        {
            var report =
                await _accountingBaseQueryService.GetBaseAccountReportAsync(fromDate: fromDate, toDate: toDate);
            var filtered = report.Where(x => x.ParentId == 3 || x.ParentId == 4)
                .Where(x => !string.Equals(x.LedgerType, "Trading", StringComparison.CurrentCultureIgnoreCase)).ToList();
            return DecorateAccountingReport(items: filtered);
        }

        public async Task<List<AccountingReportVo>> GetTrialBalance(DateTime fromDate, DateTime toDate)
        {
            var report =
                await _accountingBaseQueryService.GetBaseAccountReportAsync(fromDate: fromDate, toDate: toDate);
            return DecorateAccountingReport(items: report);
        }

        public async Task<List<AccountingReportVo>> GetTradingReport(DateTime fromDate, DateTime toDate)
        {
            var report =
                await _accountingBaseQueryService.GetBaseAccountReportAsync(fromDate: fromDate, toDate: toDate);
            var filtered = report.Where(x => x.ParentId == 3 || x.ParentId == 4)
                .Where(x => string.Equals(x.LedgerType, "Trading", StringComparison.CurrentCultureIgnoreCase)).ToList();
            return DecorateAccountingReport(items: filtered);
        }

        public async Task<List<AccountingReportVo>> GetDayVoucher(DateTime fromDate, DateTime toDate)
        {
            var report =
                await _accountingBaseQueryService.GetCashBookReportAsync(fromDate: fromDate, toDate: toDate,
                    groupId: null);
            return DecorateAccountingReport(report);
        }

        /// <summary>
        /// This is a special report for cash book which particularly shows cash relative balance
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<List<AccountingReportVo>> GetCashBook(DateTime fromDate, DateTime toDate, long? ledgerId)
        {
            var report =
                await _accountingBaseQueryService.GetCashBookReportAsync(fromDate: fromDate, toDate: toDate,
                    groupId: ledgerId);
            return DecorateAccountingReport(report);
        }


        private List<AccountingReportVo> DecorateAccountingReport(List<AccountingReportQueryValueDto> items)
        {
            var report = new List<AccountingReportVo>();

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                var parent = report.FirstOrDefault(x => x.ParentId == item.ParentId);
                if (parent == null)
                {
                    parent = new AccountingReportVo();
                    parent.Id = item.ParentId;
                    parent.ParentId = item.ParentId;
                    parent.Name = item.ParentName;
                    parent.Code = "";
                    report.Add(parent);
                }

                AddBalance(parent, item);

                var group = parent.Children.FirstOrDefault(x => x.Id == item.GroupId);
                if (group == null)
                {
                    group = new AccountingReportVo();
                    group.Id = item.GroupId;
                    group.ParentId = item.ParentId;
                    group.Name = item.GroupName;
                    group.Code = item.GroupCode;
                    parent.Children.Add(group);
                }

                AddBalance(group, item);
                var vm = new AccountingReportVo();
                vm.Id = item.LedgerId;
                vm.ParentId = item.GroupId;
                vm.Name = item.LedgerName;
                vm.Code = item.LedgerCode;
                vm.Dr = item.DrAmount;
                vm.Cr = item.CrAmount;
                vm.Balance = item.Balance;
                vm.PreviousBalance = item.PreviousBalance;
                vm.Total = item.Balance + item.PreviousBalance;
                group.Children.Add(vm);
            }

            void AddBalance(AccountingReportVo parent, AccountingReportQueryValueDto item)
            {
                parent.Dr += item.DrAmount;
                parent.Cr += item.CrAmount;
                parent.Balance += item.Balance;
                parent.PreviousBalance += item.PreviousBalance;
                parent.Total += item.Balance + item.PreviousBalance;
            }

            return report;
        }
    }
}