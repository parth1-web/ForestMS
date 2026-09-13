using DateConverter.Core.Service_Factory;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize]
    [Area("billing")]
    [Route("billing/day-close")]
    public class DayCloseController : BaseController
    {
        private DayCloseService _dayCloseService;
        private DayCloseRepository _dayCloseRepo;
        private CounterSalesRepository _counterSalesRepo;
        private WoodBillRepository _woodSalesRepo;
        private FirewoodSalesRepository _firewoodSalesRepo;

        public DayCloseController(DayCloseService dayCloseService, DayCloseRepository dayCloseRepo, CounterSalesRepository counterSalesRepo, WoodBillRepository woodSalesRepo, FirewoodSalesRepository firewoodSalesRepo)
        {
            _dayCloseService = dayCloseService;
            _dayCloseRepo = dayCloseRepo;
            _counterSalesRepo = counterSalesRepo;
            _woodSalesRepo = woodSalesRepo;
            _firewoodSalesRepo = firewoodSalesRepo;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add(DayCloseIndexViewModel vm)
        {
            var dateService = DateConverterFactory.getDateConverterService();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var clientDate = dateFunction.getDateTimeByTimeZone();
            var engSalesDate = dateService.ToAD(vm.date).getFormattedDate().Add(clientDate.TimeOfDay);

            // P1/B14 fix: screen totals used to include cancelled bills while the posting
            // logic excluded them, so the numbers shown never matched the ledger entry.
            vm.day_counter_sales = _counterSalesRepo.getQueryable().Where(a => a.sales_date.Date == engSalesDate.Date && a.is_cancelled == false).Sum(a => a.bill_amount);

            var counterSales = _counterSalesRepo.getQueryable().Where(a => a.sales_date.Date == engSalesDate.Date && a.is_cancelled == false);

            List<ServiceCount> service_count_list = new List<ServiceCount>();

            foreach (var sales in counterSales)
            {

                foreach (var detail in sales.counter_sales_details)
                {
                    ServiceCount scount = new ServiceCount();
                    scount.service = detail.service;
                    scount.service_id = detail.service_id;
                    scount.qty = detail.qty;
                    service_count_list.Add(scount);
                }
            }

            vm.service_count = service_count_list.GroupBy(x => x.service_id).Select(a => new ServiceCount { service_id = a.First().service_id, qty = a.Sum(c => c.qty), service = a.First().service }).ToList();

            // P1/B14 fix: was null-checking the never-null IQueryable result, so the screen
            // always showed "closed". A day is closed only when a row actually exists.
            vm.day_wood_sales = _woodSalesRepo.getQueryable().Where(a => a.bill_date.Date == engSalesDate.Date && a.is_cancelled == false).Sum(a => a.amount);
            vm.day_firewood_sales = _firewoodSalesRepo.getQueryable().Where(a => a.sales_date.Date == engSalesDate.Date && a.is_cancelled == false).Sum(a => a.total_amount);
            vm.is_closed = true;
            var dayCloseData = _dayCloseRepo.getQueryable().Where(a => a.eng_close_date == engSalesDate).ToList();
            if (!dayCloseData.Any())
            {
                vm.is_closed = false;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("save")]
        public IActionResult save(DayCloseIndexViewModel vm)
        {
            try
            {
                var dateService = DateConverterFactory.getDateConverterService();
                var dateFunction = DateFunctionsFactory.getDateFunctionsService();
                var currentDate = dateFunction.getDateTimeByTimeZone();
                var engSalesDate = dateService.ToAD(vm.date).getFormattedDate().Add(currentDate.TimeOfDay);

                DayCloseDto dto = new DayCloseDto();
                dto.nep_close_date = vm.date;
                dto.eng_close_date = engSalesDate;
                _dayCloseService.insert(dto);
                AlertHelper.setMessage(this, "Day Closed Successfully.", messageType.success);
                return Redirect("/billing/day-close/new");
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
                return Redirect("/billing/day-close/new");
            }
        }
    }
}