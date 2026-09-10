using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using LE.Common.Library;
using LE.Service.Repository.Interface;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme + "," + Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [Area("billing")]
    [Route("billing/counter-billing")]
    public class CounterBillingController : BaseController
    {
        private readonly CounterSalesService _counterSalesService;
        private readonly CounterSalesRepository _counterSalesRepo;
        private readonly ServiceRepository _serviceRepo;
        private readonly AuthenticationRepository _authenticationRepo;
        private readonly UserRepository _userRepo;

        public CounterBillingController(CounterSalesService counterSalesService, CounterSalesRepository counterSalesRepo, ServiceRepository serviceRepo, AuthenticationRepository authenticationRepo, UserRepository userRepo)
        {
            _counterSalesRepo = counterSalesRepo;
            _counterSalesService = counterSalesService;
            _serviceRepo = serviceRepo;
            _authenticationRepo = authenticationRepo;
            _userRepo = userRepo;
        }



        [HttpPost]
        // Consumed by the counter POS desktop client (JWT-authenticated), which cannot
        // send browser antiforgery tokens; identity is enforced server-side above.
        [IgnoreAntiforgeryToken]
        [Route("save")]
        public IActionResult save([FromBody] CounterSalesDto dto)
        {
            try
            {
                // The bill is always attributed to the authenticated user from the session,
                // never to a client-supplied id (prevents forged audit attribution).
                dto.user_id = getLoggedInAuthenticationId();
                long salesId = _counterSalesService.makeSales(dto);
                var sales = _counterSalesRepo.getById(salesId);
                var responseData = buildResponseJson(sales);
                return Content(JsonWrapper.buildSuccessJson(responseData), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonWrapper.buildErrorJson(ex.Message), "application/json");
            }
        }

        [Route("current-day")]
        [HttpGet]
        public IActionResult getSalesDataOfUserOfCurrentDay([FromQuery] long user_id)
        {
            try
            {
                // Users may only view their own sales of the current day; an authenticated
                // user querying a different user's sales is rejected (anti-IDOR).
                long loggedInUserId = getLoggedInAuthenticationId();
                if (user_id != loggedInUserId)
                {
                    return Content(JsonWrapper.buildErrorJson("You are not authorized to view sales of another user."), "application/json");
                }

                var sales = _counterSalesRepo.getQueryable().Where(a => a.user_id == user_id && a.sales_date.Date == DateTime.Now.Date).ToList();

                var salesDetails = sales.SelectMany(a => a.counter_sales_details).ToList();

                List<long> serviceIds = salesDetails.Select(a => a.service_id).Distinct().ToList();

                var serviceDetails = _serviceRepo.getQueryable().Where(a => serviceIds.Contains(a.service_id)).Select(a => new
                {

                    a.service_id,
                    a.name
                }).ToList();

                string userNameOfUser = "";

                setUserNameOfUserPerformingAction(ref userNameOfUser, user_id);

                List<dynamic> datas = new List<dynamic>();

                foreach (var sale in sales)
                {
                    var data = new
                    {
                        bill_no = sale.sales_id,
                        nep_bill_date = sale.nep_sales_date,
                        bill_total = sale.bill_amount,
                        discount = sale.discount_amount,
                        sale.net_total,
                        bill_date = sale.sales_date,
                        sale.remarks,
                        username = userNameOfUser,
                        amount_in_words = NumberToNepaliCurrencyText.NumberToCurrencyText(sale.net_total, MidpointRounding.AwayFromZero),

                        bill_details = salesDetails.Where(a => a.sales_id == sale.sales_id).Select(a => new
                        {
                            particulars = serviceDetails.Where(b => b.service_id == a.service_id).Single().name,
                            a.rate,
                            a.qty,
                            a.service_id,
                            amount = a.qty * a.rate,
                            vat = a.tax_amount
                        })
                    };

                    datas.Add(data);
                }

                return Content(JsonWrapper.buildSuccessJson(datas), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonWrapper.buildErrorJson(ex.Message), "application/json");
            }
        }

        private object buildResponseJson(CounterSales sales)
        {
            var salesDetails = sales.counter_sales_details.ToList();
            List<long> serviceIds = salesDetails.Select(a => a.service_id).Distinct().ToList();

            var serviceDetails = _serviceRepo.getQueryable().Where(a => serviceIds.Contains(a.service_id)).Select(a => new
            {

                a.service_id,
                a.name
            }).ToList();

            string userNameOfUserPerformingAction = "";

            setUserNameOfUserPerformingAction(ref userNameOfUserPerformingAction, sales.user_id);

            var data = new
            {
                bill_no = sales.sales_id,
                nep_bill_date = sales.nep_sales_date,
                bill_total = sales.bill_amount,
                discount = sales.discount_amount,
                sales.net_total,
                bill_date = sales.sales_date,
                sales.remarks,
                username = userNameOfUserPerformingAction,
                amount_in_words = NumberToNepaliCurrencyText.NumberToCurrencyText(sales.net_total, MidpointRounding.AwayFromZero),

                bill_details = salesDetails.Select(a => new
                {
                    particulars = serviceDetails.Where(b => b.service_id == a.service_id).Single().name,
                    a.rate,
                    a.qty,
                    amount = a.qty * a.rate,
                    vat = a.tax_amount
                })
            };
            return data;
        }

        private void setUserNameOfUserPerformingAction(ref string username, long user_id)
        {
            var authenticationDetail = _authenticationRepo.getById(user_id) ?? throw new ItemNotFoundException($"Authentication with id {user_id} doesnot exist.");

            var userId = authenticationDetail.type_id;

            username = _userRepo.getById(userId)?.full_name;
        }


    }
}