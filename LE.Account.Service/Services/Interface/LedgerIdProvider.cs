using LE.Account.Common.Enums;

namespace LE.Account.Service.Services.Interface
{
    public interface LedgerIdProvider
    {
        long getLedgerIdOfLedger(LedgerSetup l);
    }
}
