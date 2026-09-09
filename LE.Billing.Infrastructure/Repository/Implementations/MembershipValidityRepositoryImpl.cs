using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class MembershipValidityRepositoryImpl : BaseRepositoryImpl<MembershipValidity>, MembershipValidityRepository
    {
        private readonly AppDbContext _appDbContext;

        public MembershipValidityRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
