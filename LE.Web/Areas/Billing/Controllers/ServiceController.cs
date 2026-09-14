using AutoMapper;
using LE.Account.Infrastructure.Repository.Interface;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/service")]
    public class ServiceController : BaseController
    {
        private ServiceRepository _serviceRepo;
        private ServiceOfService _serviceOfService;
        private ServiceCategoryRepository _serviceCategoryRepo;
        private PaginatedMetaService _paginatedMetaService;
        private LedgerRepository _ledgerRepo;
        private IMapper _mapper;

        public ServiceController(ServiceRepository serviceRepo, ServiceOfService serviceOfService, IMapper mapper, ServiceCategoryRepository serviceCategoryRepo, PaginatedMetaService paginatedMetaService, LedgerRepository ledgerRepo)
        {
            _serviceRepo = serviceRepo;
            _serviceOfService = serviceOfService;
            _serviceCategoryRepo = serviceCategoryRepo;
            _mapper = mapper;
            _ledgerRepo = ledgerRepo;
            _paginatedMetaService = paginatedMetaService;
        }

        public IActionResult Index(ServiceFilter filter)

        {
            var service = _serviceRepo.getQueryable();
            if (!string.IsNullOrWhiteSpace(filter.name))
            {
                service = service.Where(a => a.name.Contains(filter.name));
            }
            ViewBag.pagerInfo = _paginatedMetaService.GetMetaData(service.Count(), filter.page, filter.number_of_rows);
            service = service.Skip(filter.number_of_rows * (filter.page - 1)).Take(filter.number_of_rows);

            var services = service.ToList();

            ServiceIndexViewModel serviceIndexVM = getViewModelFrom(services);
            return View(serviceIndexVM);
        }

        private ServiceIndexViewModel getViewModelFrom(List<LE.Billing.Entities.Service> services)
        {
            ServiceIndexViewModel VM = new ServiceIndexViewModel();
            VM.services = new List<ServiceDetail>();
            foreach (var service in services)
            {
                var serviceDetail = _mapper.Map<ServiceDetail>(service);
                VM.services.Add(serviceDetail);
            }
            return VM;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            var service_categories = _serviceCategoryRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
            ViewBag.serviceCategories = new SelectList(service_categories, "category_id", "name");

            var ledger = _ledgerRepo.getQueryable().ToList();
            ViewBag.ledgers = new SelectList(ledger, "ledger_id", "name");
            return View();
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(ServiceModel serviceModel)
        {
            try
            {
                ServiceDto serviceDto = new ServiceDto();
                serviceDto = getDtoFromModel(serviceModel);
                AlertHelper.setMessage(this, "Service Added Successfully.");
                _serviceOfService.insert(serviceDto);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("edit/{service_id}")]
        public IActionResult edit(long service_id)
        {
            try
            {
                var service_categories = _serviceCategoryRepo.getQueryable().Where(a => a.is_enabled == true).ToList();
                ViewBag.serviceCategories = new SelectList(service_categories, "category_id", "name");
                var ledger = _ledgerRepo.getQueryable().ToList();
                ViewBag.ledgers = new SelectList(ledger, "ledger_id", "name");
                var services = _serviceRepo.getQueryable().ToList();
                var serviceDetails = _serviceRepo.getById(service_id);
                var serviceModel = getModelFrom(serviceDetails);
                return View(serviceModel);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        [Route("edit")]
        public IActionResult edit(ServiceModel serviceModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ServiceDto serviceDto = getDtoFromModel(serviceModel);
                    _serviceOfService.update(serviceDto);
                    AlertHelper.setMessage(this, "Service Updated successfully.", messageType.success);
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
            return View(serviceModel);
        }

        [HttpGet]
        [Route("enable/{service_id}")]
        public IActionResult enable(long service_id)
        {
            try
            {
                _serviceOfService.enable(service_id);
                AlertHelper.setMessage(this, "Service enabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("disable/{service_id}")]
        public IActionResult disable(long service_id)
        {
            try
            {
                _serviceOfService.disable(service_id);
                AlertHelper.setMessage(this, "Service disabled successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("delete/{service_id}")]
        public IActionResult delete(long service_id)
        {
            try
            {
                _serviceOfService.delete(service_id);
                AlertHelper.setMessage(this, "Service deleted successfully.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }

        private ServiceModel getModelFrom(LE.Billing.Entities.Service service)
        {
            return _mapper.Map<ServiceModel>(service);
        }

        private ServiceDto getDtoFromModel(ServiceModel serviceModel)
        {
            return _mapper.Map<ServiceDto>(serviceModel);
        }

    }
}