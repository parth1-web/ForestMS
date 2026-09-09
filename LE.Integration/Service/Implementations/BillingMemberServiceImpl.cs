using LE.Account.Common.Enums;
using LE.Account.Entities;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Assemblers.Implementations;
using LE.Account.Service.Assemblers.Interface;
using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;
using System;
using System.Transactions;

namespace LE.Integration.Service.Implementations
{
    public class BillingMemberServiceImpl
    {
        private readonly MemberRepository _memberRepo;
        private readonly MemberAssembler _memberAssembler;

        private readonly LedgerRepository _ledgerRepo;
        private readonly LedgerAssembler _ledgerMaker;
        private TransactionDtoAssembler _transactionDtoMaker;
        private TransactionService _transactionService;

        private readonly LedgerGroupIdProviderService _ledgerGroupIdProviderService;

        public BillingMemberServiceImpl(MemberRepository memberRepo, MemberAssembler memberAssembler, LedgerGroupIdProviderService ledgerGroupIdProviderService, LedgerRepository ledgerRepo, LedgerAssembler ledgerMaker, TransactionDtoAssembler transactionDtoMaker, TransactionService transactionService)
        {
            _memberRepo = memberRepo;
            _memberAssembler = memberAssembler;
            _ledgerGroupIdProviderService = ledgerGroupIdProviderService;
            _ledgerRepo = ledgerRepo;
            _ledgerMaker = ledgerMaker;
            _transactionService = transactionService;
            _transactionDtoMaker = transactionDtoMaker;
        }

        public void disable(long member_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var member = _memberRepo.getById(member_id);

                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member with id {member_id} doesnot exist.");
                    }

                    member.Disable();
                    _memberRepo.update(member);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void enable(long member_id)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var member = _memberRepo.getById(member_id);

                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member with id {member_id} doesnot exist.");
                    }

                    member.Enable();
                    _memberRepo.update(member);

                    tx.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Member insert(MemberDto member_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var member = new Member();

                    _memberAssembler.copy(member, member_dto);

                    _memberRepo.insert(member);

                    var ledgerDto = new LedgerDto();
                    ledgerDto.name = member_dto.FullName;
                    ledgerDto.ledger_group_id = _ledgerGroupIdProviderService.getDebtorsGroupId();
                    ledgerDto.user_id = member_dto.CreatedBy;
                    ledgerDto.balance_type = OpeningBalanceType.debit;
                    var ledger = saveLedgerAndGetLedgerEntity(ledgerDto);
                    member_dto.MemberId = member.MemberId;

                    update(member_dto);
                    tx.Complete();
                    return member;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private Ledger saveLedgerAndGetLedgerEntity(LedgerDto ledgerDto)
        {
            var ledgerWithSameName = _ledgerRepo.getByName(ledgerDto.name);
            bool isNameAllowed = ledgerWithSameName == null;
            if (!isNameAllowed)
                throw new DuplicateItemException($"Ledger {ledgerDto.name} already exist.");

            Ledger _ledger = new Ledger();
            var ledgerList = _ledgerRepo.getLedgersByLedgerGroup(ledgerDto.ledger_group_id);
            var newCode = string.Concat(ledgerDto.ledger_group_id, ".1");

            if (ledgerList.Count != 0)
            {
                var lastLedgerGroupIdonParticularType = ledgerList[ledgerList.Count - 1].code;
                string[] parts = lastLedgerGroupIdonParticularType.Split('.');
                var front = int.Parse(parts[0]);
                int last = int.Parse(parts[1]) + 1;
                newCode = string.Concat(front, ".", last);
            }
            ledgerDto.code = newCode;
            _ledgerMaker.copy(_ledger, ledgerDto);
            _ledgerRepo.insert(_ledger);
            ledgerDto.ledger_id = _ledger.ledger_id;
            TransactionDto transactionDto = _transactionDtoMaker.createTransactionDtoFrom(ledgerDto);
            _transactionService.addTransaction(transactionDto);
            return _ledger;
        }

        public void update(MemberDto member_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    var member = _memberRepo.getById(member_dto.MemberId);

                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member with id {member_dto.MemberId} doesnot exist.");
                    }
                    member_dto.CreatedBy = member.CreatedBy;
                    _memberAssembler.copy(member, member_dto);

                    _memberRepo.update(member);

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
