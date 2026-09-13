using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Implementations
{
    public class WoodBillMemberServiceImpl:WoodBillMemberService
    {
        private readonly WoodBillMemberRepository _woodBillMemberRepo;
        private readonly WoodBillMembersAssembler _woodBillMemberAssembler;

        public WoodBillMemberServiceImpl(WoodBillMemberRepository woodBillMemberRepo, WoodBillMembersAssembler woodBillMemberAssembler)
        {
            _woodBillMemberAssembler = woodBillMemberAssembler;
            _woodBillMemberRepo = woodBillMemberRepo;
        }

        public void insert(List<WoodBillMemberDto> wood_bill_member_dtos)
        {
            // Runs inside the caller's real EF transaction (see beginTransaction());
            // the previous ambient TransactionScope was a no-op for EF Core.
            foreach (var wood_bill_member_dto in wood_bill_member_dtos)
            {
                var woodBillMember = new WoodBillMembers();
                _woodBillMemberAssembler.copy(woodBillMember, wood_bill_member_dto);
                _woodBillMemberRepo.insert(woodBillMember);
            }
        }
    }
}
