using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Enums;
using LE.Common.Exceptions;
using LE.Service.Repository.Interface;
using LE.Web.Helpers;
using LE.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LE.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private OrganizationSetupRepository _orgSetupRepo;
        private CounterSalesRepository _counterSalesRepository;
        private WoodBillRepository _woodSalesRepository;
        private FirewoodSalesRepository _firewoodSalesRepository;
        private MemberRepository _memberRepository;
        private readonly LedgerSetupRepository _ledgerSetupRepo;
        private readonly LedgerIdProvider _ledgerIdProvider;
        private readonly LedgerRepository _ledgerRepo;
        private readonly TransactionDetailRepository _trasactionDetailRepo;
        private readonly IHtmlLocalizer<HomeController> _localizer;

        public HomeController(UserRepository userRepo, OrganizationSetupRepository orgSetupRepo, CounterSalesRepository counterSalesRepository, FirewoodSalesRepository firewoodSalesRepository, WoodBillRepository woodSalesRepository, MemberRepository memberRepository, IHtmlLocalizer<HomeController> localizer, LedgerSetupRepository ledgerSetupRepo, LedgerRepository ledgerRepo, TransactionDetailRepository
            transactionDetailRepo, LedgerIdProvider ledgerIdProvider)
        {
            _ledgerIdProvider = ledgerIdProvider;
            _ledgerSetupRepo = ledgerSetupRepo;
            _userRepo = userRepo;
            _ledgerRepo = ledgerRepo;
            _orgSetupRepo = orgSetupRepo;
            _trasactionDetailRepo = transactionDetailRepo;
            _counterSalesRepository = counterSalesRepository;
            _woodSalesRepository = woodSalesRepository;
            _firewoodSalesRepository = firewoodSalesRepository;
            _memberRepository = memberRepository;
            this._localizer = localizer;
        }
        public IActionResult Index()
        {
            try
            {
                // getLoggedInUserId resolves the authentication row -> user row; the
                // authentication id only matches a user id by seed coincidence.
                ViewBag.LoggedInUserName = _userRepo.getById(getLoggedInUserId())?.full_name;
                ViewBag.organizationName = _orgSetupRepo.getByKey(OrganizationSetup.Organization_Name.ToString())?.value;
                ViewBag.totalMembers = _memberRepository.getQueryable().Where(a => a.IsActive == true).Count();

                LedgerDashboardModel vm = new LedgerDashboardModel();

                var cashLedger = _ledgerIdProvider.getLedgerIdOfLedger(Account.Common.Enums.LedgerSetup.cash);
                var bankLedger = _ledgerIdProvider.getLedgerIdOfLedger(Account.Common.Enums.LedgerSetup.bank);

                if (cashLedger <= 0 && bankLedger <= 0)
                {
                    throw new ItemNotFoundException("Please Setup Ledger for the first time.");
                }

                var cashLedgerGroupId = _ledgerRepo.getById(cashLedger).ledger_group_id;
                var bankLedgerGroupId = _ledgerRepo.getById(bankLedger).ledger_group_id;

                var bankLedgers = _ledgerRepo.getLedgersByLedgerGroup(bankLedgerGroupId);
                var cashLedgers = _ledgerRepo.getLedgersByLedgerGroup(cashLedgerGroupId);

                var currentDate = DateTime.Now.Date;
                var counterSalesDetails = _counterSalesRepository.getQueryable().Where(x => x.sales_date.Date == currentDate).ToList();
                vm = getViewModelFrom(bankLedgers, cashLedgers, counterSalesDetails);

                return View(vm);
            }
            catch (Exception e)
            {
                ExceptionMessageHelper.setMessage(this, e, messageType.error);
                return RedirectToAction("ledger-setup", "accounting");
            }
        }

        private LedgerDashboardModel getViewModelFrom(List<Ledger> bankLedgers, List<Ledger> cashLedgers, List<CounterSales> counterSalesDetails)
        {
            LedgerDashboardModel vm = new LedgerDashboardModel();
            vm.counter_sales_data = new List<CounterSalesData>();
            vm.bank_ledger_details = new List<BankLedgerDetails>();
            vm.cash_ledger_details = new List<CashLedgerDetails>();

            foreach (var bledger in bankLedgers)
            {
                BankLedgerDetails bank = new BankLedgerDetails();
                bank.ledger_id = bledger.ledger_id;
                bank.name = bledger.name;
                bank.amount = _trasactionDetailRepo.getEndBalance(bledger.ledger_id);
                vm.bank_ledger_details.Add(bank);
            }

            foreach (var cledger in cashLedgers)
            {
                CashLedgerDetails cash = new CashLedgerDetails();
                cash.ledger_id = cledger.ledger_id;
                cash.name = cledger.name;
                cash.amount = _trasactionDetailRepo.getEndBalance(cledger.ledger_id);
                vm.cash_ledger_details.Add(cash);
            }

            foreach (var counter in counterSalesDetails.GroupBy(a => a.user_id))
            {

                decimal amt = 0;
                long userId = 0;
                foreach (var data in counter)
                {
                    amt += data.net_total;
                    userId = data.user_id;
                }
                CounterSalesData cdata = new CounterSalesData();
                cdata.user = _userRepo.getById(userId).full_name;
                cdata.amount = amt;
                vm.counter_sales_data.Add(cdata);
            }

            return vm;
        }

        public IActionResult getDashboardDetails()
        {
            var totalCounterSales = _counterSalesRepository.getQueryable().Where(x => x.sales_date.Date == DateTime.Now.Date).Sum(a => a.net_total);
            var totalWoodSales = _woodSalesRepository.getQueryable().Where(x => x.bill_date.Date == DateTime.Now.Date).Sum(a => a.amount);
            var totalFirewoodSales = _firewoodSalesRepository.getQueryable().Where(x => x.sales_date.Date == DateTime.Now.Date).Sum(a => a.total_amount);

            return Json(new { counter = totalCounterSales, wood = totalWoodSales, firewood = totalFirewoodSales });
        }

        [Route("set-lang")]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
