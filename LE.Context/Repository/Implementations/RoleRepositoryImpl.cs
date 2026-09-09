using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class RoleRepositoryImpl : BaseRepositoryImpl<Role>, RoleRepository
    {
        private readonly AppDbContext _AppDbContext;
        public RoleRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        public Role getByName(string role_name)
        {
            return _AppDbContext.roles.Where(a => a.name == role_name).SingleOrDefault();
        }
    }
}
