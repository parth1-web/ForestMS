using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Enums;
using LE.Common.Library.DateConverter.Entity;
using LE.Common.Library;
using LE.Service.Repository.Interface;
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
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/member-punishment")]
    public class MemberPunishmentController : BaseController
    {
        private MemberRepository _memRepo;
        private MemberPunishmentService _memPunishmentService;
        private MemberPunishmentRepository _memPunishmentRepo;
        private PaginatedMetaService _paginatedMetaService;
        private FileHelper _fileHelper;
        private IMapper _mapper;
        private DateConverterService _dateConverterService;
        private MembershipRepository _membershipRepo;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private readonly UserRepository _urerRepo;



        public MemberPunishmentController(MemberRepository memRepo,
            MemberPunishmentService memPunishmentService,
            MemberPunishmentRepository memPunishmentRepo,
            PaginatedMetaService paginatedMetaService,
            FileHelper fileHelper, IMapper mapper,
            DateConverterService dateConverterService, MembershipRepository membershipRepo, OrganizationSetupRepository organizationSetupRepository, UserRepository urerRepo)
        {
            _memRepo = memRepo;
            _memPunishmentService = memPunishmentService;
            _memPunishmentRepo = memPunishmentRepo;
            _paginatedMetaService = paginatedMetaService;
            _fileHelper = fileHelper;
            _mapper = mapper;
            _dateConverterService = dateConverterService;
            _membershipRepo = membershipRepo;
            _organizationSetupRepository = organizationSetupRepository;
            _urerRepo = urerRepo;
        }

        [Route("")]
        [Route("index", Name = "billing_memberpunishment_index")]
        public IActionResult Index(MemberFilter filter)
        {
            var memPunishment = _memPunishmentRepo.getQueryable()
                .Include(a => a.Membership)
                    .ThenInclude(m => m.MemberDetails)
                .Where(a => a.IsCancelled == false);
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                memPunishment = memPunishment.Where(a => a.Membership.MembershipCode.Contains(filter.name));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(memPunishment.Count(), filter.page, filter.number_of_rows);
            var memPunishmentList = memPunishment.OrderBy(a => a.MemberPunishmentId).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();

            MemberPunishmentViewModel memIndexVm = getViewModelFrom(memPunishmentList);
            return View(memIndexVm);
        }

        private MemberPunishmentViewModel getViewModelFrom(List<MemberPunishment> memPunishments)
        {
            MemberPunishmentViewModel vm = new MemberPunishmentViewModel();
            vm.MemPunishment = new List<MemberPunishmentDetail>();

            foreach (var item in memPunishments)
            {
                var memberDetail = new MemberPunishmentDetail();
                memberDetail.MembershipId = item.MembershipId;
                memberDetail.IllegalActivity = item.IllegalActivity;
                memberDetail.PunishmentValidity = item.PunishmentValidity;
                memberDetail.NepPunishmentValidity = item.NepPunishmentValidity;
                memberDetail.Remarks = item.Remarks;
                memberDetail.Membership = item.Membership;
                memberDetail.CreatedDate = item.CreatedDate;
                memberDetail.IsCancelledRemarks = item.IsCancelledRemarks;
                memberDetail.MemberPunishmentId = item.MemberPunishmentId;
                //memberDetail.MemberId = item.MemberId;
                //memberDetail.Member = item.Member;

                var currentDate = _dateConverterService.getDateByTimeZone().Date;
                memberDetail.IsPunished = item.PunishmentValidity.Date > currentDate;



                vm.MemPunishment.Add(memberDetail);
            }
            return vm;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            ViewBag.members = getMemberSelectList();
            return View();
        }

        private SelectList getMemberSelectList()
        {
            var members = _memRepo.getQueryable()
                .Include(a => a.Membership)
                .Where(a => a.IsActive)
                .ToList();
            members.ForEach(mem => mem.FullName = $"{mem.FullName} ( {mem.Membership?.MembershipCode ?? "N/A"} )");
            return new SelectList(members, "MembershipId", "FullName");
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(MemberPunishmentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.members = getMemberSelectList();
                    AlertHelper.setMessage(this, "Please enter valid details.", messageType.error);
                    return View(dto);
                }

                // Check if the member is already punished
                if (IsMemberAlreadyPunished(dto.MembershipId))
                {
                    ViewBag.members = getMemberSelectList();
                    AlertHelper.setMessage(this, "Member is already punished.", messageType.error);
                    return View(dto);
                }

                dto.NepIssueDate = _dateConverterService.toBS(dto.IssueDate);
                dto.PunishmentValidity = _dateConverterService.toAD(dto.NepPunishmentValidity);
                dto.CreatedBy = getLoggedInAuthenticationId();
                _memPunishmentService.Insert(dto);
                AlertHelper.setMessage(this, "Member punishment added successfully", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ViewBag.members = getMemberSelectList();
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return View(dto);
            }
        }

        private bool IsMemberAlreadyPunished(long MembershipId)
        {
            // P1/B6 fix: was 'Any(p => p.MembershipId == MembershipId)' — once a member had
            // ANY punishment record (even cancelled/expired) no new punishment could be
            // added. Only a currently-active punishment should block.
            var today = _dateConverterService.getDateByTimeZone().Date;
            return _memPunishmentRepo.getQueryable()
                .Any(p => p.MembershipId == MembershipId && p.IsActive && !p.IsCancelled && p.PunishmentValidity.Date >= today);
        }

        [HttpPost]
        [Route("cancelled")]
        public JsonResult Edit([FromBody] MemberPunishmentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Please enter valid details." });
                }

                var memberPunishment = _memPunishmentRepo.getById(model.MemberPunishmentId);

                if (memberPunishment == null)
                {
                    return Json(new { success = false, message = "Member Punishment not found" });
                }

                var memPunishmentDto = _mapper.Map<MemberPunishmentDto>(memberPunishment);
                memPunishmentDto.IsCancelledRemarks = model.IsCancelledRemarks;
                memPunishmentDto.IsCancelled = true;

                _memPunishmentService.Update(memPunishmentDto);

                return Json(new { success = true, message = "Member punishment cancelled successfully!" });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(ExceptionMessageHelper.buildFailObject(ex));
            }
        }

        //this code : to check the punishment of the member while billing//
        [HttpGet("validity/{MembershipId}")]
        public bool GetMemberValidity(long MembershipId)
        {
            // P1/B6 fix: the query was inverted (!p.IsActive flagged INACTIVE punishments)
            // and ignored the punishment validity date, so expired punishments blocked
            // billing forever and cancelled ones were the only ones that mattered.
            // A member is blocked only by an active, non-cancelled punishment that has
            // not yet expired.
            var today = _dateConverterService.getDateByTimeZone().Date;
            bool hasActivePunishment = _memPunishmentRepo.getQueryable()
                .Any(p => p.MembershipId == MembershipId && p.IsActive && !p.IsCancelled && p.PunishmentValidity.Date >= today);

            return !hasActivePunishment;
        }

        [HttpGet("check-member-validity/{MemberId}")]
        public JsonResult getMemberPunishmentValidity(long MemberId)
        {
            var member = _memRepo.getQueryable().Where(a => a.MemberId == MemberId).FirstOrDefault();
            if (member == null)
                return Json(false);

            var membershipId = member.MembershipId;

            bool isValid = GetMemberValidity(membershipId);
            return Json(isValid);
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(MemberPunishmentReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            vm = getReportViewModel(vm);

            foreach (var report in vm.member_punishment_report)
            {
                if (report.IssueDate != DateTime.MinValue)      {
                    // Convert IssueDate to Nepali Date format
                    report.NepIssueDate = dateConverterService.ToBS(report.IssueDate, DateFormats.yMd).getFormattedDate();
                }
            }

            return View(vm);
        }


        private MemberPunishmentReportIndexViewModel getReportViewModel(MemberPunishmentReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var details = _memPunishmentRepo.getQueryable()
            .Where(a => a.IssueDate.Date >= startDate.Date && a.IssueDate.Date <= endDate.Date).ToList();

            if (vm.status == PunishmentStatus.completed)
            {
                details = details.Where(a => !a.IsCancelled && a.PunishmentValidity.Date < DateTime.Now.Date).ToList();
            }
            else if (vm.status == PunishmentStatus.cancelled)
            {
                details = details.Where(a => a.IsCancelled).ToList();
            }
            else if (vm.status == PunishmentStatus.active)
            {
                details = details.Where(a => !a.IsCancelled && a.PunishmentValidity.Date >= DateTime.Now.Date).ToList();
            }

            foreach (var detail in details)
            {
                var reports = _mapper.Map<MemberPunishmentReportDetails>(detail);

                vm.member_punishment_report.Add(reports);
            }

            return vm;
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(MemberPunishmentReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            vm = getReportViewModel(vm);

            // Convert IssueDate to Nepali Date for each punishment report
            foreach (var report in vm.member_punishment_report)
            {
                report.NepIssueDate = dateConverterService.ToBS(report.IssueDate, DateFormats.yMd).getFormattedDate();
            }

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

            return View(vm);
        }

        [HttpGet]
        [Route("SingleReport")]
        public IActionResult SingleReport(long MemberPunishmentId)
        {
            try
            {
                ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
                ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

                var dateConverterService = DateConverterFactory.getDateConverterService();

                var punishmentData = _memPunishmentRepo.getQueryable().Where(a => a.MemberPunishmentId == MemberPunishmentId).SingleOrDefault();

                if (punishmentData == null)
                {
                    throw new Exception("Punishment record not found.");
                }

                var member = _memRepo.getById(punishmentData.MembershipId);
                var membership = _membershipRepo.getById(punishmentData.MembershipId);

                MemberPunishmentSingleReportIndexViewModel vm = new MemberPunishmentSingleReportIndexViewModel
                {
                    MemPunishmentReportDetail = punishmentData,
                    logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value,
                    //user = _urerRepo.getById(getLoggedInUserId()).full_name,
                    print_date = dateConverterService.ToBS(DateTime.Now.Date, NepaliDate.DateFormats.yMd).getFormattedDate(),
                    print_time = DateTime.Now.ToShortTimeString()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return Redirect("/inventory/wood-details");
            }
        }
       
    }
}