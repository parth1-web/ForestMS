using LE.Account.Entities;

namespace LE.Account.Infrastructure.Dto
{
    public class LedgerAmountDto
    {
        public Ledger ledger { get; set; }
        public decimal amount { get; set; }
    }
}
