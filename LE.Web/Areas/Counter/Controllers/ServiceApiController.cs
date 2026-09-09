using LE.Billing.Infrastructure.Repository.Interface;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LE.Web.Areas.Counter.Controllers
{
    [Route("api/service")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public sealed class ServiceApiController : ControllerBase
    {
        private readonly ServiceRepository _serviceRepo;
        private readonly CounterSalesRepository _counterSalesRepo;
        public ServiceApiController(ServiceRepository serviceRepo, CounterSalesRepository counterSalesRepo)
        {
            _serviceRepo = serviceRepo;
            _counterSalesRepo = counterSalesRepo;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public IActionResult get()
        {
            try
            {
                var services = _serviceRepo.getAll();
                var responseData = services.Select(a => new
                {
                    a.category_id,
                    a.service_id,
                    a.name,
                    tax = Math.Round((a.tax * a.rate) / 100, 2),
                    rate = a.rate - Math.Round((a.tax * a.rate) / 100, 2),
                }).ToList();

                return Content(JsonWrapper.buildSuccessJson(responseData), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonWrapper.buildErrorJson(ex.Message), "application/json");
            }

        }
        [HttpGet]
        [Route("")]
        [Route("sales-details")]
        public IActionResult salesDetails()
        {
            try
            {
                var userId = 1;
                var services = _counterSalesRepo.getQueryable().Where(a => a.sales_date.Date == DateTime.Now.Date && a.user_id == userId);
                var responseData = services.Select(a => new
                {
                    amount = services.Sum(x => x.net_total)
                }).ToList();

                return Content(JsonWrapper.buildSuccessJson(responseData), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonWrapper.buildErrorJson(ex.Message), "application/json");
            }
        }
    }
}