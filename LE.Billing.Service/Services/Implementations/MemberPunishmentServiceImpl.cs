using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;

namespace LE.Billing.Service.Services.Implementations
{
    public class MemberPunishmentServiceImpl : MemberPunishmentService
    {
        private MemberPunishmentRepository _memPunishmentRepo;
        private MemberPunishmentAssembler _memPunishmentAssembler;

        public MemberPunishmentServiceImpl(MemberPunishmentRepository memPunishmentRepo, MemberPunishmentAssembler memPunishmentAssembler)
        {
            _memPunishmentRepo = memPunishmentRepo;
            _memPunishmentAssembler = memPunishmentAssembler;
        }

        public MemberPunishment Insert(MemberPunishmentDto dto)
        {
            try
            {
                using (var tx = _memPunishmentRepo.beginTransaction())
                {
                    MemberPunishment mp = new MemberPunishment();
                    _memPunishmentAssembler.copy(mp, dto);
                    _memPunishmentRepo.insert(mp);
                    _memPunishmentRepo.saveChanges();
                    tx.Commit();
                    return mp;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(MemberPunishmentDto dto)
        {
            try
            {
                using (var tx = _memPunishmentRepo.beginTransaction())
                {
                    var mp = _memPunishmentRepo.getById(dto.MemberPunishmentId);
                    if (mp == null)
                    {
                        throw new ItemNotFoundException("Member punishment not found.");
                    }

                    dto.CreatedBy = mp.CreatedBy;
                    dto.CreatedDate = mp.CreatedDate;
                    _memPunishmentAssembler.copy(mp, dto);
                    _memPunishmentRepo.update(mp);
                    _memPunishmentRepo.saveChanges();
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(long memPunishment_id)
        {
            try
            {
                using (var tx = _memPunishmentRepo.beginTransaction())
                {
                    var mp = _memPunishmentRepo.getById(memPunishment_id);
                    if (mp == null)
                    {
                        throw new ItemNotFoundException("Member punishment not found.");
                    }
                    _memPunishmentRepo.delete(mp);
                    _memPunishmentRepo.saveChanges();
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
