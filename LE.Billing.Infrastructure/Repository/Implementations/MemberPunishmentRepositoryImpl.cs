using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class MemberPunishmentRepositoryImpl : BaseRepositoryImpl<MemberPunishment>, MemberPunishmentRepository
    {
        private readonly AppDbContext _appDbContext;

        public MemberPunishmentRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}
