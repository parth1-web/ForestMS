using LE.Common.Enums;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;
using System.Linq;

namespace LE.Context.Repository.Implementations
{
    public class AuthenticationRepositoryImpl : BaseRepositoryImpl<Authentication>, AuthenticationRepository
    {
        private readonly AppDbContext _AppDbContext;

        public AuthenticationRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        public Authentication getByType(long type_id, UserType type)
        {
            return _AppDbContext.authentications.Where(a => a.type == type && a.type_id == type_id).SingleOrDefault();
        }

        public Authentication getByUsername(string username)
        {
            return _AppDbContext.authentications.Where(a => a.username == username).SingleOrDefault();
        }
    }
}
