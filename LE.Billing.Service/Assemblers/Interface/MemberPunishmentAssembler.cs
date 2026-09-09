using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;

namespace LE.Billing.Service.Assemblers.Interface
{
    public interface MemberPunishmentAssembler
    {
        void copy(MemberPunishment memberPunishment, MemberPunishmentDto memberPunishmentDto);
    }
}
