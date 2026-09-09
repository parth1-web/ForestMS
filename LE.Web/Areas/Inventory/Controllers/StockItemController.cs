using AutoMapper;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Assemblers.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Web.Areas.Inventory.FilterModel;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Area("inventory")]
    [Route("inventory/stock-item")]
    public class StockItemController : BaseController
    {
        private StockItemAssembler _stockItemAssembler;
        private StockItemService _stockItemService;
        private readonly StockItemRepository _stockItemRepo;
        private readonly WoodTypeRepository _woodTypeRepo;
        private readonly StockUnitRepository _stockUnitRepo;
        private readonly StockItemAvailabilityRepository _stockItemAvailRepo;
        private PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        public StockItemController(StockItemAssembler stockItemMaker, StockItemService stockItemService, IMapper mapper, StockItemRepository stockItemRepo, PaginatedMetaService paginatedMetaService , WoodTypeRepository woodTypeRepo, StockUnitRepository stockUnitRepo, StockItemAvailabilityRepository stockItemAvailRepo)
        {
            _stockItemAssembler = stockItemMaker;
            _stockItemService = stockItemService;
            _mapper = mapper;
            _stockItemRepo = stockItemRepo;
            _stockUnitRepo = stockUnitRepo;
            _woodTypeRepo = woodTypeRepo;
            _paginatedMetaService = paginatedMetaService;
            _stockItemAvailRepo = stockItemAvailRepo;
        }

        public IActionResult Index(StockItemFilter filter)
        {
            var stockItem = _stockItemRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                stockItem = stockItem.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(stockItem.Count(), filter.page, filter.number_of_rows);
            stockItem = stockItem.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var stockItems = stockItem.OrderByDescending(a=>a.stock_item_id).ToList();

            StockItemIndexViewModel stockItemIndexVM = getViewModelFrom(stockItems);
            return View(stockItemIndexVM);
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            var stock_units = _stockUnitRepo.getQueryable().Where(a=>a.is_enabled==true).ToList();
            var wood_types = _woodTypeRepo.getQueryable().Where(a=>a.is_enabled==true).ToList();
            ViewBag.stockUnit = new SelectList(stock_units, "stock_unit_id", "name");
            ViewBag.woodType =new SelectList(wood_types, "wood_type_id", "name");
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(StockItemModel stock_item_model)
        {
            try
            {
                StockItemDto stock_item_dto = getStockItemDtoFromModel(stock_item_model);
              
                _stockItemService.save(stock_item_dto);
                AlertHelper.setMessage(this, "Stock Item added successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("delete/{stock_item_id}")]
        public IActionResult delete(long stock_item_id)
        {
            try
            {
                _stockItemService.delete(stock_item_id);
                AlertHelper.setMessage(this, "Stock Item deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{stock_item_id}")]
        public IActionResult edit(long stock_item_id)
        {
            try
            {
                var stockItemDetails = _stockItemRepo.getById(stock_item_id);
                var stockItemModel = getStockItemModelFromStockItem(stockItemDetails);

                var stock_units = _stockUnitRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
                var wood_types = _woodTypeRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
                ViewBag.stockUnit = new SelectList(stock_units, "stock_unit_id", "name");
                ViewBag.woodType = new SelectList(wood_types, "wood_type_id", "name");
                return View(stockItemModel);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(StockItemModel stockItemModel,IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StockItemDto stockItemDto = getStockItemDtoFromModel(stockItemModel);
                    _stockItemService.update(stockItemDto);
                    AlertHelper.setMessage(this, "Stock Item Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
            return View(stockItemModel);
        }

        [HttpGet]
        [Route("enable/{stock_item_id}")]
        public IActionResult enable(long stock_item_id)
        {
            try
            {
                _stockItemService.enable(stock_item_id);
                AlertHelper.setMessage(this, "Stock Item enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{stock_item_id}")]
        public IActionResult disable(long stock_item_id)
        {
            try
            {
                _stockItemService.disable(stock_item_id);
                AlertHelper.setMessage(this, "Stock Item disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

      
        private object getStockItemModelFromStockItem(StockItem itemDetails)
        {
            return _mapper.Map<StockItemModel>(itemDetails);
        }


        private StockItemIndexViewModel getViewModelFrom(List<StockItem> stockItems)
        {
            StockItemIndexViewModel VM = new StockItemIndexViewModel();
            VM.stock_items = new List<StockItemDetail>();
            foreach (var item in stockItems)
            {
                var stockItemDetail = _mapper.Map<StockItemDetail>(item);
                VM.stock_items.Add(stockItemDetail);
            }
            return VM;
        }

        private StockItemDto getStockItemDtoFromModel(StockItemModel stock_item_model)
        {
            var stockItemDto = _mapper.Map<StockItemDto>(stock_item_model);
            return stockItemDto;
        }

    }
}