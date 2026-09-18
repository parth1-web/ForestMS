using AutoMapper;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Web.Areas.Billing.FilterModel;
using LE.Web.Areas.Billing.Models;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
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
    [Route("billing/service-category")]
    public class ServiceCategoryController : BaseController
    {

        private ServiceCategoryService _serviceCategoryService;
        private ServiceCategoryRepository _serviceCategoryRepo;
        private readonly IMapper _mapper;
        private PaginatedMetaService _paginatedMetaService;

        public ServiceCategoryController(ServiceCategoryService serviceCategoryService, ServiceCategoryRepository serviceCategoryRepo, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _serviceCategoryService = serviceCategoryService;
            _serviceCategoryRepo = serviceCategoryRepo;
            _paginatedMetaService = paginatedMetaService;
            _mapper = mapper;
        }
        [Route("")]
        [Route("index", Name = "billing_servicecategory_index")]
        public IActionResult Index(ServiceCategoryFilter filter)
        {
            var serviceCategory = _serviceCategoryRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                serviceCategory = serviceCategory.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(serviceCategory.Count(), filter.page, filter.number_of_rows);
            var serviceCategories = serviceCategory.OrderBy(a => a.category_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();

            ServiceCategoryIndexViewModel serviceCategoryIndexVM = getViewModelFrom(serviceCategories);
            return View(serviceCategoryIndexVM);
        }

        private ServiceCategoryIndexViewModel getViewModelFrom(List<ServiceCategory> serviceCategories)
        {
            ServiceCategoryIndexViewModel VM = new ServiceCategoryIndexViewModel();
            VM.service_categories = new List<ServiceCategoryDetail>();
            foreach (var category in serviceCategories)
            {
                var categoryDetail = _mapper.Map<ServiceCategoryDetail>(category);
                VM.service_categories.Add(categoryDetail);
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
        public IActionResult add(ServiceCategoryModel serviceCategoryModel)
        {
            try
            {
                ServiceCategoryDto custCatDto = new ServiceCategoryDto();
                custCatDto = getDtoFromModel(serviceCategoryModel);
                _serviceCategoryService.insert(custCatDto);
                AlertHelper.setMessage(this, "Service Category Added Successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpGet]
        [Route("delete/{category_id}")]
        public IActionResult delete(long category_id)
        {
            try
            {
                _serviceCategoryService.delete(category_id);
                AlertHelper.setMessage(this, "Service Category deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{category_id}")]
        public IActionResult edit(long category_id)
        {
            try
            {
                var serviceCategories = _serviceCategoryRepo.getQueryable().ToList();

                var categoryDetails = _serviceCategoryRepo.getById(category_id);
                var serviceCategoryModel = getModelFrom(categoryDetails);
                return View(serviceCategoryModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpPost]
        [Route("edit")]
        public IActionResult edit(ServiceCategoryModel serviceCategoryModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ServiceCategoryDto serviceCategoryDto = getDtoFromModel(serviceCategoryModel);
                    _serviceCategoryService.update(serviceCategoryDto);
                    AlertHelper.setMessage(this, "Service Category Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
                return View(serviceCategoryModel);

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }

        }

        [HttpGet]
        [Route("enable/{category_id}")]
        public IActionResult enable(long category_id)
        {
            try
            {
                _serviceCategoryService.enable(category_id);
                AlertHelper.setMessage(this, "Service Category enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{category_id}")]
        public IActionResult disable(long category_id)
        {
            try
            {
                _serviceCategoryService.disable(category_id);
                AlertHelper.setMessage(this, "Service Category disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        private object getModelFrom(ServiceCategory serviceCategoryDetails)
        {
            return _mapper.Map<ServiceCategoryModel>(serviceCategoryDetails);
        }

        private ServiceCategoryDto getDtoFromModel(ServiceCategoryModel serviceCategoryModel)
        {
            return _mapper.Map<ServiceCategoryDto>(serviceCategoryModel);
        }

    }
}