using AutoMapper;
using LE.Account.Infrastructure.Repository.Interface;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/furniture")]
    public class FurnitureController : Controller
    {
        private FurnitureRepository _furnitureRepo;
        private FurnitureService _furnitureService;
        private FurnitureCategoryRepository _furnitureCategoryRepo;
        private PaginatedMetaService _paginatedMetaService;
        private LedgerRepository _ledgerRepo;
        private IMapper _mapper;

        public FurnitureController(FurnitureRepository furnitureRepo, FurnitureService furnitureService, FurnitureCategoryRepository furnitureCategoryRepo, PaginatedMetaService paginatedMetaService, LedgerRepository ledgerRepo, IMapper mapper)
        {
            _furnitureRepo = furnitureRepo;
            _furnitureService = furnitureService;
            _furnitureCategoryRepo = furnitureCategoryRepo;
            _paginatedMetaService = paginatedMetaService;
            _ledgerRepo = ledgerRepo;
            _mapper = mapper;
        }

        public IActionResult Index(FurnitureFilter filter)
        {
            var furniture = _furnitureRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                furniture = furniture.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(furniture.Count(), filter.page, filter.number_of_rows);
            furniture = furniture.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var furnitures = furniture.ToList();

            FurnitureIndexViewModel furnitureIndexVM = getViewModelFrom(furnitures);
            return View(furnitureIndexVM);
        }

        private FurnitureIndexViewModel getViewModelFrom(List<Furniture> furnitures)
        {
            FurnitureIndexViewModel VM = new FurnitureIndexViewModel();
            VM.furnitures = new List<FurnitureDetail>();
            foreach (var furniture in furnitures)
            {
                var furnitureDetail = _mapper.Map<FurnitureDetail>(furniture);
                VM.furnitures.Add(furnitureDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            var furniture_categories = _furnitureCategoryRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.furnitureCategories = new SelectList(furniture_categories, "furniture_category_id", "name");
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(FurnitureModel furnitureModel)
        {
            try
            {
                FurnitureDto furnitureDto = new FurnitureDto();
                furnitureDto = getDtoFromModel(furnitureModel);
                AlertHelper.setMessage(this, "Furniture Added Successfully.");
                _furnitureService.insert(furnitureDto);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{furniture_id}")]
        public IActionResult edit(long furniture_id)
        {
            try
            {
                var furniture_categories = _furnitureCategoryRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
                ViewBag.furnitureCategories = new SelectList(furniture_categories, "furniture_category_id", "name");
                var furniture = _furnitureRepo.getQueryable().ToList();
                var furnitureDetails = _furnitureRepo.getById(furniture_id);
                var furnitureModel = getModelFrom(furnitureDetails);
                return View(furnitureModel);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpPost]
        [Route("edit")]
        public IActionResult edit(FurnitureModel furnitureModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FurnitureDto furnitureDto = getDtoFromModel(furnitureModel);
                    _furnitureService.update(furnitureDto);
                    AlertHelper.setMessage(this, "Furniture Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
            return View(furnitureModel);
        }

        [HttpGet]
        [Route("enable/{furniture_id}")]
        public IActionResult enable(long furniture_id)
        {
            try
            {
                _furnitureService.enable(furniture_id);
                AlertHelper.setMessage(this, "Furniture enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{furniture_id}")]
        public IActionResult disable(long furniture_id)
        {
            try
            {
                _furnitureService.disable(furniture_id);
                AlertHelper.setMessage(this, "Furniture disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("delete/{furniture_id}")]
        public IActionResult delete(long furniture_id)
        {
            try
            {
                _furnitureService.delete(furniture_id);
                AlertHelper.setMessage(this, "Furniture deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }


        private FurnitureModel getModelFrom(Furniture furniture)
        {
            return _mapper.Map<FurnitureModel>(furniture);
        }

        private FurnitureDto getDtoFromModel(FurnitureModel furnitureModel)
        {
            return _mapper.Map<FurnitureDto>(furnitureModel);
        }

    }
}