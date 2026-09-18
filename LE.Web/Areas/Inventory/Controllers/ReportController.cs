using AutoMapper;
using LE.Common.Enums;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Controllers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("inventory")]
    [Route("inventory/reports")]
    public class ReportController : BaseController
    {
        private readonly WoodDetailsRepository _woodDetailsRepo;
        private readonly PilingRepository _pilingRepo;
        private readonly StockCategoryPurposeRepository _stockCategoryPurposeRepo;
        private readonly PaginatedMetaService _paginatedMetaService;
        private readonly OrganizationSetupRepository _organizationSetupRepo;
        private IMapper _mapper;

        public ReportController(WoodDetailsRepository woodDetailsRepo, PilingRepository pilingRepo, StockCategoryPurposeRepository stockCategoryPurposeRepo, PaginatedMetaService paginatedMetaService, OrganizationSetupRepository organizationSetupRepo, IMapper mapper)
        {
            _woodDetailsRepo = woodDetailsRepo;
            _pilingRepo = pilingRepo;
            _stockCategoryPurposeRepo = stockCategoryPurposeRepo;
            _paginatedMetaService = paginatedMetaService;
            _organizationSetupRepo = organizationSetupRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("sales-report", Name = "inventory_reports_salesreport")]
        public IActionResult salesReport(WoodDetailsIndexViewModel vm)
        {
            var categoryPurpose = _stockCategoryPurposeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.categoryPurposes = new SelectList(categoryPurpose, "stock_category_purpose_id", "name");

            var piling = _pilingRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.piling = new SelectList(piling, "piling_id", "title");

            var woodDetails = getSalesReport(vm);

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(woodDetails.Count(), vm.page, vm.number_of_rows);
            var res = woodDetails.OrderByDescending(a => a.created_date).Skip(vm.number_of_rows * (vm.page - 1)).Take(vm.number_of_rows).ToList();
            vm = getViewModelFrom(res);
            return View(vm);
        }

        [HttpGet]
        [Route("sales-report-print")]
        public IActionResult salesReportPrint(WoodDetailsIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepo.getByKey(OrganizationSetup.Address.ToString()).value;

            var woodDetails = getSalesReport(vm);

            var res = woodDetails.OrderByDescending(a => a.created_date).ToList();
            vm = getViewModelFrom(res);
            return View(vm);
        }

        [HttpGet]
        [Route("sales-report-pdf")]
        public IActionResult salesReportPdf(WoodDetailsIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepo.getByKey(OrganizationSetup.Address.ToString()).value;

            var woodDetails = getSalesReport(vm);

            var res = woodDetails.OrderByDescending(a => a.created_date).ToList();
            vm = getViewModelFrom(res);
            return new ViewAsPdf("salesReportPrint", vm);
        }

        private WoodDetailsIndexViewModel getViewModelFrom(List<WoodDetails> woodDetails)
        {
            WoodDetailsIndexViewModel VM = new WoodDetailsIndexViewModel();
            VM.wood_details = new List<WoodsItemDetail>();
            foreach (var details in woodDetails)
            {
                var woodDetail = _mapper.Map<WoodsItemDetail>(details);
                VM.wood_details.Add(woodDetail);
            }
            return VM;
        }

        private IQueryable<WoodDetails> getSalesReport(WoodDetailsIndexViewModel vm)
        {

            var woodDetails = _woodDetailsRepo.getQueryable().Where(a => a.is_sold);
            if (!string.IsNullOrEmpty(vm.search_by_golia_number))
            {
                woodDetails = woodDetails.Where(a => a.goliya_number == vm.search_by_golia_number || a.tuna_no == vm.search_by_golia_number);
            }
            if (vm.stock_type_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.stock_type_id == vm.stock_type_id);
            }

            if (vm.stock_category_purpose_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.stock_category_purpose_id == vm.stock_category_purpose_id);
            }

            if (vm.piling_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.piling_id == vm.piling_id);
            }

            return woodDetails;
        }

        [HttpGet]
        [Route("piling-wise-report", Name = "inventory_reports_pilingwisereport")]
        public IActionResult pilingWiseReport(WoodDetailsIndexViewModel vm)
        {
            var piling = _pilingRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.piling = new SelectList(piling, "piling_id", "title");

            var woodDetails = _woodDetailsRepo.getQueryable();

            if (vm.piling_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.piling_id == vm.piling_id);
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(woodDetails.Count(), vm.page, vm.number_of_rows);
            var res = woodDetails.OrderByDescending(a => a.created_date).Skip(vm.number_of_rows * (vm.page - 1)).Take(vm.number_of_rows).ToList();
            var VM = getViewModelFrom(res);
            VM.piling_title = vm.piling_id > 0 ? _pilingRepo.getById(vm.piling_id).title : "N/A";

            return View(VM);
        }

        [HttpGet]
        [Route("piling-wise-report-print")]
        public IActionResult pilingWiseReportPrint(WoodDetailsIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepo.getByKey(OrganizationSetup.Address.ToString()).value;

            var woodDetails = _woodDetailsRepo.getQueryable();

            if (vm.piling_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.piling_id == vm.piling_id);
            }

            var res = woodDetails.OrderByDescending(a => a.created_date).ToList();
            var VM = getViewModelFrom(res);
            VM.piling_title = vm.piling_id > 0 ? _pilingRepo.getById(vm.piling_id).title : "N/A";
            return View(VM);
        }

        [HttpGet]
        [Route("piling-wise-report-pdf")]
        public IActionResult pilingWiseReportPdf(WoodDetailsIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepo.getByKey(OrganizationSetup.Address.ToString()).value;

            var woodDetails = _woodDetailsRepo.getQueryable();

            if (vm.piling_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.piling_id == vm.piling_id);
            }

            var res = woodDetails.OrderByDescending(a => a.created_date).ToList();
            var VM = getViewModelFrom(res);
            VM.piling_title = vm.piling_id > 0 ? _pilingRepo.getById(vm.piling_id).title : "N/A";
            return new ViewAsPdf("pilingWiseReportPrint", VM);
        }
    }
}
