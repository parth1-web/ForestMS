using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using System;
using System.Collections.Generic;

namespace LE.Account.Service.Services.Implementations
{
    public class LedgerSetupServiceImpl : LedgerSetupService
    {
        private readonly LedgerSetupRepository _ledgerSetupRepo;

        public LedgerSetupServiceImpl(LedgerSetupRepository ledgerSetupRepo)
        {
            _ledgerSetupRepo = ledgerSetupRepo;
        }

        public void saveOrUpdate(List<LedgerSetup> keyValue)
        {
            try
            {
                using (var tx = _ledgerSetupRepo.beginTransaction())
                {
                    foreach (var kvp in keyValue)
                    {
                        saveOrUpdate(kvp.key, kvp.value);
                    }
                    _ledgerSetupRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void saveOrUpdate(string key, string value)
        {
            try
            {
                using (var tx = _ledgerSetupRepo.beginTransaction())
                {
                    var ledgerSetup = _ledgerSetupRepo.getByKey(key);

                    if (ledgerSetup == null)
                    {
                        save(key, value);
                    }
                    else
                    {
                        update(ledgerSetup, value);
                    }
                    _ledgerSetupRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void update(LedgerSetup ledgerSetup, string value)
        {
            ledgerSetup.value = value;
            _ledgerSetupRepo.update(ledgerSetup);
        }

        private void save(string key, string value)
        {
            LedgerSetup ledgerSetup = new LedgerSetup();
            ledgerSetup.key = key;
            ledgerSetup.value = value;
            _ledgerSetupRepo.insert(ledgerSetup);
        }
    }
}
