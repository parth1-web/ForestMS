using AutoMapper;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Web.Areas.Billing.FilterModel;
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
    [Route("billing/tole")]
    public class ToleController : BaseController
    {
        private ToleService _toleService;
        private ToleRepository _toleRepo;
        private IMapper _mapper;
        private PaginatedMetaService _paginatedMetaService;

        public ToleController(ToleService toleService, ToleRepository toleRepo, IMapper mapper, PaginatedMetaService paginatedMetaService)
        {
            _toleService = toleService;
            _toleRepo = toleRepo;
            _mapper = mapper;
            _paginatedMetaService = paginatedMetaService;
        }


        [Route("")]
        [Route("index")]
        public IActionResult Index(ToleFilter filter)
        {
            var tole = _toleRepo.getQueryable();
            if (!string.IsNullOrEmpty(filter.tole_no))
            {
                tole = tole.Where(a => a.tole_no == filter.tole_no);
            }
            tole = tole.OrderBy(a => a.tole_no);
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(tole.Count(), filter.page, filter.number_of_rows);
            tole = tole.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var toles = tole.ToList();

            ToleIndexViewModel toleIndexVM = getViewModelFrom(toles);
            return View(toleIndexVM);
        }

        private ToleIndexViewModel getViewModelFrom(List<Tole> toles)
        {
            ToleIndexViewModel VM = new ToleIndexViewModel();
            VM.toles = new List<ToleDetail>();
            foreach (var tole in toles)
            {
                var toleDetail = _mapper.Map<ToleDetail>(tole);
                VM.toles.Add(toleDetail);
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
        public IActionResult add(ToleDto toleDto)
        {
            try
            {
                _toleService.insert(toleDto);
                AlertHelper.setMessage(this, "Tole Added successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{tole_id}")]
        public IActionResult edit(long tole_id)
        {
            var tole = _toleRepo.getById(tole_id);
            ToleDto toleDto = _mapper.Map<ToleDto>(tole);
            return View(toleDto);
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(ToleDto toleDto)
        {
            try
            {
                _toleService.update(toleDto);
                AlertHelper.setMessage(this, "Tole updated successfully.", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }


        [HttpGet]
        [Route("delete/{tole_id}")]
        public IActionResult delete(long tole_id)
        {
            try
            {
                _toleService.delete(tole_id);
                AlertHelper.setMessage(this, "Tole Deleted Successfully.", messageType.success);
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