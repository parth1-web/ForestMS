using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class ModuleRepositoryImpl : BaseRepositoryImpl<Module>, ModuleRepository
    {
        private readonly AppDbContext _appDbContext;

        public ModuleRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Module getByName(string module_name)
        {
            return _appDbContext.modules.Where(a => a.module_name.ToUpper() == module_name.ToUpper()).SingleOrDefault();
        }

        public Module getByCode(string module_code)
        {
            // P1/S3: permission checks resolve modules by module_code (the seed data
            // reuses area-like codes, e.g. "billing"), which is unique per module.
            return _appDbContext.modules.Where(a => a.module_code.ToUpper() == module_code.ToUpper()).SingleOrDefault();
        }
    }
}
