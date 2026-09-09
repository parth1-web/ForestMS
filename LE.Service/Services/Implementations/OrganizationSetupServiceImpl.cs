using LE.Entities.OrganizationSetup;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace LE.Service.Services.Implementations
{
    public class OrganizationSetupServiceImpl:OrganizationSetupService
    {
        private readonly OrganizationSetupRepository _orgSetupRepo;

        public OrganizationSetupServiceImpl(OrganizationSetupRepository orgSetupRepo)
        {
            _orgSetupRepo = orgSetupRepo;
        }

        public void saveOrUpdate(List<OrganizationSetup> keyValue)
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

        public void saveOrUpdate(string key, string value)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var orgSetup = _orgSetupRepo.getByKey(key);

                    if (orgSetup == null)
                    {
                        save(key, value);
                    }
                    else
                    {
                        update(orgSetup, value);
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void update(OrganizationSetup orgSetup, string value)
        {
            orgSetup.value = value;
            _orgSetupRepo.update(orgSetup);
        }

        private void save(string key, string value)
        {
            OrganizationSetup orgSetup = new OrganizationSetup();
            orgSetup.key = key;
            orgSetup.value = value;
            _orgSetupRepo.insert(orgSetup);
        }
    }
}
