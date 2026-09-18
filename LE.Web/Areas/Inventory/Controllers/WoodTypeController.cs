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
    [Route("inventory/wood-type")]
    public class WoodTypeController : Controller
    {
        private WoodTypeRepository _woodTypeRepo;
        private WoodTypeService _woodTypeService;
        private readonly PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        public WoodTypeController(WoodTypeRepository woodTypeRepos, WoodTypeService stockCategoryPurposeService, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _woodTypeService = stockCategoryPurposeService;
            _woodTypeRepo = woodTypeRepos;
            _mapper = mapper;
            _paginatedMetaService = paginatedMetaService;
        }

        [Route("")]
        [Route("index", Name = "inventory_woodtype_index")]
        public IActionResult Index(WoodTypeFilter filter)
        {
            var woodType = _woodTypeRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                woodType = woodType.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(woodType.Count(), filter.page, filter.number_of_rows);
            var woodTypes = woodType.OrderBy(a => a.wood_type_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();

            WoodTypeIndexViewModel woodTypeVM = getViewModelFrom(woodTypes);
            return View(woodTypeVM);
        }

        private WoodTypeIndexViewModel getViewModelFrom(List<WoodType> woodTypes)
        {
            WoodTypeIndexViewModel VM = new WoodTypeIndexViewModel();
            VM.wood_types = new List<Woods>();
            foreach(var woodType in woodTypes)
            {
                var details = _mapper.Map<Woods>(woodType);
                VM.wood_types.Add(details);
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
        public IActionResult add(WoodTypeModel model)
        {
            try
            {
                var woodTypeDto = _mapper.Map<WoodTypeDto>(model);
                _woodTypeService.save(woodTypeDto);
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
        [Route("edit/{wood_type_id}")]
        public IActionResult edit(long wood_type_id)
        {
            var woodType = _woodTypeRepo.getById(wood_type_id);
            var categoryModel = _mapper.Map<WoodTypeModel>(woodType);
            return View(categoryModel);
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(WoodTypeModel model)
        {
            try
            {
                var woodTypeDto = _mapper.Map<WoodTypeDto>(model);
                _woodTypeService.update(woodTypeDto);
                AlertHelper.setMessage(this, "Wood Type Updated Successfully.", messageType.success);
                return RedirectToAction("index");

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
           
        }

        [HttpGet]
        [Route("delete/{wood_type_id}")]
        public IActionResult delete(long wood_type_id)
        {
            try
            {
                _woodTypeService.delete(wood_type_id);
                AlertHelper.setMessage(this, "Wood Type Deleted Successfully.");
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