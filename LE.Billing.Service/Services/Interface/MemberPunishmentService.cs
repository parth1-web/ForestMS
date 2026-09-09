using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Services.Interface
{
    public interface MemberPunishmentService
    {
        MemberPunishment Insert(MemberPunishmentDto memPunishmentDto);
        void Update(MemberPunishmentDto memPunishmentDto);
        void Delete(long memPunishment_id);
    }
}
