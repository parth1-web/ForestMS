using LE.Common.Enums;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System.Collections.Generic;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class UserRoleRepositoryImpl : BaseRepositoryImpl<UserRole>, UserRoleRepository
    {
        private readonly AppDbContext _AppDbContext;

        public UserRoleRepositoryImpl(AppDbContext AppDbContext):base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }
        public List<UserRole> getByRoleId(long role_id)
        {
            return _AppDbContext.user_roles.Where(a => a.role_id == role_id).ToList();
        }

        public List<UserRole> getByTypeId(UserType type, long type_id)
        {
            return _AppDbContext.user_roles.Where(a => a.type == type && a.type_id==type_id).ToList();
        }
    }
}
