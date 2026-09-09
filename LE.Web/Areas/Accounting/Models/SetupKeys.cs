using LE.Account.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LE.Web.Areas.Accounting.Models
{
    public class SetupKeys
    {
        public static List<string> getKeys()
        {
            return new List<string>()
            {
                getCashLedgerKey,
                getBankLedgerKey,
                getTaxLedgerKey,
                getBallaballiSalesLedgerKey,
                getLakadiSalesLedgerKey,
                getFirewoodSalesLedgerKey,
                getFurnitureSalesLedgerKey,
                getChiranSalesLedgerKey,
                getDebtorsGroupKey,
                getCreditorsGroupKey
            };
        }

        public static List<string> getFiscalKeys()
        {
            return new List<string>()
            {
                getFiscalYearMonthKey,
                getFiscalYearDayKey,
            };
        }
        public static string getPurchaseLedgerKey { get; } = LedgerSetup.purchase.ToString();
        public static string getSalesLedgerKey { get; } = LedgerSetup.sales.ToString();
        public static string getSalesReturnKey { get; } = LedgerSetup.sales_return.ToString();
        public static string getDiscountReceivedLedgerKey { get; } = LedgerSetup.discount_received.ToString();
        public static string getDiscountAllowedLedgerKey { get; } = LedgerSetup.discount_allowed.ToString();
        [Display(Name = "Cash")]
        public static string getCashLedgerKey { get; } = LedgerSetup.cash.ToString();
        [Display(Name = "Bank")]
        public static string getBankLedgerKey { get; } = LedgerSetup.bank.ToString();
        [Display(Name = "Fiscal Year Month")]
        public static string getFiscalYearMonthKey { get; } = AccountSetting.fiscal_year_month.ToString();
        [Display(Name = "Fiscal Year Day")]
        public static string getFiscalYearDayKey { get; } = AccountSetting.fiscal_year_day.ToString();
        [Display(Name = "Tax")]
        public static string getTaxLedgerKey { get; } = LedgerSetup.Tax.ToString();
        public static string getKhabaKhuttiSalesLedgerKey { get; } = LedgerSetup.khabakhutti_sales.ToString();
        [Display(Name = "Firewood Sales")]
        public static string getFirewoodSalesLedgerKey { get; } = LedgerSetup.firewood_sales.ToString();
        [Display(Name = "Lakadi Sales")]
        public static string getLakadiSalesLedgerKey { get; } = LedgerSetup.lakadi_sales.ToString();
        [Display(Name = "Ballaballi Sales")]
        public static string getBallaballiSalesLedgerKey { get; } = LedgerSetup.ballaballi_sales.ToString();
        [Display(Name = "Furniture Sales")]
        public static string getFurnitureSalesLedgerKey { get; } = LedgerSetup.furniture_sales.ToString();
        public static string getChiranSalesLedgerKey { get; } = LedgerSetup.chiran_sales.ToString();
        public static string getCreditorsGroupKey { get; } = LedgerSetup.Creditors_Group.ToString();
        public static string getDebtorsGroupKey { get; } = LedgerSetup.Debtors_Group.ToString();
    }
}
