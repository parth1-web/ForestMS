using AutoMapper;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Web.Areas.Inventory.FilterModel;
using LE.Web.Areas.Inventory.Models;
using LE.Web.Areas.Inventory.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using LE.Web.LEPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("inventory")]
    [Route("inventory/daura")]
    public class PurchaseController : BaseController
    {
        private StockItemRepository _stockItemRepo;
        private PurchaseRepository _purchaseRepo;
        private PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        private PurchaseService _purchaseService;
        public PurchaseController(PaginatedMetaService paginatedMetaService, PurchaseRepository purchaseRepo, StockItemRepository stockItemRepo, PurchaseService purchaseService, IMapper mapper, TransactionDetailRepository transactionDetailRepo)
        {
            _stockItemRepo = stockItemRepo;
            _purchaseService = purchaseService;
            _purchaseRepo = purchaseRepo;
            _paginatedMetaService = paginatedMetaService;
            _mapper = mapper;
        }

        [Route("")]
        [Route("index", Name = "inventory_purchase_index")]
        public IActionResult Index(PurchaseFilter filter)
        {
            var purchase = _purchaseRepo.getQueryable()
                .Include(a => a.stock_items)
                .Where(a => a.is_deleted == false);
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                purchase = purchase.Where(a => a.stock_items.name.Contains(filter.name));
            }
            // P1/B18 fix: the is_deleted filter was applied after Skip/Take, so deleted
            // rows consumed page slots (short pages, wrong counts). Filtering now happens
            // before pagination.
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(purchase
                .Count(), filter.page, filter.number_of_rows);
            purchase = purchase.OrderByDescending(a => a.purchase_date).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var purchases = purchase.ToList();

            PurchaseIndexViewModel purchaseIndexViewModel = getViewModelFrom(purchases);
            return View(purchaseIndexViewModel);
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            PurchaseModel purchaseModel = new PurchaseModel();
            var item = _stockItemRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.items = new SelectList(item, "stock_item_id", "name");
            return View(purchaseModel);
        }



        [HttpPost]
        [Route("new")]
        public IActionResult add(PurchaseModel purchaseModel)
        {
            try
            {
                PurchaseDto dto = new PurchaseDto();

                dto = _mapper.Map<PurchaseDto>(purchaseModel);
                // Attribution is taken from the session, not the posted form
                // (the previous assignment before Map was dead code that the
                // mapper overwrote with the client-supplied user id).
                dto.user_id = getLoggedInAuthenticationId();
                _purchaseService.makePurchase(dto);
                AlertHelper.setMessage(this, "Stock added successfully.");
                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                ExceptionMessageHelper.setMessage(this, e, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{purchase_id}")]
        public IActionResult delete(long purchase_id)
        {
            try
            {
                _purchaseService.delete(purchase_id);
                AlertHelper.setMessage(this, "Purchased stock deleted successfully.", messageType.success);
                return RedirectToAction("index");

            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }


        private PurchaseIndexViewModel getViewModelFrom(List<Purchase> purchases)
        {
            PurchaseIndexViewModel VM = new PurchaseIndexViewModel();
            VM.purchase_items = new List<PurchaseDetail>();
            foreach (var purchase in purchases)
            {
                var purchaseDetail = _mapper.Map<PurchaseDetail>(purchase);
                VM.purchase_items.Add(purchaseDetail);
            }
            return VM;
        }
    }
}