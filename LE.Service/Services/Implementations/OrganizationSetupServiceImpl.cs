using LE.Entities.OrganizationSetup;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using System;
using System.Collections.Generic;

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
                using (var tx = _orgSetupRepo.beginTransaction())
                {
                    foreach (var kvp in keyValue)
                    {
                        saveOrUpdate(kvp.key, kvp.value);
                    }
                    _orgSetupRepo.saveChanges();
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
                using (var tx = _orgSetupRepo.beginTransaction())
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
                    _orgSetupRepo.saveChanges();
                    tx.Commit();
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
