using System;
using System.Collections.Generic;
using System.Text;

namespace LE.Account.Infrastructure.Dto
{
    public class LedgerTransactionDto
    {
        public long ledger_id { get; set; }
        public decimal amount { get; set; }
    }
}
