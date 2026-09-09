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
    }
}
