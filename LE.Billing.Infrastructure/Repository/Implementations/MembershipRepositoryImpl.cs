using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class MembershipRepositoryImpl : BaseRepositoryImpl<Membership>, MembershipRepository
    {
        private readonly AppDbContext _appDbContext;

        public MembershipRepositoryImpl (AppDbContext appDbContext) : base (appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Membership GetByCode(string code)
        {
            return _appDbContext.membership.Where(x=>x.MembershipCode.Contains(code) && !x.IsCancelled).SingleOrDefault();
        }
    }
}
