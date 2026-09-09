using LE.Entities.OrganizationSetup;
using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface OrganizationSetupRepository
    {
        void insert(OrganizationSetup orgSetup);
        void update(OrganizationSetup orgSetup);
        void delete(OrganizationSetup orgSetup);
        List<OrganizationSetup> getAll();
        OrganizationSetup getById(long org_setup_id);
        OrganizationSetup getByKey(string key);
        IQueryable<OrganizationSetup> getQueryable();
    }
}
