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
using LE.Inventory.Common.Enums;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Services.Interface;
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
    [Route("billing/wood-billing")]
    public class WoodBillingController : BaseController
    {
        private readonly MemberRepository _memberRepo;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private readonly UserRepository _urerRepo;
        private readonly WoodDetailsRepository _woodDetailRepo;
        private readonly WoodBillService _woodBillService;
        private readonly WoodDetailsService _woodDetailService;
        private readonly WoodBillRepository _woodBillRepo;
        private readonly WoodBillMemberRepository _woodBillMemberRepo;
        private readonly WoodBillDetailRepository _woodBillDetailRepo;
        private IMapper _mapper;
        private readonly MemberPunishmentController _memberPunishmentController;

        public WoodBillingController(MemberRepository memberRepo,
            WoodDetailsRepository woodDetailRepo,
            WoodBillService woodBillService,
            WoodBillRepository woodBillRepo,
            WoodBillMemberRepository woodBillMemberRepo,
            WoodBillDetailRepository woodBillDetailRepo,
            IMapper mapper, WoodDetailsService woodDetailService,
            UserRepository userRepo,
            OrganizationSetupRepository organizationSetupRepository,
            MemberPunishmentController memberPunishmentController)
        {
            _organizationSetupRepository = organizationSetupRepository;
            _memberRepo = memberRepo;
            _woodDetailRepo = woodDetailRepo;
            _woodBillService = woodBillService;
            _woodBillRepo = woodBillRepo;
            _woodBillDetailRepo = woodBillDetailRepo;
            _mapper = mapper;
            _woodDetailService = woodDetailService;
            _woodBillMemberRepo = woodBillMemberRepo;
            _urerRepo = userRepo;
            _memberPunishmentController = memberPunishmentController;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("new/{type}")]
        public IActionResult add(long type)
        {
            WoodBillModel model = new WoodBillModel();
            var membersList = _memberRepo.getQueryable().Where(a => a.IsActive && a.Membership.MembershipValidity.ValidityDate.Date >= DateTime.Now.Date).ToList();
            ViewBag.members = membersList;
            var woodDetail = _woodDetailRepo.getQueryable().Where(a => a.is_sold == false && a.stock_type_id == type).Select(s => new { id = s.wood_details_id, name = string.Format("{0}--{1}", s.goliya_number, (StockTypes)s.stock_type_id) }).ToList();
            ViewBag.woodDetails = new SelectList(woodDetail, "id", "name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("new")]
        public JsonResult add([FromBody] WoodBillModel model)
        {
            try
            {
                var isMembershipValid = true;
                foreach (var member in model.members)
                {
                    var membershipId = _memberRepo.getQueryable().Where(a => a.MemberId == member.MemberId).FirstOrDefault().MembershipId; 
                    isMembershipValid = _memberPunishmentController.GetMemberValidity(membershipId);
                    if (!isMembershipValid)
                    {
                        return Json(new {error=true, responseText= "one of the member is punished." });
                    }
                }

                if (isMembershipValid)
                {

                    WoodBillDto woodBillDto = getWoodBillDtoFromModel(model);
                    long billId = _woodBillService.insert(woodBillDto);
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

        //TODO refactoring required
        private WoodBillDto getWoodBillDtoFromModel(WoodBillModel model)
        {
            WoodBillDto dto = new WoodBillDto();
            setWoodBillDto(model, dto);

            List<WoodBillDetailDto> woodDetailDto = setWoodBillDetailDto(model, dto);

            List<WoodBillMemberDto> woodBillMemberDtos = new List<WoodBillMemberDto>();

            List<WoodBillMemberTransactionDto> transDtos = new List<WoodBillMemberTransactionDto>();

            foreach (var member in model.members)
            {
                WoodBillMemberDto memberDto = new WoodBillMemberDto();
                memberDto.wood_bill_id = dto.wood_bill_id;
                memberDto.member_id = member.MemberId;
                woodBillMemberDtos.Add(memberDto);
                for (int detailIndex = 0; detailIndex < model.items.Count; detailIndex++)
                {
                    var detail = model.items[detailIndex];
                    var memberCount = model.members.Count();
                    WoodBillMemberTransactionDto transDto = new WoodBillMemberTransactionDto();
                    transDto.wood_details_id = detail.wood_details_id;
                    transDto.wood_bill_id = dto.wood_bill_id;
                    transDto.rate = detail.rate;
                    transDto.member_id = member.MemberId;
                    var qtyDetail = _woodDetailRepo.getById(detail.wood_details_id);

                    var qty = qtyDetail.getNetTotal();

                    if (qtyDetail.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.KhabaKhutti))
                    {
                        qty = 1;
                    }

                    // P1/B15 fix: each member's share was rounded independently
                    // (Round(qty/count, 4)), so shares never summed back to the bill
                    // quantity. All members except the last now receive the rounded
                    // share; the last member absorbs the remaining remainder.
                    decimal providedQuantity;
                    bool isLastMember = member.MemberId == model.members.Last().MemberId;
                    if (isLastMember)
                    {
                        decimal assignedSoFar = 0;
                        foreach (var other in model.members)
                        {
                            if (other.MemberId == member.MemberId) continue;
                            assignedSoFar += Math.Round(qty / memberCount, 4);
                        }
                        providedQuantity = qty - assignedSoFar;
                    }
                    else
                    {
                        providedQuantity = Math.Round(qty / memberCount, 4);
                    }
                    transDto.quantity = providedQuantity;
                    if (dto.tax_amount > 0)
                    {
                        // Same remainder handling for tax so member tax shares sum
                        // back to the bill tax exactly.
                        if (isLastMember)
                        {
                            transDto.tax_amount = Math.Round(dto.tax_amount - Math.Round(dto.tax_amount / memberCount, 2) * (memberCount - 1), 2);
                        }
                        else
                        {
                            transDto.tax_amount = Math.Round(dto.tax_amount / memberCount, 2);
                        }
                    }
                    transDto.amount = Math.Round(providedQuantity * detail.rate, 2);
                    transDtos.Add(transDto);
                }
            }

            dto.addWoodMemberDetails(woodBillMemberDtos);
            dto.addWoodMemberTransactionDetails(transDtos);

            dto.addWoodBillDetails(woodDetailDto);

            return dto;
        }

        private List<WoodBillDetailDto> setWoodBillDetailDto(WoodBillModel model, WoodBillDto dto)
        {
            List<WoodBillDetailDto> woodDetailDto = new List<WoodBillDetailDto>();
            foreach (var item in model.items)
            {
                WoodBillDetailDto detailDto = new WoodBillDetailDto();
                detailDto.wood_details_id = item.wood_details_id;
                detailDto.wood_bill_id = dto.wood_bill_id;
                detailDto.rate = item.rate;
                var woodDetails = _woodDetailRepo.getById(item.wood_details_id);
                detailDto.stock_type_id = woodDetails.stock_type_id;
                var qty = woodDetails.getNetTotal();
                if (woodDetails.balla_balli_category_id == Convert.ToInt32(BallaBalliCategorys.KhabaKhutti))
                {
                    qty = 1;
                }
                detailDto.amount = Math.Round(qty * item.rate, 2);
                woodDetailDto.Add(detailDto);
            }
            return woodDetailDto;
        }

        private void setWoodBillDto(WoodBillModel model, WoodBillDto dto)
        {
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var dateService = DateConverterFactory.getDateConverterService();
            var currentTime = dateFunction.getDateTimeByTimeZone();

            dto.bill_date = dateService.ToAD(model.nep_bill_date).getFormattedDate().Add(currentTime.TimeOfDay);
            dto.amount = model.total_amount;
            dto.remarks = model.remarks;
            dto.user_id = getLoggedInUserId();
            dto.sales_type = (SalesType)Enum.Parse(typeof(SalesType), model.sales_type, true);
            dto.name = model.other_customer;
            dto.address = model.address;
            if (model.tax_percentage > 0)
            {
                dto.tax_amount = Math.Round((model.tax_percentage * dto.amount) / 100, 2);
            }
        }

        [HttpGet]
        [Route("bill")]
        public IActionResult bill(long wood_bill_id)
        {
            try
            {
                ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
                ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

                var dateConverterService = DateConverterFactory.getDateConverterService();
                var billData = _woodBillDetailRepo.getQueryable().Where(a => a.wood_bill_id == wood_bill_id).ToList();

                List<string> name = new List<string>();
                List<string> address = new List<string>();
                List<string> tole_no = new List<string>();

                var bill = _woodBillRepo.getById(wood_bill_id);
                if (bill.sales_type == SalesType.Member)
                {

                    var members = _woodBillMemberRepo.getQueryable().Where(a => a.wood_bill_id == bill.wood_bill_id);
                    foreach (var member in members)
                    {
                        name.Add(member.member.FullName + " (" + member.member.Membership.MembershipCode + ")");
                        address.Add(member.member.Address);
                        tole_no.Add(member.member.Membership.ToleNo);
                    }

                }
                else if (bill.sales_type == SalesType.Antarik)
                {
                    name.Add("Antarik");
                    address.Add("");
                }
                else
                {
                    name.Add(bill.name);
                    address.Add(bill.address);
                }

                WoodBillIndexViewModel vm = getBillViewModelFrom(billData);
                vm.logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;
                vm.members = name;
                vm.address = address;
                vm.tole_no = tole_no;
                vm.nep_bill_date = bill.nep_bill_date;
                vm.remarks = bill.remarks;
                vm.user = _urerRepo.getById(getLoggedInUserId()).full_name;
                vm.amount = bill.amount;
                decimal taxAmt = bill.tax_amount;
                vm.tax_amount = taxAmt;
                vm.wood_bill_id = bill.wood_bill_id;
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

        private WoodBillIndexViewModel getBillViewModelFrom(List<WoodBillDetail> billData)
        {
            WoodBillIndexViewModel VM = new WoodBillIndexViewModel();
            VM.bill_details = new List<BillDetail>();
            foreach (var sale in billData)
            {
                var billDetail = _mapper.Map<BillDetail>(sale);
                VM.bill_details.Add(billDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("report/{stock_type_id}")]
        public IActionResult report(WoodBillReportIndexViewModel vm, long stock_type_id)
        {
            ViewBag.urlId = stock_type_id;
            setReportViewModel(vm, stock_type_id);
            return View(vm);
        }

        [HttpGet]
        [Route("report-print/{stock_type_id}")]
        public IActionResult reportPrint(WoodBillReportIndexViewModel vm, long stock_type_id)
        {
            ViewBag.urlId = stock_type_id;

            setReportViewModel(vm, stock_type_id);

            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            return View(vm);
        }


        private void setReportViewModel(WoodBillReportIndexViewModel vm, long stock_type_id)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var details = _woodBillRepo.getQueryable().Where(a => a.bill_date.Date >= startDate.Date && a.bill_date.Date <= endDate.Date && a.is_cancelled == vm.is_cancelled).ToList();


            foreach (var detail in details)
            {
                foreach (var stockType in detail.wood_bill_detail)
                {
                    if (stockType.woodDetails.stock_type_id == stock_type_id)
                    {
                        var bill = _mapper.Map<WoodBillReportDetails>(detail);
                        var str = "";
                        if (detail.sales_type == SalesType.Member)
                        {
                            var memberIds = _woodBillMemberRepo.getQueryable().Where(a => a.wood_bill_id == detail.wood_bill_id);
                            foreach (var member in memberIds)
                            {
                                str = str + "" + member.member.FullName;
                            }

                            bill.name = str;
                        }
                        else
                        {
                            bill.name = bill.name;
                        }
                        vm.wood_bill_datas.Add(bill);
                    }
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel/{wood_bill_id}")]
        public IActionResult cancel(long wood_bill_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _woodBillService.cancel(wood_bill_id, userId);
                AlertHelper.setMessage(this, "Wood Bill Cancelled Successfully.", messageType.success);
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