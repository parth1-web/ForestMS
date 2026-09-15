using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class MemberServiceImpl : MemberService
    {
        private MemberRepository _memberRepo;
        private MemberAssembler _memberAssembler;
        private LedgerGroupIdProviderService _ledgerGroupIdProviderService;

        public MemberServiceImpl(MemberRepository memberRepo,
            MemberAssembler memberAssembler, 
            LedgerGroupIdProviderService ledgerGroupIdProviderService
            )
        {
            _memberRepo = memberRepo;
            _memberAssembler = memberAssembler;
            _ledgerGroupIdProviderService = ledgerGroupIdProviderService;
        }

        public Member Insert(MemberDto member_dto)
        {
            try
            {
                using (var tx = _memberRepo.beginTransaction())
                {
                    Member member = new Member();
                    _memberAssembler.copy(member, member_dto);
                    _memberRepo.insert(member);
                    _memberRepo.saveChanges();
                    tx.Commit();
                    return member;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(MemberDto member_dto)
        {
            try
            {
                using (var tx = _memberRepo.beginTransaction())
                {
                    var member = _memberRepo.getById(member_dto.MemberId);
                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member not found");
                    }
                    member_dto.CreatedBy = member.CreatedBy;
                    _memberAssembler.copy(member, member_dto);
                    _memberRepo.update(member);
                    _memberRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Disable(long member_id)
        {
            try
            {
                using (var tx = _memberRepo.beginTransaction())
                {
                    var member = _memberRepo.getById(member_id);
                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member not found.");
                    }
                    member.Disable();
                    _memberRepo.update(member);
                    _memberRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Enable(long member_id)
        {
            try
            {
                using (var tx = _memberRepo.beginTransaction())
                {
                    var member = _memberRepo.getById(member_id);
                    if (member == null)
                    {
                        throw new ItemNotFoundException($"Member not found");
                    }
                    member.Enable();
                    _memberRepo.update(member);
                    _memberRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		public void Delete(long member_id)
		{
			try
			{
				using (var tx = _memberRepo.beginTransaction())
				{
					var member = _memberRepo.getById(member_id);
					if (member == null)
					{
						throw new ItemNotFoundException($"Member not found");
					}
					member.Delete();
					_memberRepo.update(member);
					_memberRepo.saveChanges();
					tx.Commit();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

       
    }
}
