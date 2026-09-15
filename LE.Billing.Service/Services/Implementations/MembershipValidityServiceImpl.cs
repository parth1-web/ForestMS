using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class MembershipValidityServiceImpl : MembershipValidityService
    {
        private MembershipValidityRepository _memPeriodRepo;
        private MembershipValidityAssembler _memPeriodAssembler;

        public MembershipValidityServiceImpl(MembershipValidityRepository memPeriodRepo, MembershipValidityAssembler memPeriodAssembler)
        {
            _memPeriodRepo = memPeriodRepo;
            _memPeriodAssembler = memPeriodAssembler;
        }

        public MembershipValidity Insert(MembershipValidityDto dto)
        {
            try
            {
                using (var tx = _memPeriodRepo.beginTransaction())
                {
                    MembershipValidity msp = new MembershipValidity();
                    _memPeriodAssembler.copy(msp, dto);
                    _memPeriodRepo.insert(msp);
                    _memPeriodRepo.saveChanges();
                    tx.Commit();
                    return msp;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(MembershipValidityDto dto)
        {
            try
            {
                using (var tx = _memPeriodRepo.beginTransaction())
                {
                    var msp = _memPeriodRepo.getById(dto.MembershipValidityId);

                    if (msp == null)
                    {
                        throw new ItemNotFoundException($"Membership validity not found.");
                    }
                    dto.CreatedBy = msp.CreatedBy;
                    dto.CreatedDate = msp.CreatedDate;
                    dto.IssueDate = msp.IssueDate;
                    dto.IsCurrent = msp.IsCurrent;
					_memPeriodAssembler.copy(msp, dto);
					_memPeriodRepo.update(msp);
                    _memPeriodRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Expire(long membership_id)
        {
            try
            {
                using (var tx = _memPeriodRepo.beginTransaction())
                {
                    var msp = _memPeriodRepo.getById(membership_id);
                    if (msp == null)
                    {
                        throw new ItemNotFoundException($"Membership not found.");
                    }
                    msp.Expire();
                    _memPeriodRepo.update(msp);
                    _memPeriodRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Unexpire(long membership_id)
        {
            try
            {
                using (var tx = _memPeriodRepo.beginTransaction())
                {
                    var msp = _memPeriodRepo.getById(membership_id);
                    if (msp == null)
                    {
                        throw new ItemNotFoundException($"Membership not found.");
                    }
                    msp.Unexpire();
                    _memPeriodRepo.update(msp);
                    _memPeriodRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Cancel(long membership_id)
        {
            try
            {
                using (var tx = _memPeriodRepo.beginTransaction())
                {
                    var msp = _memPeriodRepo.getById(membership_id);
                    if (msp == null)
                    {
                        throw new ItemNotFoundException($"Membership not found.");
                    }
                    msp.IsCancelled = true;
                    _memPeriodRepo.update(msp);
                    _memPeriodRepo.saveChanges();
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
