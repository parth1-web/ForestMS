using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class MemberRepositoryImpl : BaseRepositoryImpl<Member>, MemberRepository
    {
        private readonly AppDbContext _appDbContext;

        public MemberRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Member getByName(string member_name)
        {
            return _appDbContext.members.Where(a => a.FullName == member_name).SingleOrDefault();
        }
    }
}
