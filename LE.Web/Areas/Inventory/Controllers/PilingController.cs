using AutoMapper;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Dto;
using LE.Inventory.Infrastructure.Repository.Interface;
using LE.Inventory.Service.Services.Interface;
using LE.Web.Areas.Inventory.FilterModel;
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
    [Route("inventory/piling")]
    public class PilingController : Controller
    {
        private readonly PilingRepository _pilingRepo;
        private readonly PilingService _pilingService;
        private readonly PaginatedMetaService _paginatedMetaService;
        private IMapper _mapper;

        public PilingController(PilingRepository pilingRepo, PilingService pilingService, PaginatedMetaService paginatedMetaService, IMapper mapper)
        {
            _pilingRepo = pilingRepo;
            _pilingService = pilingService;
            _paginatedMetaService = paginatedMetaService;
            _mapper = mapper;
        }

        [Route("")]
        [Route("index", Name = "inventory_piling_index")]
        public IActionResult Index(PilingFilter filter)
        {
            var piling = _pilingRepo.getQueryable();

            if (!string.IsNullOrWhiteSpace(filter.title))
            {
                piling = piling.Where(a => a.title.Contains(filter.title));
            }

            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(piling.Count(), filter.page, filter.number_of_rows);
            piling = piling.OrderBy(a => a.piling_id).Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var vm = getViewModel(piling.ToList());

            return View(vm);
        }

        private PilingIndexViewModel getViewModel(List<Piling> pilings)
        {
            PilingIndexViewModel vm = new PilingIndexViewModel();

            vm.pilings = new List<Pilings>();

            foreach (var piling in pilings)
            {
                var data = _mapper.Map<Pilings>(piling);
                vm.pilings.Add(data);
            }

            return vm;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(PilingDto pilingDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _pilingService.save(pilingDto);
                    AlertHelper.setMessage(this, "Piling added Successfully.", messageType.success);
                }
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return View();
            }
        }

        [HttpGet]
        [Route("edit/{piling_id}")]
        public IActionResult edit(long piling_id)
        {
            var piling = _pilingRepo.getById(piling_id);
            var pilingDto = _mapper.Map<PilingDto>(piling);
            return View(pilingDto);
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(PilingDto dto)
        {
            try
            {
                _pilingService.update(dto);
                AlertHelper.setMessage(this, "Piling updated successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("enable/{piling_id}")]
        public IActionResult enable(long piling_id)
        {
            try
            {
                var piling = _pilingRepo.getById(piling_id);
                if (piling != null)
                    _pilingService.enable(piling_id);
                AlertHelper.setMessage(this, "Piling enabled successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("disable/{piling_id}")]
        public IActionResult disable(long piling_id)
        {
            try
            {
                var piling = _pilingRepo.getById(piling_id);
                if (piling != null)
                    _pilingService.disable(piling_id);
                AlertHelper.setMessage(this, "Piling disabled successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("delete/{piling_id}")]
        public IActionResult delete(long piling_id)
        {
            try
            {
                _pilingService.delete(piling_id);
                AlertHelper.setMessage(this, "Piling deleted successfully.", messageType.success);
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
