using System.Collections.Generic;

namespace LE.Context.Repository.Implementations
{
    using LE.Common.Repository.Implementations;
    using LE.Context.Data;
    using LE.Service.Repository.Interface;
    using System.Linq;
    using userEntity = LE.Entities.User.User;

    public class UserRepositoryImpl : BaseRepositoryImpl<userEntity>, UserRepository
    {
        private readonly AppDbContext _AppDbContext;
        public UserRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        public List<userEntity> getByName(string user_name)
        {
            return _AppDbContext.users.Where(a => a.full_name == user_name).ToList();
        }
    }
}
