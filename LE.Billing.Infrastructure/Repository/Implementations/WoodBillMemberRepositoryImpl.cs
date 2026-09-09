using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class WoodBillMemberRepositoryImpl : BaseRepositoryImpl<WoodBillMembers>, WoodBillMemberRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodBillMemberRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
