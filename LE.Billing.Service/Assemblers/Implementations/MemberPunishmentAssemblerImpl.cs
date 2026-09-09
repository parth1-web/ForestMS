using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Service.Assemblers.Interface;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class MemberPunishmentAssemblerImpl : MemberPunishmentAssembler
    {
        public void copy(MemberPunishment memPunishment, MemberPunishmentDto memPunishmentDto)
        {
            memPunishment.MemberPunishmentId = memPunishmentDto.MemberPunishmentId;
            memPunishment.MembershipId = memPunishmentDto.MembershipId;
            memPunishment.IllegalActivity = memPunishmentDto.IllegalActivity;
            memPunishment.IssueDate = memPunishmentDto.IssueDate;
            memPunishment.NepIssueDate = memPunishmentDto.NepIssueDate;
            memPunishment.PunishmentValidity = memPunishmentDto.PunishmentValidity;
            memPunishment.NepPunishmentValidity = memPunishmentDto.NepPunishmentValidity;
            memPunishment.IsActive = memPunishmentDto.IsActive;
            memPunishment.Remarks = memPunishmentDto.Remarks;
            memPunishment.CreatedDate = memPunishmentDto.CreatedDate;
            memPunishment.CreatedBy = memPunishmentDto.CreatedBy;
            memPunishment.IsCancelledRemarks = memPunishmentDto.IsCancelledRemarks;
            memPunishment.IsCancelled = memPunishmentDto.IsCancelled;
            //memPunishment.MemberId = memPunishmentDto.MemberId;
        }
    }
}
