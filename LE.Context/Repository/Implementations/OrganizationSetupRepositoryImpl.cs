using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.OrganizationSetup;
using LE.Service.Repository.Interface;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class OrganizationSetupRepositoryImpl : BaseRepositoryImpl<OrganizationSetup>, OrganizationSetupRepository
    {
        private readonly AppDbContext _appDbContext;

        public OrganizationSetupRepositoryImpl(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public OrganizationSetup getByKey(string key)
        {
            return _appDbContext.organization_setup.Where(a => a.key == key).SingleOrDefault();
        }
    }
}
