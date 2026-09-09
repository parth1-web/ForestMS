using DateConverter.Core.Service_Factory;
using LE.Account.Infrastructure.Reports.Reporter.ValueObjects;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Accounting.ViewModels
{
    public class StatementOfLedgerIndexViewModel : GenericIndexViewModel
    {
        [Display(Name = "From Date")]
        public string start_date { get; set; }

        [Display(Name = "To Date")]
        public string end_date { get; set; }

        [Display(Name = "Date")]
        public string date { get; set; } = DateConverterFactory.getDateConverterService().ToBS(DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone().Date, DateFormats.yMd).getFormattedDate();
        public long ledger_id { get; set; }
        public string name { get; set; }
        public const string ACCOUNT_TYPE_CASH = "CASH_ACCOUNT";
        public const string ACCOUNT_TYPE_BANK = "BANK_ACCOUNT";
        public string ledger_type { get; set; } = "CASH_ACCOUNT";
        public decimal old_balance { get; set; }
        public List<TransactionDetailModel> transactionDetail { get; set; }
        public List<TransactionDetailViewModel> ExpensesData { get; set; }
        public List<TransactionDetailViewModel> IncomeData { get; set; }
        public List<TransactionDetailViewModel> BankExpensesData { get; set; }
        public List<TransactionDetailViewModel> BankIncomeData { get; set; }
        public List<AccountingReportVo> Report = new List<AccountingReportVo>();
    }
}
