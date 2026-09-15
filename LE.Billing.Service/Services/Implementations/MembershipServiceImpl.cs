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
    public class MembershipServiceImpl : MembershipService
    {
        private readonly MembershipRepository _membershipRepo;
        private readonly MembershipAssembler _membershipAssembler;
        private readonly LedgerService _ledgerService;

        public MembershipServiceImpl(MembershipRepository membershipRepo, MembershipAssembler membershipAssembler, LedgerService ledgerService)
        {
            _membershipRepo = membershipRepo;
            _membershipAssembler = membershipAssembler;
            _ledgerService = ledgerService;
        }

        public Membership Insert(MembershipDto membershipDto)
        {
            try
            {
                using (var tx = _membershipRepo.beginTransaction())
                {
                    Membership membership = new Membership();
                    _membershipAssembler.copy(membership, membershipDto);
                    _membershipRepo.insert(membership);
                    _membershipRepo.saveChanges();
                    tx.Commit();
                    return membership;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(MembershipDto membershipDto)
        {
            try
            {
                using (var tx = _membershipRepo.beginTransaction())
                {
                    var membership = _membershipRepo.getById(membershipDto.MembershipId);
                    if (membership == null)
                    {
                        throw new ItemNotFoundException("Membership not found");
                    }
                    _membershipAssembler.copy(membership, membershipDto);
                    _membershipRepo.update(membership);
                    _membershipRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CancelMembership(long membershipId)
        {
            try
            {
                using (var tx = _membershipRepo.beginTransaction())
                {
                    var membership = _membershipRepo.getById(membershipId);
                    if (membership == null)
                    {
                        throw new ItemNotFoundException("Membership not found");
                    }

                    membership.IsCancelled = true;
                    _membershipRepo.update(membership);
                    _membershipRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Enable(long membershipId)
        {
            try
            {
                using (var tx = _membershipRepo.beginTransaction())
                {
                    var membership = _membershipRepo.getById(membershipId);
                    if (membership == null)
                    {
                        throw new ItemNotFoundException("Membership not found.");
                    }

                    membership.IsActive = true;
                    _membershipRepo.update(membership);
                    _membershipRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		public void Disable(long membershipId)
		{
			try
			{
				using (var tx = _membershipRepo.beginTransaction())
				{
					var membership = _membershipRepo.getById(membershipId);
					if (membership == null)
					{
						throw new ItemNotFoundException("Membership not found.");
					}

					membership.IsActive = false;
					_membershipRepo.update(membership);
					_membershipRepo.saveChanges();
					tx.Commit();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void Delete(long membershipId)
		{
			try
			{
				using (var tx = _membershipRepo.beginTransaction())
				{
					var membership = _membershipRepo.getById(membershipId);
					if (membership == null)
					{
						throw new ItemNotFoundException("Membership not found.");
					}

					membership.IsCancelled = true;
					membership.CancelledDate = DateTime.Now;
					_membershipRepo.update(membership);
					_membershipRepo.saveChanges();
					tx.Commit();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
