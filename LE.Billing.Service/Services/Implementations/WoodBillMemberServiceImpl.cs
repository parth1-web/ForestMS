using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

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
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var wood_bill_member_dto in wood_bill_member_dtos)
                    {
                        var woodBillMember = new WoodBillMembers();
                        _woodBillMemberAssembler.copy(woodBillMember, wood_bill_member_dto);
                        _woodBillMemberRepo.insert(woodBillMember);
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
