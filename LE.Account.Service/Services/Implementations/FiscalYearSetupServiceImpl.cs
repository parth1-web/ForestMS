using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace LE.Account.Service.Services.Implementations
{
    public class FiscalYearSetupServiceImpl : FiscalYearSetupService
    {
        private readonly AccountSettingsRepository _accountSettingRepo;

        public FiscalYearSetupServiceImpl(AccountSettingsRepository accountSettingRepo)
        {
            _accountSettingRepo = accountSettingRepo;
        }

        public void saveOrUpdate(List<AccountSettings> keyValue)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var kvp in keyValue)
                    {
                        saveOrUpdate(kvp.key, kvp.value);
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void saveOrUpdate(string key, long value)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var accountSetup = _accountSettingRepo.getByKey(key);

                    if (accountSetup == null)
                    {
                        save(key, value);
                    }
                    else
                    {
                        update(accountSetup, value);
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void update(AccountSettings accountSetup, long value)
        {
            accountSetup.value = value;
            _accountSettingRepo.update(accountSetup);
        }

        private void save(string key, long value)
        {
            AccountSettings accountSetup = new AccountSettings();
            accountSetup.key = key;
            accountSetup.value = value;
            _accountSettingRepo.insert(accountSetup);
        }
    }
}
