using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Billing.Common.Enums;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Enums;
using LE.Common.Library;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/fire-wood-billing")]
    public class FirewoodBillingController : BaseController
    {
        private readonly MemberRepository _memberRepo;
        private readonly WoodTypeRepository _woodTypeRepo;
        private readonly StockItemRepository _stockItemRepo;
        private readonly FirewoodSalesDetailRepository _firewoodSalesDetailRepo;
        private readonly FirewoodSalesRepository _firewoodSalesRepo;
        private readonly FirewoodSalesService _firwoodSalesService;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private IMapper _mapper;
        private readonly MemberPunishmentController _memberPunishmentController;


        public FirewoodBillingController(MemberRepository memberRepo, 
            WoodTypeRepository woodTypeRepo, 
            StockItemRepository stockItemRepo, 
            FirewoodSalesService firewoodSalesService,
            FirewoodSalesDetailRepository firewoodSalesDetailRepo, 
            IMapper mapper, FirewoodSalesRepository firewoodSalesRepo,
            OrganizationSetupRepository organizationSetupRepository, 
            MemberPunishmentController memberPunishmentController)
        {
            _organizationSetupRepository = organizationSetupRepository;
            _memberRepo = memberRepo;
            _woodTypeRepo = woodTypeRepo;
            _stockItemRepo = stockItemRepo;
            _firwoodSalesService = firewoodSalesService;
            _firewoodSalesDetailRepo = firewoodSalesDetailRepo;
            _mapper = mapper;
            _firewoodSalesRepo = firewoodSalesRepo;
            _memberPunishmentController = memberPunishmentController;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            FireWoodBillModel model = new FireWoodBillModel();
            var membersList = _memberRepo.getQueryable().Where(a => a.IsActive && a.Membership.MembershipValidity.ValidityDate.Date >= DateTime.Now.Date).ToList();
            ViewBag.members = new SelectList(membersList, "MemberId", "FullName");

            var firewoodItem = _stockItemRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.firewoodItems = new SelectList(firewoodItem, "stock_item_id", "name");

            return View(model);
        }

        [HttpPost]
        [Route("new")]
        [IgnoreAntiforgeryToken]
        public IActionResult add([FromBody] FireWoodBillModel model)
        {
            try
            {
                var membershipId = _memberRepo.getQueryable().Where(a => a.MemberId == model.MemberId).FirstOrDefault().MembershipId; ;
                var isMembershipValid = _memberPunishmentController.GetMemberValidity(membershipId);
                if (isMembershipValid)
                {
                FirewoodSalesDto firewoodSalesDto = getDtoFromModel(model);
                long billId = _firwoodSalesService.makeSales(firewoodSalesDto);
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

        private FirewoodSalesDto getDtoFromModel(FireWoodBillModel model)
        {
            FirewoodSalesDto dto = new FirewoodSalesDto();
            setFirewoodSalesDto(model, dto);
            setFirewoodSalesDetailDto(dto, model);
            return dto;
        }

        private void setFirewoodSalesDto(FireWoodBillModel model, FirewoodSalesDto dto)
        {
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var dateService = DateConverterFactory.getDateConverterService();
            var currentTime = dateFunction.getDateTimeByTimeZone();

            dto.sales_date = dateService.ToAD(model.nep_sales_date).getFormattedDate().Add(currentTime.TimeOfDay);
            dto.sales_type = (FirewoodSalesType)Enum.Parse(typeof(FirewoodSalesType), model.sales_type, true);
            dto.type_id = model.type_id;
            dto.others_name = model.others_name;
            dto.address = model.address;
            dto.total_amount = model.total_amount;
            dto.remarks = model.remarks;
            dto.user_id = getLoggedInUserId();
        }

        private void setFirewoodSalesDetailDto(FirewoodSalesDto dto, FireWoodBillModel model)
        {
            foreach (var detail in model.items)
            {
                FirewoodSalesDetailDto sales_dto = new FirewoodSalesDetailDto();
                sales_dto.amount = Math.Round(detail.quantity * detail.rate, 2);
                sales_dto.quantity = detail.quantity;
                sales_dto.rate = detail.rate;
                sales_dto.stock_item_id = detail.stock_item_id;
                dto.addSalesDatas(sales_dto);
            }

        }

        [HttpGet]
        [Route("bill")]
        public IActionResult bill(long firewood_sales_id)
        {
            try
            {

                ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
                ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

                var billData = _firewoodSalesDetailRepo.getQueryable().Where(a => a.firewood_sales_id == firewood_sales_id).ToList();
                var bill = _firewoodSalesRepo.getById(firewood_sales_id);
                FireWoodBillIndexViewModel vm = getBillViewModelFrom(billData);
                vm.amount = bill.total_amount;
                vm.logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;
                vm.nep_sales_date = bill.nep_sales_date;
                vm.remarks = bill.remarks;
                vm.user = _userRepo.getById(getLoggedInUserId()).full_name;
                vm.address = bill.address;
                vm.firewood_sales_id = bill.firewood_sales_id;
                vm.numWords = NumberToNepaliCurrencyText.NumberToCurrencyText(bill.total_amount, MidpointRounding.AwayFromZero);
                vm.print_date = DateConverterFactory.getDateConverterService().ToBS(DateTime.Now.Date, DateFormats.yMd).getFormattedDate();
                vm.print_time = DateTime.Now.ToShortTimeString();

                if (bill.sales_type == FirewoodSalesType.Member)
                {
                    var member = _memberRepo.getById(Convert.ToInt32(bill.type_id));
                    vm.customer = member.FullName;
                    vm.address = member.Address;
                    vm.tole_no = member.Membership.ToleNo;
                }
                else
                {
                    vm.customer = bill.others_name;
                    vm.address = bill.address;
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, responseText = ex.Message });
            }

        }

        private FireWoodBillIndexViewModel getBillViewModelFrom(List<FirewoodSalesDetail> billData)
        {
            FireWoodBillIndexViewModel VM = new FireWoodBillIndexViewModel();
            VM.firewood_bill_details = new List<FirewoodBillDetail>();
            foreach (var sale in billData)
            {
                var billDetail = _mapper.Map<FirewoodBillDetail>(sale);
                VM.firewood_bill_details.Add(billDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(FireWoodBillReportIndexViewModel vm)
        {
            setReportViewModel(vm);
            return View(vm);
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(FireWoodBillReportIndexViewModel vm)
        {
            setReportViewModel(vm);

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            return View(vm);
        }

        private void setReportViewModel(FireWoodBillReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var details = _firewoodSalesRepo.getQueryable().Where(a => a.sales_date.Date >= startDate && a.sales_date.Date <= endDate && a.is_cancelled == vm.is_cancelled).ToList();

            foreach (var detail in details)
            {
                var bill = _mapper.Map<FirewoodBillReportDetails>(detail);
                if (detail.sales_type == FirewoodSalesType.Member)
                {
                    if (detail.type_id != null)
                    {
                        bill.others_name = _memberRepo.getById(Convert.ToInt32(detail.type_id)).FullName;
                    }
                }
                else
                {
                    bill.others_name = bill.others_name;
                }
                vm.firewood_bill_datas.Add(bill);
            }
        }

        [HttpGet]
        [Route("cancel/{firewood_bill_id}")]
        public IActionResult cancel(long firewood_bill_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _firwoodSalesService.cancel(firewood_bill_id, userId);
                AlertHelper.setMessage(this, "Firewood Bill Cancelled Successfully.", messageType.success);
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