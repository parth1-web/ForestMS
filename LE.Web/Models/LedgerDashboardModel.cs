using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Web.Models
{
    public class LedgerDashboardModel
    {
        public List<CashLedgerDetails> cash_ledger_details { get; set; }
        public List<BankLedgerDetails> bank_ledger_details { get; set; }
        public List<CounterSalesData> counter_sales_data { get; set; }
    }

    public class CashLedgerDetails
    {
        public long ledger_id { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
    }
    public class BankLedgerDetails
    {
        public long ledger_id { get; set; }
        public string name { get; set; }
        public decimal amount { get; set; }
    }

    public class CounterSalesData
    {
        public string user { get; set; }
        public decimal amount { get; set; }
    }

}
