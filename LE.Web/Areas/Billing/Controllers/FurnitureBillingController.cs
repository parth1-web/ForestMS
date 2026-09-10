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
    [Route("billing/furniture-billing")]
    public class FurnitureBillingController : BaseController
    {
        private readonly MemberRepository _memberRepo;
        private readonly WoodTypeRepository _woodTypeRepo;
        private readonly FurnitureRepository _furnitureRepo;
        private readonly FurnitureSalesDetailRepository _furnitureSalesDetailRepo;
        private readonly FurnitureSalesRepository _furnitureSalesRepo;
        private readonly FurnitureSalesService _furnitureSalesService;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
		private readonly UserRepository _urerRepo;
		private IMapper _mapper;

		public FurnitureBillingController(MemberRepository memberRepo, WoodTypeRepository woodTypeRepo, FurnitureRepository furnitureRepo, FurnitureSalesDetailRepository furnitureSalesDetailRepo, FurnitureSalesRepository furnitureSalesRepo, FurnitureSalesService furnitureSalesService, OrganizationSetupRepository organizationSetupRepository, IMapper mapper, UserRepository urerRepo)
		{
			_memberRepo = memberRepo;
			_woodTypeRepo = woodTypeRepo;
			_furnitureRepo = furnitureRepo;
			_furnitureSalesDetailRepo = furnitureSalesDetailRepo;
			_furnitureSalesRepo = furnitureSalesRepo;
			_furnitureSalesService = furnitureSalesService;
			_organizationSetupRepository = organizationSetupRepository;
			_mapper = mapper;
			_urerRepo = urerRepo;
		}

		[HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            FurnitureBillModel model = new FurnitureBillModel();
            var membersList = _memberRepo.getQueryable().Where(a => a.IsActive && a.Membership.MembershipValidity.ValidityDate.Date >= DateTime.Now.Date).ToList();
            ViewBag.members = new SelectList(membersList, "MemberId", "FullName");

            var furnitureItem = _furnitureRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.furnitureItems = new SelectList(furnitureItem, "furniture_id", "name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("new")]
        public IActionResult add([FromBody] FurnitureBillModel model)
        {
            try
            {
                //if (Convert.ToInt16(model.sales_type) == (int)FirewoodSalesType.Member)
                //{
                //    if (model.type_id > 0)
                //    {
                //        var memDetail = _memberRepo.getById((long)model.type_id);
                //        if (memDetail != null)
                //        {
                //            var checkMemberValidity = memDetail.Membership.MembershipValidity.ValidityDate.Date >= DateTime.Now.Date;
                //            if (!checkMemberValidity)
                //            {
                //                return Json(new {error = true, responseText = "Member with membership has been expired." });
                //            }
                //        }
                //    }
                //}
                FurnitureSalesDto furnitureSalesDto = getDtoFromModel(model);
                long billId = _furnitureSalesService.makeSales(furnitureSalesDto);
                return Json(billId);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, responseText = ex.Message });
            }
        }

        private FurnitureSalesDto getDtoFromModel(FurnitureBillModel model)
        {
            FurnitureSalesDto dto = new FurnitureSalesDto();
            setFurnitureSalesDto(model, dto);
            setFurnitureSalesDetailDto(dto, model);
            return dto;
        }

        private void setFurnitureSalesDto(FurnitureBillModel model, FurnitureSalesDto dto)
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
            dto.user_id = getLoggedInAuthenticationId();
        }

        private void setFurnitureSalesDetailDto(FurnitureSalesDto dto, FurnitureBillModel model)
        {
            foreach (var detail in model.items)
            {
                FurnitureSalesDetailDto sales_dto = new FurnitureSalesDetailDto();
                sales_dto.amount = Math.Round(detail.quantity * detail.rate, 2);
                sales_dto.quantity = detail.quantity;
                sales_dto.rate = detail.rate;
                sales_dto.furniture_id = detail.furniture_id;
                dto.addSalesDatas(sales_dto);
            }
        }

        [HttpGet]
        [Route("bill")]
        public IActionResult bill(long furniture_sales_id)
        {
            try
            {

                ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
                ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
                ViewBag.PanNo = _organizationSetupRepository.getByKey(OrganizationSetup.Pan_No.ToString()).value;

                var billData = _furnitureSalesDetailRepo.getQueryable().Where(a => a.furniture_sales_id == furniture_sales_id).ToList();
                var bill = _furnitureSalesRepo.getById(furniture_sales_id);
                FurnitureBillIndexViewModel vm = getBillViewModelFrom(billData);
                vm.amount = bill.total_amount;
                vm.logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;
                vm.nep_sales_date = bill.nep_sales_date;
                vm.remarks = bill.remarks;
                vm.user = _userRepo.getById(getLoggedInUserId()).full_name;
                vm.address = bill.address;
                vm.furniture_sales_id = bill.furniture_sales_id;
                vm.numWords = NumberToNepaliCurrencyText.NumberToCurrencyText(bill.total_amount, MidpointRounding.AwayFromZero);
                vm.print_date = DateConverterFactory.getDateConverterService().ToBS(DateTime.Now.Date, DateFormats.yMd).getFormattedDate();
                vm.print_time = DateTime.Now.ToShortTimeString();

                if (bill.sales_type == FirewoodSalesType.Member)
                {
                    var member = _memberRepo.getById(Convert.ToInt32(bill.type_id));
                    vm.customer = member.FullName + " (" + member.Membership.MembershipCode + ")";
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

        private FurnitureBillIndexViewModel getBillViewModelFrom(List<FurnitureSalesDetail> billData)
        {
            FurnitureBillIndexViewModel VM = new FurnitureBillIndexViewModel();
            VM.furniture_bill_details = new List<FurnitureBillDetail>();
            foreach (var sale in billData)
            {
                var billDetail = _mapper.Map<FurnitureBillDetail>(sale);
                VM.furniture_bill_details.Add(billDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(FurnitureBillReportIndexViewModel vm)
        {
            setReportViewModel(vm);
            return View(vm);
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(FurnitureBillReportIndexViewModel vm)
        {
            setReportViewModel(vm);

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            return View(vm);
        }

        private void setReportViewModel(FurnitureBillReportIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var details = _furnitureSalesRepo.getQueryable().Where(a => a.sales_date.Date >= startDate && a.sales_date.Date <= endDate && a.is_cancelled == vm.is_cancelled).ToList();

            foreach (var detail in details)
            {
                var bill = _mapper.Map<FurnitureBillReportDetails>(detail);
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
                vm.furniture_bill_datas.Add(bill);
            }
        }

        [HttpGet]
        [Route("cancel/{furniture_bill_id}")]
        public IActionResult cancel(long furniture_bill_id)
        {
            try
            {
                long userId = getLoggedInAuthenticationId();
                _furnitureSalesService.cancel(furniture_bill_id, userId);
                AlertHelper.setMessage(this, "Furniture Bill Cancelled Successfully.", messageType.success);
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