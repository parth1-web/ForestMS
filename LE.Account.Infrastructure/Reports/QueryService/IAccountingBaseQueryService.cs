using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LE.Account.Infrastructure.Reports.Dto;

namespace LE.Account.Infrastructure.Reports.QueryService
{
    public interface IAccountingBaseQueryService
    {
        Task<List<AccountingReportQueryValueDto>> GetBaseAccountReportAsync(DateTime fromDate, DateTime toDate);

        Task<List<AccountingReportQueryValueDto>> GetCashBookReportAsync(DateTime fromDate, DateTime toDate,
            long? groupId = null);
    }
}