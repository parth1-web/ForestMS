using AutoMapper;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Web.Areas.Inventory.FilterModel;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("inventory")]
    [Route("inventory/category-purpose")]
    public class CategoryPurposeController : Controller
    {
        private StockCategoryPurposeRepository _categoryPurposeRepo;
        private StockCategoryPurposeService _catgoryPurposeService;
        private readonly PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        public CategoryPurposeController(StockCategoryPurposeRepository stockCategoryPurposeRepo, StockCategoryPurposeService stockCategoryPurposeService, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _catgoryPurposeService = stockCategoryPurposeService;
            _categoryPurposeRepo = stockCategoryPurposeRepo;
            _mapper = mapper;
            _paginatedMetaService = paginatedMetaService;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index(CategoryPurposeFilter filter)
        {
            var category = _categoryPurposeRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                category = category.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(category.Count(), filter.page, filter.number_of_rows);
            category = category.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var categories = category.ToList();

            CategoryPurposeIndexViewModel categoryVM = getViewModelFrom(categories);
            return View(categoryVM);
        }

        private CategoryPurposeIndexViewModel getViewModelFrom(List<StockCategoryPurpose> categories)
        {
            CategoryPurposeIndexViewModel VM = new CategoryPurposeIndexViewModel();
            VM.category_purposes = new List<CategoryPurposeDetails>();
            foreach(var category in categories)
            {
                var details = _mapper.Map<CategoryPurposeDetails>(category);
                VM.category_purposes.Add(details);
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
        public IActionResult add(CategoryPurposeModel model)
        {
            try
            {
                var categoryPurposeDto = _mapper.Map<StockCategoryPurposeDto>(model);
                _catgoryPurposeService.save(categoryPurposeDto);
                AlertHelper.setMessage(this, "Category Purpose added Successfully.");
                return RedirectToAction("index");

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{stock_category_purpose_id}")]
        public IActionResult edit(long stock_category_purpose_id)
        {
            var category = _categoryPurposeRepo.getById(stock_category_purpose_id);
            var categoryModel = _mapper.Map<CategoryPurposeModel>(category);
            return View(categoryModel);
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(CategoryPurposeModel model)
        {
            try
            {
                var categoryPurposeDto = _mapper.Map<StockCategoryPurposeDto>(model);
                _catgoryPurposeService.update(categoryPurposeDto);
                AlertHelper.setMessage(this, "Category Purpose Updated Successfully.", messageType.success);
                return RedirectToAction("index");

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
           
        }

        [HttpGet]
        [Route("delete/{stock_category_purpose_id}")]
        public IActionResult delete(long stock_category_purpose_id)
        {
            try
            {
                _catgoryPurposeService.delete(stock_category_purpose_id);
                AlertHelper.setMessage(this, "Category Purpose Deleted Successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

    }
}