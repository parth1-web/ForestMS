using AutoMapper;
using LE.Common.Enums;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Inventory.FilterModel;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("inventory")]
    [Route("inventory/wood-details")]
    public class WoodDetailsController : BaseController
    {
        private readonly WoodDetailsRepository _woodDetailsRepo;
        private readonly WoodDetailsService _woodDetailsService;
        private readonly WoodTypeRepository _woodTypeRepo;
        private WoodDetailsAssembler _woodDetailsAssembler;
        private readonly StockCategoryPurposeRepository _stockCategoryPurposeRepo;
        private readonly PilingRepository _pilingRepo;
        private readonly PaginatedMetaService _paginatedMetaService;
        private OrganizationSetupRepository _organizationSetupRepository;
        private IMapper _mapper;

        public WoodDetailsController(WoodDetailsRepository woodDetailsRepo, WoodDetailsService woodDetailsService, WoodTypeRepository woodTypeRepo, WoodDetailsAssembler woodDetailsAssembler, StockCategoryPurposeRepository stockCategoryPurposeRepo, PilingRepository pilingRepo, PaginatedMetaService paginatedMetaService, OrganizationSetupRepository organizationSetupRepository, IMapper mapper)
        {
            _woodDetailsRepo = woodDetailsRepo;
            _woodDetailsService = woodDetailsService;
            _woodTypeRepo = woodTypeRepo;
            _woodDetailsAssembler = woodDetailsAssembler;
            _stockCategoryPurposeRepo = stockCategoryPurposeRepo;
            _pilingRepo = pilingRepo;
            _paginatedMetaService = paginatedMetaService;
            _organizationSetupRepository = organizationSetupRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult Index(WoodDetailsFilter filter)
        {
            var woodDetail = _woodDetailsRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                woodDetail = woodDetail.Where(a => a.goliya_number.Contains(filter.name));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(woodDetail.Count(), filter.page, filter.number_of_rows);
            woodDetail = woodDetail.OrderByDescending(a => a.wood_details_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var woodDetails = woodDetail.ToList();

            WoodDetailsIndexViewModel woodDetailsIndexVM = getViewModelFrom(woodDetails);
            return View(woodDetailsIndexVM);
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

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            var woodType = _woodTypeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.woodTypes = new SelectList(woodType, "wood_type_id", "name");

            var caetegoryPurpose = _stockCategoryPurposeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.categoryPurposes = new SelectList(caetegoryPurpose, "stock_category_purpose_id", "name");

            var pilings = _pilingRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.pilings = new SelectList(pilings, "piling_id", "title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("new")]
        public IActionResult add([FromBody] WoodDetailsModel woodDetailModel)
        {
            try
            {
                WoodDetailsDto woodDetailDto = setDtoFromModel(woodDetailModel);
                _woodDetailsService.save(woodDetailDto);
                return Json(new { success = true, responseText = "Wood details added successfully" }); ;
            }
            catch (Exception ex)
            {
                return Json(ExceptionMessageHelper.buildErrorObject(ex));
            }
        }

        private WoodDetailsDto setDtoFromModel(WoodDetailsModel model)
        {
            WoodDetailsDto dto = new WoodDetailsDto();
            setWoodDetailsDto(model, dto);
            setDamagedDetailsDto(model, dto);
            return dto;
        }

        private void setDamagedDetailsDto(WoodDetailsModel model, WoodDetailsDto dto)
        {
            List<DamagedWoodDetailDto> damagedWoodDetails = new List<DamagedWoodDetailDto>();
            foreach (var damaged in model.damagedWoodDetailModels)
            {
                DamagedWoodDetailDto damagedDto = new DamagedWoodDetailDto();
                if (damaged.damaged_first_size > 0)
                {
                    damagedDto.damaged_wood_details_id = damaged.damaged_wood_details_id;
                    damagedDto.damaged_feet_size = damaged.damaged_feet_size;
                    damagedDto.damaged_first_size = damaged.damaged_first_size;
                    damagedDto.damaged_second_size = damaged.damaged_second_size;
                    damagedDto.damaged_third_size = damaged.damaged_third_size;
                    damagedDto.damaged_fourth_size = damaged.damaged_fourth_size;
                    damagedDto.damaged_fifth_size = damaged.damaged_fifth_size;
                    damagedWoodDetails.Add(damagedDto);
                }
            }
            dto.DamagedWoodDetailDtos = damagedWoodDetails;
        }

        private static void setWoodDetailsDto(WoodDetailsModel model, WoodDetailsDto dto)
        {
            dto.wood_details_id = model.wood_details_id;
            dto.balla_balli_category_id = model.balla_balli_category_id;
            dto.circle_size = model.circle_size;
            dto.wood_type_id = model.wood_type_id;
            dto.stock_type_id = model.stock_type_id;
            dto.stock_category_purpose_id = model.stock_category_purpose_id;
            dto.piling_id = model.piling_id;
            dto.length = model.length;
            dto.grade = model.grade;
            dto.year = model.year;
            dto.goliya_number = model.goliya_number;
            dto.tuna_no = model.tuna_no;
        }

        [HttpGet]
        [Route("delete/{wood_detail_id}")]
        public IActionResult delete(long wood_detail_id)
        {
            try
            {
                _woodDetailsService.delete(wood_detail_id);
                AlertHelper.setMessage(this, "Wood Details Deleted Successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{wood_detail_id}")]
        public IActionResult edit(long wood_detail_id)
        {
            var woodType = _woodTypeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.woodTypes = new SelectList(woodType, "wood_type_id", "name");

            var caetegoryPurpose = _stockCategoryPurposeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.categoryPurposes = new SelectList(caetegoryPurpose, "stock_category_purpose_id", "name");

            var pilings = _pilingRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.pilings = new SelectList(pilings, "piling_id", "title");

            WoodDetailsModel model = new WoodDetailsModel();
            var woodDetails = _woodDetailsRepo.getById(wood_detail_id);
            model = getModelFromEntity(woodDetails);
            return View(model);
        }

        private WoodDetailsModel getModelFromEntity(WoodDetails woodDetails)
        {
            WoodDetailsModel model = new WoodDetailsModel();
            model.balla_balli_category_id = woodDetails.balla_balli_category_id;
            model.circle_size = woodDetails.circle_size;
            model.goliya_number = woodDetails.goliya_number;
            model.tuna_no = woodDetails.tuna_no;
            model.grade = woodDetails.grade;
            model.length = woodDetails.length;
            model.stock_category_purpose_id = woodDetails.stock_category_purpose_id;
            model.piling_id = woodDetails.piling_id;
            model.stock_type_id = woodDetails.stock_type_id;
            model.wood_details_id = woodDetails.wood_details_id;
            model.wood_type_id = woodDetails.wood_type_id;
            model.year = woodDetails.year;
            model.damagedWoodDetailModels = getModelofDamaged(woodDetails);
            model.wood_details_id = woodDetails.wood_details_id;
            return model;
        }

        private List<DamagedWoodDetailModel> getModelofDamaged(WoodDetails woodDetails)
        {
            List<DamagedWoodDetailModel> damagedWoodDetailModels = new List<DamagedWoodDetailModel>();
            foreach (var damage in woodDetails.DamagedWoodDetails)
            {
                DamagedWoodDetailModel model = new DamagedWoodDetailModel();
                model.damaged_feet_size = damage.damaged_feet_size;
                model.damaged_fifth_size = damage.damaged_fifth_size;
                model.damaged_first_size = damage.damaged_first_size;
                model.damaged_fourth_size = damage.damaged_fourth_size;
                model.damaged_second_size = damage.damaged_second_size;
                model.damaged_third_size = damage.damaged_third_size;
                model.dividor_value = damage.dividor_value;
                model.damaged_wood_details_id = damage.damaged_wood_details_id;
                damagedWoodDetailModels.Add(model);
            }
            return damagedWoodDetailModels;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit")]
        public IActionResult edit([FromBody] WoodDetailsModel model)
        {
            try
            {
                WoodDetailsDto dto = new WoodDetailsDto();
                dto = setDtoFromModel(model);
                _woodDetailsService.update(dto);
                AlertHelper.setMessage(this, "Wood Details Updated successfully.", messageType.success);
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(ExceptionMessageHelper.buildErrorObject(ex));
            }
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(WoodDetailsIndexViewModel woodDetailsIndexVM)
        {
            var categoryPurpose = _stockCategoryPurposeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.categoryPurposes = new SelectList(categoryPurpose, "stock_category_purpose_id", "name");

            var piling = _pilingRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.piling = new SelectList(piling, "piling_id", "title");

            List<WoodDetails> woodDetails = getWoodDetails(woodDetailsIndexVM);

            woodDetailsIndexVM = getViewModelFrom(woodDetails);
            return View(woodDetailsIndexVM);
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(WoodDetailsIndexViewModel woodDetailsIndexVM)
        {
            ViewBag.organizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            List<WoodDetails> woodDetails = getWoodDetails(woodDetailsIndexVM);
            woodDetailsIndexVM = getViewModelFrom(woodDetails);
            return View(woodDetailsIndexVM);
        }

        [HttpGet]
        [Route("report-pdf")]
        public ViewAsPdf reportPDF(WoodDetailsIndexViewModel woodDetailsIndexVM)
        {
            ViewBag.organizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            List<WoodDetails> woodDetails = getWoodDetails(woodDetailsIndexVM);
            woodDetailsIndexVM = getViewModelFrom(woodDetails);
            return new ViewAsPdf("reportPrint", woodDetailsIndexVM);
        }

        private List<WoodDetails> getWoodDetails(WoodDetailsIndexViewModel woodDetailsIndexVM)
        {
            var woodDetails = _woodDetailsRepo.getQueryable();

            if (woodDetailsIndexVM.stock_type_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.stock_type_id == woodDetailsIndexVM.stock_type_id);
            }

            if (woodDetailsIndexVM.stock_category_purpose_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.stock_category_purpose_id == woodDetailsIndexVM.stock_category_purpose_id);
            }

            if (woodDetailsIndexVM.piling_id > 0)
            {
                woodDetails = woodDetails.Where(a => a.piling_id == woodDetailsIndexVM.piling_id);
            }

            if (woodDetailsIndexVM.sold_type == "sold")
            {
                woodDetails = woodDetails.Where(a => a.is_sold == true);
            }

            if (woodDetailsIndexVM.sold_type == "unsold")
            {
                woodDetails = woodDetails.Where(a => a.is_sold == false);
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(woodDetails.Count(), woodDetailsIndexVM.page, woodDetailsIndexVM.number_of_rows);
            woodDetails = woodDetails.Skip(woodDetailsIndexVM.number_of_rows * (woodDetailsIndexVM.page - 1)).Take(woodDetailsIndexVM.number_of_rows);
            return woodDetails.OrderByDescending(a => a.wood_details_id).ToList();
        }
    }
}