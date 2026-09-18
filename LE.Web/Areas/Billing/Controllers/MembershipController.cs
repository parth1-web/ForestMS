using AutoMapper;
using ImageMagick;
using LE.Account.Infrastructure.Dto;
using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Enums;
using LE.Service.Services.Interface;
using LE.Web.Areas.Billing.FilterModel;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/membership")]
    public class MembershipController : BaseController
    {
        private readonly MemberService _memberService;
        private readonly MemberRepository _memberRepo;
        private readonly MemberPunishmentRepository _memPunishmentRepo;
        private readonly ToleRepository _toleRepo;
        private readonly PaginatedMetaService _paginatedMetaService;
        private readonly FileHelper _fileHelper;
        private readonly IMapper _mapper;
        private readonly DateConverterService _dateConverterService;
        private readonly MembershipService _membershipService;
        private readonly MembershipRepository _membershipRepo;
        private readonly MembershipValidityRepository _membershipValidityRepo;
        private readonly MembershipValidityService _membershipValidityService;
        private readonly LedgerGroupIdProviderService _ledgerGroupIdProviderService;
        private readonly LedgerService _ledgerService;

        public MembershipController(MemberService memberService, MemberRepository memberRepo, MemberPunishmentRepository memPunishmentRepo,
            ToleRepository toleRepo, PaginatedMetaService paginatedMetaService, FileHelper fileHelper, IMapper mapper,
            DateConverterService dateConverterService, MembershipService membershipService, MembershipRepository membershipRepo,
            MembershipValidityService membershipValidityService, LedgerGroupIdProviderService ledgerGroupIdProviderService, LedgerService ledgerService, MembershipValidityRepository membershipValidityRepo)
        {
            _memberService = memberService;
            _memberRepo = memberRepo;
            _memPunishmentRepo = memPunishmentRepo;
            _toleRepo = toleRepo;
            _paginatedMetaService = paginatedMetaService;
            _fileHelper = fileHelper;
            _mapper = mapper;
            _dateConverterService = dateConverterService;
            _membershipService = membershipService;
            _membershipRepo = membershipRepo;
            _membershipValidityService = membershipValidityService;
            _ledgerGroupIdProviderService = ledgerGroupIdProviderService;
            _ledgerService = ledgerService;
            _membershipValidityRepo = membershipValidityRepo;
        }

        [Route("")]
        [Route("index", Name = "billing_membership_index")]
        public IActionResult Index(MemberFilter filter)
        {
            var membership = _membershipRepo.getQueryable()
                .Include(m => m.MemberDetails)
                .Include(m => m.MembershipValidity)
                .Where(m => !m.IsCancelled);

            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                membership = membership.Where(a => a.MembershipCode.Contains(filter.name) || a.MemberDetails.Any(x => x.FullName.Contains(filter.name)));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(membership.Count(), filter.page, filter.number_of_rows);
            var membershipList = membership.OrderBy(a => a.MembershipId).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();

            var model = getViewModelFrom(membershipList);
            return View(model);
        }

        private List<MembershipDto> getViewModelFrom(List<Membership> membershipList)
        {
            List<MembershipDto> mmList = new List<MembershipDto>();
            foreach (var membership in membershipList)
            {
                var memberDetail = _mapper.Map<MembershipDto>(membership);
                memberDetail.MemberDetails = memberDetail.MemberDetails.Where(a => a.IsDeleted == false).ToList();
                memberDetail.CreatedDate = _dateConverterService.toBS(membership.CreatedDate.Date);
                //var currentDate = _dateConverterService.getDateByTimeZone();
                //var memPunished = _memPunishmentRepo.getQueryable().Where(a => a.MembershipId == member.MembershipId && a.PunishmentValidity.Date >= currentDate.Date)?.OrderByDescending(a => a.PunishmentValidity.Date).ToList();
                //memberDetail.IsPunished = memPunished.Count > 0;
                //if (memPunished.Any())
                //{
                //	memberDetail.NepPunishmentValidity = memPunished.FirstOrDefault().NepPunishmentValidity;
                //}
                mmList.Add(memberDetail);
            }
            return mmList;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            MembershipModel mm = new MembershipModel();
            MembershipValidityModel mv = new MembershipValidityModel();
            mv.IssueDate = "2079-02-17";
            mv.NepValidityDate = "2084-12-05";
            mm.MembershipValidity = mv;
            var tole = _toleRepo.getAll();
            ViewBag.toles = new SelectList(tole, "tole_id", "tole_no");
            return View(mm);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(MembershipModel membershipModel)
        {
            try
            {
                // P1 fix: the ambient TransactionScope was a no-op for EF Core; ledger +
                // membership + validity + members now run in one real DB transaction.
                using (var tx = _membershipRepo.beginTransaction())
                {
                    var createdBy = getLoggedInUserId();

                    var checkMembershipCode = _membershipRepo.GetByCode(membershipModel.MembershipCode);
                    if (checkMembershipCode != null)
                    {
                        return Json(new { success = false, message = "Membership code already exists." });
                    }

                    var memberLedger = membershipModel.MemberDetails.Where(x => x.IsGharmuli).FirstOrDefault();
                    if (memberLedger == null)
                        return Json(new { success = false, message = "No Gharmuli member found." });

                    //Create Ledger
                    var ledgerDto = new LedgerDto();
                    ledgerDto.name = $"{memberLedger.FullName} ({membershipModel.MembershipCode})";
                    ledgerDto.ledger_group_id = _ledgerGroupIdProviderService.getDebtorsGroupId();
                    ledgerDto.opening_balance = 0;
                    ledgerDto.balance_type = LE.Account.Common.Enums.OpeningBalanceType.debit;
                    ledgerDto.user_id = createdBy;
                    var ledger = _ledgerService.save(ledgerDto);

                    //Create Membership
                    var membershipDto = getMembershipDtoFromModel(membershipModel);
                    membershipDto.LedgerId = ledger.ledger_id;
                    membershipDto.CreatedBy = createdBy;
                    var membership = _membershipService.Insert(membershipDto);

                    //Create Membership Validity
                    membershipModel.MembershipValidity.MembershipId = membership.MembershipId;
                    var membershipValidityDto = getMembershipValidityDtoFromModel(membershipModel.MembershipValidity);
                    membershipValidityDto.CreatedBy = createdBy;
                    _membershipValidityService.Insert(membershipValidityDto);

                    //Create Members
                    membershipModel.MemberDetails.ForEach(x =>
                    {
                        x.MembershipId = membership.MembershipId;
                    });

                    foreach (var member in membershipModel.MemberDetails)
                    {
                        MemberDto memberDto = getMemberDtoFromModel(member);
                        if (member.ImageName != null)
                        {
                            memberDto.ImageName = _fileHelper.saveImageAndGetFileName(member.ImageName, "mem-");
                        }
                        memberDto.CreatedBy = createdBy;
                        _memberService.Insert(memberDto);
                    }
                    tx.Commit();

                    AlertHelper.setMessage(this, "Membership added successfully", messageType.success);
                    return Json(new { success = true, message = "Membership added successfully." });
                }
            }
            catch (Exception ex)
            {
                var tole = _toleRepo.getAll();
                ViewBag.toles = new SelectList(tole, "tole_id", "tole_no");
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                // P1/B15 fix: the failure branch returned success = true ("Membership
                // failed to save."), so the UI reported every failure as a success.
                return Json(new { success = false, message = "Membership failed to save." });
            }
        }

        [HttpGet]
        [Route("edit/{membershipId}")]
        public IActionResult edit(long membershipId)
        {
            var membership = _membershipRepo.getById(membershipId);
            var membershipModel = getMembershipModelFromMembership(membership);

            var toles = _toleRepo.getAll();
            ViewBag.toles = toles;

            return View(membershipModel);
        }

        private MembershipDto getMembershipModelFromMembership(Membership entity)
        {
            MembershipDto mm = new MembershipDto();
            mm.MembershipId = entity.MembershipId;
            mm.MembershipCode = entity.MembershipCode;
            mm.Area = entity.Area;
            mm.ToleNo = entity.ToleNo;
            mm.Religion = entity.Religion;
            mm.HasBioGas = entity.HasBioGas;
            mm.HasLPG = entity.HasLPG;
            mm.IsCancelled = entity.IsCancelled;
            mm.IsActive = entity.IsActive;
            mm.Remarks = entity.Remarks;
            mm.CreatedDate = _dateConverterService.toBS(entity.CreatedDate.Date);
            mm.LedgerId = entity.LedgerId;
            mm.CancelledDate = entity.CancelledDate;
            mm.CreatedBy = entity.CreatedBy;

            List<Member> membersList = new List<Member>();
            foreach (var member in entity.MemberDetails)
            {
                Member members = new Member();

                members.MemberId = member.MemberId;
                members.MembershipId = member.MembershipId;
                members.FullName = member.FullName;
                members.MemberCitizenship = member.MemberCitizenship;
                members.Address = member.Address;
                members.ContactNo = member.ContactNo;
                members.Age = member.Age;
                members.Gender = member.Gender;
                members.IsGharmuli = member.IsGharmuli;
                members.IsActive = member.IsActive;
                members.IsDeleted = member.IsDeleted;
                members.FamilyRole = member.FamilyRole;
                members.CreatedBy = member.CreatedBy;
                members.ImageName = member.ImageName;

                membersList.Add(members);
            }

            mm.MemberDetails = membersList;
            mm.MembershipValidity = entity.MembershipValidity;
            mm.MemberPunishments = entity.MemberPunishments;

            return mm;
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(MembershipModel model)
        {
            try
            {
                // P1 fix: real EF transaction (the ambient scope was a no-op).
                using (var tx = _membershipRepo.beginTransaction())
                {
                    var membership = _membershipRepo.getById(model.MembershipId);
                    var checkMembershipCode = _membershipRepo.getQueryable().Where(m => m.MembershipId != model.MembershipId && m.MembershipCode == model.MembershipCode).ToList().Count() > 0;
                    if (checkMembershipCode)
                    {
                        return Json(new { success = false, message = "Membership code already exists." });
                    }

                    //Update Membership
                    var membershipDto = getMembershipDtoFromModel(model);
                    _membershipService.Update(membershipDto);

                    if (model.MemberDetails != null && model.MemberDetails.Count() > 0)
                    {
                        //Add Members
                        model.MemberDetails.ForEach(x =>
                        {
                            x.MembershipId = membership.MembershipId;
                        });

                        foreach (var member in model.MemberDetails)
                        {
                            MemberDto memberDto = getMemberDtoFromModel(member);
                            if (member.ImageName != null)
                            {
                                memberDto.ImageName = _fileHelper.saveImageAndGetFileName(member.ImageName, "mem-");
                            }
                            memberDto.CreatedBy = getLoggedInUserId();
                            _memberService.Insert(memberDto);
                        }
                    }

                    tx.Commit();

                    AlertHelper.setMessage(this, "Membership added successfully", messageType.success);
                    return Json(new { success = true, message = "Membership updated successfully." });
                }
            }
            catch (Exception ex)
            {
                var toles = _toleRepo.getAll();
                ViewBag.toles = toles;
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enable/{membership_id}")]
        public IActionResult enable(long membership_id)
        {
            try
            {
                _membershipService.Enable(membership_id);
                AlertHelper.setMessage(this, "Membership enabled successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("disable/{membership_id}")]
        public IActionResult disable(long membership_id)
        {
            try
            {
                _membershipService.Disable(membership_id);
                AlertHelper.setMessage(this, "Membership disabled successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{membership_id}")]
        public IActionResult delete(long membership_id)
        {
            try
            {
                    // P1 fix: real EF transaction (the ambient scope was a no-op).
                    using (var tx = _membershipRepo.beginTransaction())
                    {
                        _membershipService.Delete(membership_id);

                        var ledgerId = _membershipRepo.getById(membership_id).LedgerId;
                        _ledgerService.delete(ledgerId);
                        tx.Commit();
                    }
                    AlertHelper.setMessage(this, "Membership deleted successfully.");
                    return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        private MemberModel getModelFromEntity(Member member)
        {
            return _mapper.Map<MemberModel>(member);
        }

        private MemberDto getMemberDtoFromModel(MemberModel memberModel)
        {
            return _mapper.Map<MemberDto>(memberModel);
        }

        private MembershipDto getMembershipDtoFromModel(MembershipModel membershipModel)
        {
            MembershipDto md = new MembershipDto();
            md.MembershipId = membershipModel.MembershipId;
            md.MembershipCode = membershipModel.MembershipCode;
            md.Area = membershipModel.Area;
            md.ToleNo = membershipModel.ToleNo;
            md.Religion = membershipModel.Religion;
            md.HasBioGas = membershipModel.HasBioGas;
            md.HasLPG = membershipModel.HasLPG;
            return md;
        }

        private MembershipValidityDto getMembershipValidityDtoFromModel(MembershipValidityModel membershipValidityModel)
        {
            var mvd = _mapper.Map<MembershipValidityDto>(membershipValidityModel);
            mvd.ValidityDate = _dateConverterService.toAD(membershipValidityModel.NepValidityDate);
            return mvd;
        }

        [HttpGet]
        [Route("getMemberhipDetails/{membershipId}")]
        public JsonResult getMemberhipDetails(long membershipId)
        {
            try
            {
                var membership = _membershipRepo.getById(membershipId);
                if (membership == null)
                {
                    return Json(new { success = false, message = "Memberhip not found" });
                }

                MembershipIndexViewModel mm = new MembershipIndexViewModel();
                mm.MembershipCode = membership.MembershipCode;
                mm.Area = membership.Area;
                mm.ToleNo = membership.ToleNo;
                mm.Religion = EnumExtensions.GetDisplayName(membership.Religion);
                mm.IsActive = membership.IsActive;
                mm.IsCancelled = membership.IsCancelled;
                mm.CreatedDate = _dateConverterService.toBS(membership.CreatedDate.Date);
                mm.HasBioGas = membership.HasBioGas;
                mm.HasLPG = membership.HasLPG;

                MembershipValidityIndexViewModel mvm = new MembershipValidityIndexViewModel();
                if (membership.MembershipValidity != null)
                {
                    mvm.ValidityDate = membership.MembershipValidity.ValidityDate.Date;
                    mvm.NepValidityDate = membership.MembershipValidity.NepValidityDate;
                }

                mm.MembershipValidity = mvm;

                List<MembersIndexViewModel> memberVM = new List<MembersIndexViewModel>();

                foreach (var member in membership.MemberDetails.Where(m => m.IsDeleted == false))
                {
                    MembersIndexViewModel mem = new MembersIndexViewModel();
                    mem.FullName = member.FullName;
                    mem.IsGharmuli = member.IsGharmuli;
                    mem.MemberCitizenship = !string.IsNullOrEmpty(member.MemberCitizenship) ? member.MemberCitizenship : "-";
                    mem.Age = member.Age;
                    mem.Gender = EnumExtensions.GetDisplayName(member.Gender);
                    mem.Address = !string.IsNullOrEmpty(member.Address) ? member.Address : "-";
                    mem.ContactNo = !string.IsNullOrEmpty(member.ContactNo) ? member.ContactNo : "-";
                    mem.FamilyRole = EnumExtensions.GetDisplayName(member.FamilyRole);
                    mem.CreatedDate = _dateConverterService.toBS(member.CreatedDate.Date);
                    mem.ImageName = member.ImageName;
                    mem.Status = member.IsActive ? "Active" : "Inactive";
                    memberVM.Add(mem);
                }

                mm.MemberDetails = memberVM;

                return Json(new
                {
                    success = true,
                    data = mm
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while fetching the membership details." });
            }
        }

        [HttpGet]
        [Route("members", Name = "billing_membership_members")]
        public IActionResult Members(MemberFilter filter)
        {
            var members = _memberRepo.getAll().Where(m => m.IsDeleted == false);
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                members = members.Where(a => a.Membership.MembershipCode.Contains(filter.name, StringComparison.OrdinalIgnoreCase) || a.FullName.Contains(filter.name, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(members.Count(), filter.page, filter.number_of_rows);
            var membersList = members.OrderBy(a => a.MemberId).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();
            return View(membersList);
        }

        [HttpGet]
        [Route("members/edit/{memberId}")]
        public IActionResult EditMember(long memberId)
        {
            var memberDetail = _memberRepo.getById(memberId);
            var model = _mapper.Map<MemberDetail>(memberDetail);
            return View(model);
        }

        [HttpPost]
        [Route("members/edit")]
        public IActionResult EditMember(MemberModel model)
        {
            try
            {
                var memberDetail = _mapper.Map<MemberDetail>(model);

                if (ModelState.IsValid)
                {
                    var members = _memberRepo.getQueryable();
                    if (model.IsGharmuli)
                    {
                        var isGharmuliValid = _memberRepo.getQueryable().Where(a => a.Membership.MembershipCode == model.Membership.MembershipCode).ToList().Count() <= 2;
                        if (!isGharmuliValid)
                        {
                            AlertHelper.setMessage(this, "Only 2 member are allowed to be gharmuli in a membership.", messageType.error);
                            return View(memberDetail);
                        }
                    }

                    if (!string.IsNullOrEmpty(model.MemberCitizenship))
                    {
                        var isCitizenshipNoDuplicate = members.Where(m => m.MemberId != model.MemberId && m.MemberCitizenship == model.MemberCitizenship).ToList().Count() > 0;
                        if (isCitizenshipNoDuplicate)
                        {
                            AlertHelper.setMessage(this, $"A member with the citizenship no. {model.MemberCitizenship} already exists.", messageType.error);
                            return View(memberDetail);
                        }
                    }

                    var dto = _mapper.Map<MemberDto>(model);

                    if (model.ImageName != null)
                    {
                        dto.ImageName = _fileHelper.saveImageAndGetFileName(model.ImageName, "mem-");
                    }

                    _memberService.Update(dto);
                    AlertHelper.setMessage(this, "Member updated successfully.");
                    return RedirectToAction("Members");
                }

                return View(memberDetail);

            }
            catch (Exception ex)
            {
                var memberDetail = _mapper.Map<MemberDetail>(model);
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return View(memberDetail);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("members/enable/{memberId}")]
        public IActionResult EnableMember(long memberId)
        {
            try
            {
                _memberService.Enable(memberId);
                AlertHelper.setMessage(this, "Member enabled successfully.");
                return RedirectToAction("Members");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("Members");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("members/disable/{memberId}")]
        public IActionResult DisableMember(long memberId)
        {
            try
            {
                _memberService.Disable(memberId);
                AlertHelper.setMessage(this, "Member disabled successfully.");
                return RedirectToAction("Members");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("Members");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("members/delete/{memberId}")]
        public IActionResult DeleteMember(long memberId)
        {
            try
            {
                _memberService.Delete(memberId);
                AlertHelper.setMessage(this, "Member deleted successfully.");
                return RedirectToAction("Members");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("Members");
            }
        }

        [HttpGet]
        [Route("renew/{membershipId}")]
        public IActionResult RenewMembership(long membershipId)
        {
            var membership = _membershipRepo.getById(membershipId);
            return View(membership);
        }

        [HttpPost]
        [Route("renew")]
        public IActionResult RenewMembership(RenewMembershipModel model)
        {
            var checkValidity = _membershipValidityRepo.getQueryable().Where(m => m.MembershipId == model.MembershipId && m.ValidityDate.Date >= DateTime.Now.Date).ToList().Count() > 0;
            if (checkValidity)
            {
                AlertHelper.setMessage(this, "Membership has validity and do not need to be renewed.", messageType.error);
                return RedirectToAction(nameof(Index));
            }
            var membershipValidity = _membershipValidityRepo.getQueryable().Where(m => m.MembershipId == model.MembershipId).FirstOrDefault();

            MembershipValidityDto mv = new MembershipValidityDto();
            mv.MembershipValidityId = membershipValidity.MembershipValidityId;
            mv.MembershipId = model.MembershipId;
            mv.NepValidityDate = model.NepValidityDate;
            mv.ValidityDate = _dateConverterService.toAD(model.NepValidityDate);
            mv.RenewedDate = _dateConverterService.toBS(DateTime.Now.Date);

            _membershipValidityService.Update(mv);

            AlertHelper.setMessage(this, $"Membership with code {membershipValidity.Membership.MembershipCode} successfully renewed till date {model.NepValidityDate}.", messageType.success);
            return RedirectToAction(nameof(Index));
        }
    }
}