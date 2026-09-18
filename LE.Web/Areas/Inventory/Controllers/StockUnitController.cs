using AutoMapper;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using LE.Web.Areas.Inventory.FilterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using LE.Web.LEPagination;
using LE.Inventory.Infrastructure.Repository.Interface;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Area("inventory")]
    [Route("inventory/stock-unit")]
    public class StockUnitController : BaseController
    {
        private StockUnitAssembler _stockUnitAssembler;
        private StockUnitService _stockUnitService;
        private readonly StockUnitRepository _stockUnitRepo;
        private PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        public StockUnitController(StockUnitAssembler stockUnitMaker, StockUnitService stockUnitService, IMapper mapper, StockUnitRepository stockUnitRepo, PaginatedMetaService paginatedMetaService)
        {
            _stockUnitAssembler = stockUnitMaker;
            _stockUnitService = stockUnitService;
            _mapper = mapper;
            _stockUnitRepo = stockUnitRepo;
            _paginatedMetaService = paginatedMetaService;
        }

        [Route("")]
        [Route("index", Name = "inventory_stockunit_index")]
        public IActionResult Index(StockUnitFilter filter)
        {
            var stockUnit = _stockUnitRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                stockUnit = stockUnit.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(stockUnit.Count(), filter.page, filter.number_of_rows);
            var stockUnits = stockUnit.OrderByDescending(a => a.stock_unit_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows).ToList();

            StockUnitIndexViewModel stockUnitIndexVM = getViewModelFrom(stockUnits);
            return View(stockUnitIndexVM);
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(StockUnitModel stock_unit_model)
        {
            try
            {
                StockUnitDto stock_unit_dto = getStockUnitDtoFromModel(stock_unit_model);
                _stockUnitService.save(stock_unit_dto);
                AlertHelper.setMessage(this, "Stock Unit added successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("delete/{stock_unit_id}")]
        public IActionResult delete(long stock_unit_id)
        {
            try
            {
                _stockUnitService.delete(stock_unit_id);
                AlertHelper.setMessage(this, "Stock Unit deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{stock_unit_id}")]
        public IActionResult edit(long stock_unit_id)
        {
            try
            {
                var stockUnitDetails = _stockUnitRepo.getById(stock_unit_id);
                var stockUnitModel = getStockUnitModelFromStockUnit(stockUnitDetails);
                return View(stockUnitModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpPost]
        [Route("edit")]
        public IActionResult edit(StockUnitModel stockUnitModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StockUnitDto stockUnitDto = getStockUnitDtoFromModel(stockUnitModel);
                    _stockUnitService.update(stockUnitDto);
                    AlertHelper.setMessage(this, "Stock Unit Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
            return View(stockUnitModel);
        }

        [HttpGet]
        [Route("enable/{stock_unit_id}")]
        public IActionResult enable(long stock_unit_id)
        {
            try
            {
                _stockUnitService.enable(stock_unit_id);
                AlertHelper.setMessage(this, "Stock Unit enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{stock_unit_id}")]
        public IActionResult disable(long stock_unit_id)
        {
            try
            {
                _stockUnitService.disable(stock_unit_id);
                AlertHelper.setMessage(this, "Stock Unit disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        private object getStockUnitModelFromStockUnit(StockUnit unitDetails)
        {
            return _mapper.Map<StockUnitModel>(unitDetails);
        }


        private StockUnitIndexViewModel getViewModelFrom(List<StockUnit> stockUnits)
        {
            StockUnitIndexViewModel VM = new StockUnitIndexViewModel();
            VM.stock_units = new List<StockUnitDetail>();
            foreach (var unit in stockUnits)
            {
                var stockUnitDetail = _mapper.Map<StockUnitDetail>(unit);
                VM.stock_units.Add(stockUnitDetail);
            }
            return VM;
        }

        private StockUnitDto getStockUnitDtoFromModel(StockUnitModel stock_unit_model)
        {
            var stockUnitDto = _mapper.Map<StockUnitDto>(stock_unit_model);
            return stockUnitDto;
        }
    }
}