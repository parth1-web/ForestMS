using Dapper;
using LE.Account.Infrastructure.Reports.Dto;
using LE.Common.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Account.Infrastructure.Reports.QueryService
{
    public class AccountingBaseQueryService : IAccountingBaseQueryService
    {
        private readonly IConnectionProvider _connectionProvider;

        public AccountingBaseQueryService(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<List<AccountingReportQueryValueDto>> GetBaseAccountReportAsync(DateTime fromDate,
            DateTime toDate)
        {
            await using var conn = _connectionProvider.GetDbConnection();
            return (await conn.QueryAsync<AccountingReportQueryValueDto>(BaseAccountingReportQuery, new
            {
                fromDate = fromDate.Date,
                toDate = toDate.Date
            })).ToList();
        }


        public async Task<List<AccountingReportQueryValueDto>> GetCashBookReportAsync(DateTime fromDate, DateTime toDate, long? groupId)
        {
            await using var conn = _connectionProvider.GetDbConnection();
            return (await conn.QueryAsync<AccountingReportQueryValueDto>(DayCashBookReportQuery, new
            {
                fromDate = fromDate.Date,
                toDate = toDate.Date,
                groupId
            })).ToList();
        }

        #region Queries

        private const string DayCashBookReportQuery = @"
-- declare @fromDate date = '2010-01-01';
-- declare @toDate  date = '2025-12-31';
-- declare @groupId int = 19;
 with cte as (
    select 
        t.ledger_id, 
        coalesce(sum(t.dr_amount), 0) as dr_amount, 
        coalesce(sum(t.cr_amount), 0) as cr_amount
    from transaction_detail t
    where t.transaction_date::date >= @fromDate::date
      and t.transaction_date::date <= @toDate::date
      and t.transaction_id in (
          select t2.transaction_id
          from transaction_detail t2
          where @groupId is null
             or t2.ledger_id in (
                 select x.""LedgerId""
                 from coa_structure x
                 where x.""GroupId"" = @groupId
             )
      )
    group by t.ledger_id
)

select 
    coa.""ParentName"",
    coa.""ParentId"",
    coa.""GroupId"",
    coa.""GroupName"",
    coa.""GroupCode"",
    coa.""LedgerId"",
    coa.""LedgerName"",
    coa.""LedgerCode"",
    coalesce(
        case when coa.""ParentId"" in (1, 4) 
             then cte.dr_amount - cte.cr_amount 
             else cte.cr_amount - cte.dr_amount 
        end,
        0
    ) as ""Balance"",
    coalesce(cte.dr_amount, 0) as ""DrAmount"",
    coalesce(cte.cr_amount, 0) as ""CrAmount""

from coa_structure coa
inner join cte on cte.ledger_id = coa.""LedgerId""
order by coa.""ParentId"" asc;
";

        private const string BaseAccountingReportQuery = @"
-- declare @fromDate date = '2022-01-01';
-- declare @toDate  date = '2023-12-31'
-- Assume @fromDate and @toDate are passed as parameters from C# code.

with 
fiscal_year as (
    select fn_get_fiscal_year_id(@toDate) as fiscalyearid
),
cte as (
    select * from coa_structure
),
transactions as (
    select t.ledger_id,
           coalesce(sum(t.dr_amount), 0) as dr_amount,
           coalesce(sum(t.cr_amount), 0) as cr_amount
    from transaction_detail t
    where t.transaction_date >= @fromDate
      and t.transaction_date <= @toDate
    group by t.ledger_id
),
previous_transactions as (
    select t.ledger_id,
           coalesce(sum(t.dr_amount), 0) as dr_amount,
           coalesce(sum(t.cr_amount), 0) as cr_amount
    from transaction_detail t, fiscal_year fy
    where t.fiscal_year_id = fy.fiscalyearid - 1
    group by t.ledger_id
)

select 
    cte.""ParentName"" as ""ParentName"",
    cte.""ParentId"" as ""ParentId"",
    cte.""GroupId"" as ""GroupId"",
    cte.""GroupName"" as ""GroupName"",
    cte.""GroupCode"" as ""GroupCode"",
    cte.""LedgerId"" as ""LedgerId"",
    cte.""LedgerName"" as ""LedgerName"",
    cte.""LedgerCode"" as ""LedgerCode"",
    cte.""LedgerType"" as ""LedgerType"",
    coalesce(
        case when cte.""ParentId"" in (1, 4) then t.dr_amount - t.cr_amount
             else t.cr_amount - t.dr_amount end,
        0
    ) as ""Balance"",
    coalesce(t.dr_amount, 0) as ""DrAmount"",
    coalesce(t.cr_amount, 0) as ""CrAmount"",
    coalesce(pt.dr_amount, 0) as ""PreviousDrAmount"",
    coalesce(pt.cr_amount, 0) as ""PreviousCrAmount"",
    coalesce(
        case when cte.""ParentId"" in (1, 4) then pt.dr_amount - pt.cr_amount
             else pt.cr_amount - pt.dr_amount end,
        0
    ) as ""PreviousBalance""
from cte
left join transactions t on cte.""LedgerId"" = t.ledger_id
left join previous_transactions pt on cte.""LedgerId"" = pt.ledger_id
order by cte.""ParentId"" asc;

";

        #endregion
    }
}