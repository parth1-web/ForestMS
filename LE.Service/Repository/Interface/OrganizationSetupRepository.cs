using LE.Entities.OrganizationSetup;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface OrganizationSetupRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(OrganizationSetup orgSetup);
        void update(OrganizationSetup orgSetup);
        void delete(OrganizationSetup orgSetup);
        List<OrganizationSetup> getAll();
        OrganizationSetup getById(long org_setup_id);
        OrganizationSetup getByKey(string key);
        IQueryable<OrganizationSetup> getQueryable();
    }
}
