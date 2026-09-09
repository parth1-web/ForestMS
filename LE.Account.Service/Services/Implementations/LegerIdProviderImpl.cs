using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Account.Service.Services.Implementations
{
    public class LegerIdProviderImpl : LedgerIdProvider
    {
        private readonly LedgerSetupRepository _ledgerSetupRepo;
        public LegerIdProviderImpl(LedgerSetupRepository ledgerSetupRepo)
        {
            _ledgerSetupRepo = ledgerSetupRepo;
        }

        public long getLedgerIdOfLedger(LedgerSetup l)
        {
            try
            {
                var ledger = _ledgerSetupRepo.getByKey(l.ToString());
                if (ledger == null)
                {
                    throw new ItemNotFoundException("Ledgers are not set up. Please set up Ledgers in Ledger Setup.");
                }
                return Convert.ToInt32(ledger.value);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
