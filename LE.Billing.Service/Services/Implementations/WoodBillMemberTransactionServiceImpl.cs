using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Implementations
{
    public class WoodBillMemberTransactionServiceImpl:WoodBillMemberTransactionService
    {
        private readonly WoodBillMemberTransactionRepository _woodBillMemberTransactionRepo;
        private readonly WoodBillMemberTransactionAssembler _woodBillMemberTransactionAssembler;

        public WoodBillMemberTransactionServiceImpl(WoodBillMemberTransactionRepository woodBillMemberRepo, WoodBillMemberTransactionAssembler woodBillMemberAssembler)
        {
            _woodBillMemberTransactionAssembler = woodBillMemberAssembler;
            _woodBillMemberTransactionRepo = woodBillMemberRepo;
        }

        public void insert(List<WoodBillMemberTransactionDto> wood_bill_member_trans_dtos)
        {
            // Runs inside the caller's real EF transaction (see beginTransaction());
            // the previous ambient TransactionScope was a no-op for EF Core.
            foreach (var wood_bill_member_dto in wood_bill_member_trans_dtos)
            {
                var woodBillMember = new WoodBillMemberTransaction();
                _woodBillMemberTransactionAssembler.copy(woodBillMember, wood_bill_member_dto);
                _woodBillMemberTransactionRepo.insert(woodBillMember);
            }
        }
    }
}
