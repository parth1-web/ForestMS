using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Enums;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Area("billing")]
    [Route("billing/counter")]
    public class CounterBillingReportController : BaseController
    {
        private readonly UserRepository _userRepo;
        private readonly CounterSalesRepository _counterSalesRepo;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private IMapper _mapper;
        private readonly CounterSalesService _counterSalesService;

        public CounterBillingReportController(UserRepository userRepo, CounterSalesRepository counterSalesRepo, IMapper mapper, OrganizationSetupRepository organizationSetupRepository, CounterSalesService counterSalesService)
        {
            _organizationSetupRepository = organizationSetupRepository;
            _userRepo = userRepo;
            _counterSalesRepo = counterSalesRepo;
            _mapper = mapper;
            _counterSalesService = counterSalesService;
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(CounterBillIndexViewModel vm)
        {
            var userDetails = _userRepo.getQueryable().Where(a => a.is_active == true).ToList();
            ViewBag.users = new SelectList(userDetails, "user_id", "full_name");

            setReportViewModel(vm);
            return View(vm);
        }


        [HttpGet]
        [Route("bill-details/{bill_id}")]
        public IActionResult billDetails(long bill_id)
        {
            try
            {
                CounterBillIndexViewModel vm = new CounterBillIndexViewModel();
                var counterSales = _counterSalesRepo.getById(bill_id);
                foreach (var detail in counterSales.counter_sales_details)
                {
                    vm.counter_bill_details.Add(_mapper.Map<CounterBillDetail>(detail));
                }
                return View(vm.counter_bill_details);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("index");
            }
        }

        [HttpGet]
        [Route("report-print")]
        public IActionResult reportPrint(CounterBillIndexViewModel vm)
        {
            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            ViewBag.Logo = _organizationSetupRepository.getByKey(OrganizationSetup.Logo.ToString()).value;
            setReportViewModel(vm);
            return View(vm);
        }

        private void setReportViewModel(CounterBillIndexViewModel vm)
        {
            var dateService = DateConverterFactory.getDateConverterService();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var startDate = dateService.ToAD(vm.start_date).getFormattedDate().Date;
            var endDate = dateService.ToAD(vm.end_date).getFormattedDate().Date;

            var details = _counterSalesRepo.getQueryable().Where(a => a.sales_date.Date >= startDate && a.sales_date.Date <= endDate).ToList();

            if (vm.user_id > 0)
            {
                details = _counterSalesRepo.getQueryable().Where(a => a.sales_date.Date >= startDate && a.sales_date.Date <= endDate && a.user_id == vm.user_id && a.is_cancelled == vm.is_cancelled).ToList();
            }

            foreach (var detail in details)
            {
                var bill = _mapper.Map<CounterBillDetail>(detail);
                vm.counter_bill_details.Add(bill);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel/{counter_sales_id}")]
        public IActionResult cancel(long counter_sales_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _counterSalesService.cancel(counter_sales_id, userId);
                AlertHelper.setMessage(this, $"Counter Sales with bill Number{counter_sales_id} Cancelled Successfully.");
                return RedirectToAction("report");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return RedirectToAction("report");
            }
        }
    }
}