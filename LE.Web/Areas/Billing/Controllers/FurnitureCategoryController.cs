using AutoMapper;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Web.Areas.Billing.FilterModel;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/furniture-category")]
    public class FurnitureCategoryController : Controller
    {

        private FurnitureCategoryService _furnitureCategoryService;
        private FurnitureCategoryRepository _furnitureCategoryRepo;
        private readonly IMapper _mapper;
        private PaginatedMetaService _paginatedMetaService;

        public FurnitureCategoryController(FurnitureCategoryService furnitureCategoryService, FurnitureCategoryRepository furnitureCategoryRepo, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _furnitureCategoryService = furnitureCategoryService;
            _furnitureCategoryRepo = furnitureCategoryRepo;
            _mapper = mapper;
            _paginatedMetaService = paginatedMetaService;
        }

        public IActionResult Index(FurnitureCategoryFilter filter)
        {
            var furnitureCategory = _furnitureCategoryRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                furnitureCategory = furnitureCategory.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(furnitureCategory.Count(), filter.page, filter.number_of_rows);
            furnitureCategory = furnitureCategory.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var furnitureCategories = furnitureCategory.ToList();

            FurnitureCategoryIndexViewModel furnitureCategoryIndexVM = getViewModelFrom(furnitureCategories);
            return View(furnitureCategoryIndexVM);
        }

        private FurnitureCategoryIndexViewModel getViewModelFrom(List<FurnitureCategory> furnitureCategories)
        {
            FurnitureCategoryIndexViewModel VM = new FurnitureCategoryIndexViewModel();
            VM.furniture_categories = new List<FurnitureCategoryDetail>();
            foreach (var category in furnitureCategories)
            {
                var categoryDetail = _mapper.Map<FurnitureCategoryDetail>(category);
                VM.furniture_categories.Add(categoryDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(FurnitureCategoryModel furnitureCategoryModel)
        {
            try
            {
                FurnitureCategoryDto furnitureCatDto = new FurnitureCategoryDto();
                furnitureCatDto = getDtoFromModel(furnitureCategoryModel);
                _furnitureCategoryService.insert(furnitureCatDto);
                AlertHelper.setMessage(this, "Furniture Category Added Successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpGet]
        [Route("delete/{furniture_category_id}")]
        public IActionResult delete(long furniture_category_id)
        {
            try
            {
                _furnitureCategoryService.delete(furniture_category_id);
                AlertHelper.setMessage(this, "Furniture Category deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{furniture_category_id}")]
        public IActionResult edit(long furniture_category_id)
        {
            try
            {
                var furnitureCategories = _furnitureCategoryRepo.getQueryable().ToList();

                var categoryDetails = _furnitureCategoryRepo.getById(furniture_category_id);
                var furnitureCategoryModel = getModelFrom(categoryDetails);
                return View(furnitureCategoryModel);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpPost]
        [Route("edit")]
        public IActionResult edit(FurnitureCategoryModel furnitureCategoryModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FurnitureCategoryDto furnitureCategoryDto = getDtoFromModel(furnitureCategoryModel);
                    _furnitureCategoryService.update(furnitureCategoryDto);
                    AlertHelper.setMessage(this, "Furniture Category Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
                return View(furnitureCategoryModel);

            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }

        }

        [HttpGet]
        [Route("enable/{furniture_category_id}")]
        public IActionResult enable(long furniture_category_id)
        {
            try
            {
                _furnitureCategoryService.enable(furniture_category_id);
                AlertHelper.setMessage(this, "Furniture Category enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{furniture_category_id}")]
        public IActionResult disable(long furniture_category_id)
        {
            try
            {
                _furnitureCategoryService.disable(furniture_category_id);
                AlertHelper.setMessage(this, "Furniture Category disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        private object getModelFrom(FurnitureCategory furnitureCategoryDetails)
        {
            return _mapper.Map<FurnitureCategoryModel>(furnitureCategoryDetails);
        }

        private FurnitureCategoryDto getDtoFromModel(FurnitureCategoryModel furnitureCategoryModel)
        {
            return _mapper.Map<FurnitureCategoryDto>(furnitureCategoryModel);
        }

    }
}