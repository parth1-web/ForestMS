using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Entities.User;
using LE.Service.Repository.Interface;

namespace LE.Context.Repository.Implementations
{
    public class LoginSessionRepositoryImpl : BaseRepositoryImpl<LoginSession>, LoginSessionRepository
    {
        private readonly AppDbContext _AppDbContext;

        public LoginSessionRepositoryImpl(AppDbContext AppDbContext) : base(AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }
    }
}
