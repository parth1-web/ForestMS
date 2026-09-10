using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Enums;
using LE.Common.Library;
using LE.Common.Library.DateConverter.Entity;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Area("billing")]
    [Route("billing/chiran-billing")]
    public class ChiranBillingController : BaseController
    {
        private readonly MemberRepository _memberRepo;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private readonly UserRepository _urerRepo;
        private readonly ChiranSalesService _chiranSalesService;
        private readonly ChiranSalesDetailService _chiranSalesDetailService;
        private readonly ChiranSalesRepository _chiranSalesRepo;
        private readonly ChiranSalesDetailRepository _chiranSalesDetailRepo;
        private readonly WoodTypeRepository _woodTypeRepo;
        private IMapper _mapper;
        private readonly MemberPunishmentController _memberPunishmentController;

        public ChiranBillingController(MemberRepository memberRepo,
            ChiranSalesService woodBillService, 
            ChiranSalesRepository woodBillRepo, 
            ChiranSalesDetailRepository woodBillDetailRepo,
            IMapper mapper, ChiranSalesDetailService woodDetailService, 
            UserRepository userRepo, OrganizationSetupRepository organizationSetupRepository,
            WoodTypeRepository woodTypeRepo, MemberPunishmentController memberPunishmentController
            )
        {
            _organizationSetupRepository = organizationSetupRepository;
            _memberRepo = memberRepo;
            _chiranSalesService = woodBillService;
            _chiranSalesRepo = woodBillRepo;
            _chiranSalesDetailRepo = woodBillDetailRepo;
            _mapper = mapper;
            _chiranSalesDetailService = woodDetailService;
            _urerRepo = userRepo;
            _woodTypeRepo = woodTypeRepo;
            _memberPunishmentController = memberPunishmentController;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            ChiranBillModel model = new ChiranBillModel();
            var membersList = _memberRepo.getQueryable().Where(a => a.IsActive && a.Membership.MembershipValidity.ValidityDate.Date >= DateTime.Now.Date).ToList();
            ViewBag.members = new SelectList(membersList, "MemberId", "FullName");
            var woodTypes = _woodTypeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.woodType = new SelectList(woodTypes, "wood_type_id", "name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("new")]
        public JsonResult add([FromBody] ChiranBillModel model)
        {
            try
            {

                var membershipId = _memberRepo.getQueryable().Where(a => a.MemberId == model.MemberId).FirstOrDefault().MembershipId; ;
                var isMembershipValid = _memberPunishmentController.GetMemberValidity(membershipId);
                if (isMembershipValid)
                {
                    ChiranSalesDto dto = getWoodBillDtoFromModel(model);
                    long billId = _chiranSalesService.insert(dto);
                    return Json(billId);
                }
                else
                {
                    return Json(new { error = true, responseText = "Membership is punished!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = true, responseText = ex.Message });
            }
        }

        private ChiranSalesDto getWoodBillDtoFromModel(ChiranBillModel model)
        {
            ChiranSalesDto dto = new ChiranSalesDto();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var dateService = DateConverterFactory.getDateConverterService();
            var currentTime = dateFunction.getDateTimeByTimeZone();

            dto.sales_date = dateService.ToAD(model.nep_sales_date).getFormattedDate().Add(currentTime.TimeOfDay);
            dto.amount = model.total_amount;
            dto.remarks = model.remarks;
            dto.user_id = getLoggedInUserId();
            dto.sales_type = model.sales_type;
            dto.name = model.other_customer;
            dto.address = model.address;
            dto.member_id = model.MemberId;

            if (model.tax_percentage > 0)
            {
                dto.tax_amount = Math.Round((model.tax_percentage * dto.amount) / 100, 2);
            }

            List<ChiranSalesDetailDto> chiranSalesDetailDtos = new List<ChiranSalesDetailDto>();

            foreach (var item in model.chiranItems)
            {
                ChiranSalesDetailDto detailDto = new ChiranSalesDetailDto();
                detailDto.rate = item.rate;
                detailDto.wood_type_id = item.wood_type_id;
                detailDto.circle_size = item.circle_size;
                detailDto.length = item.length;
                detailDto.breadth = item.breadth;
                decimal size = Math.Round((item.circle_size * item.length * item.breadth * item.quantity) / 144, 2);
                detailDto.total_size = size;
                detailDto.amount = size * item.rate;
                detailDto.quantity = item.quantity;
                chiranSalesDetailDtos.Add(detailDto);
            }
            dto.addChiranBillDetails(chiranSalesDetailDtos);
            return dto;
        }

        [HttpGet]
        [Route("bill")]
        public IActionResult bill(long chiran_sales_id)
        {
            try
            {
                ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
                ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

                var dateConverterService = DateConverterFactory.getDateConverterService();
                var billData = _chiranSalesDetailRepo.getQueryable().Where(a => a.chiran_sales_id == chiran_sales_id).ToList();

                string name = "";
                string address = "";
                string tole_no = "";
                var bill = _chiranSalesRepo.getById(chiran_sales_id);
                if (bill.sales_type == SalesType.Member)
                {
                    var members = _memberRepo.getQueryable().Where(a => a.MemberId == bill.member_id).Single();
                    name = members.FullName;
                    address = members.Address;
                    tole_no = members.Membership.ToleNo;
                }
                else if (bill.sales_type == SalesType.Antarik)
                {
                    name = "Antarik";
                }
                else
                {
                    name = bill.name;
                    address = bill.address;
                }

                ChiranBillIndexViewModel vm = getBillViewModelFrom(billData);
                vm.logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;
                vm.member = name;
                vm.address = address;
                vm.tole_no = tole_no;
                vm.nep_sales_date = bill.nep_sales_date;
                vm.remarks = bill.remarks;
                vm.user = _urerRepo.getById(getLoggedInUserId()).full_name;
                vm.amount = bill.amount;
                decimal taxAmt = bill.tax_amount;
                vm.tax_amount = taxAmt;
                vm.chiran_sales_id = bill.chiran_sales_id;
                vm.print_date = dateConverterService.ToBS(DateTime.Now.Date, NepaliDate.DateFormats.yMd).getFormattedDate();
                vm.print_time = DateTime.Now.ToShortTimeString();
                vm.numWords = NumberToNepaliCurrencyText.NumberToCurrencyText(bill.amount + taxAmt, MidpointRounding.AwayFromZero);

                return View(vm);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return Redirect("/inventory/wood-details");
            }
        }

        private ChiranBillIndexViewModel getBillViewModelFrom(List<ChiranSalesDetail> billData)
        {
            ChiranBillIndexViewModel VM = new ChiranBillIndexViewModel();
            VM.bill_details = new List<ChiranDetail>();
            foreach (var sale in billData)
            {
                var billDetail = _mapper.Map<ChiranDetail>(sale);
                VM.bill_details.Add(billDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(ChiranBillReportIndexViewModel vm)
        {
            vm = getReportViewModel(vm);
            return View(vm);
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(ChiranBillReportIndexViewModel vm)
        {
            vm = getReportViewModel(vm);

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            return View(vm);
        }

        private ChiranBillReportIndexViewModel getReportViewModel(ChiranBillReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var details = _chiranSalesRepo.getQueryable().Where(a => a.sales_date.Date >= startDate.Date && a.sales_date.Date <= endDate.Date && a.is_cancelled == vm.is_cancelled).ToList();


            foreach (var detail in details)
            {
                var bill = _mapper.Map<ChiranBillReportDetails>(detail);
                if (detail.sales_type == SalesType.Member)
                {
                    if (detail.member_id > 0)
                    {
                        var memberName = _memberRepo.getById((long)detail.member_id).FullName;
                        bill.name = memberName;
                    }
                }
                else
                {
                    bill.name = bill.name;
                }
                vm.chiran_bill_datas.Add(bill);
            }

            return vm;
        }


        [HttpGet]
        [Route("cancel/{chiran_sales_id}")]
        public IActionResult cancel(long chiran_sales_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _chiranSalesService.cancel(chiran_sales_id, userId);
                AlertHelper.setMessage(this, "Chiran Bill Cancelled Successfully", messageType.success);
                return RedirectToAction("report");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("report");
            }
        }
    }
}