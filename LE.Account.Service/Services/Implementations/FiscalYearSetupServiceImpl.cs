using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using System;
using System.Collections.Generic;

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
                using (var tx = _accountSettingRepo.beginTransaction())
                {
                    foreach (var kvp in keyValue)
                    {
                        saveOrUpdate(kvp.key, kvp.value);
                    }
                    _accountSettingRepo.saveChanges();
                    tx.Commit();
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
                using (var tx = _accountSettingRepo.beginTransaction())
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
                    _accountSettingRepo.saveChanges();
                    tx.Commit();
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
