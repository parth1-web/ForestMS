using LE.Account.Entities;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/ledger-setup")]
    public class LedgerSetupController : BaseController
    {
        private LedgerSetupService _ledgerSetupService;
        private LedgerSetupRepository _ledgerSetupRepo;
        private LedgerRepository _ledgerRepo;
        private LedgerGroupRepository _ledgerGroupRepo;

        public LedgerSetupController(LedgerSetupService ledgerSetupService, LedgerSetupRepository ledgerSetupRepo, LedgerRepository ledgerRepo, LedgerGroupRepository ledgerGroupRepo)
        {
            _ledgerSetupService = ledgerSetupService;
            _ledgerSetupRepo = ledgerSetupRepo;
            _ledgerRepo = ledgerRepo;
            _ledgerGroupRepo = ledgerGroupRepo;

        }

        [Route("")]
        [Route("index")]
        public IActionResult Index(List<LedgerSetup> setups = null)
        {
            if (setups.Count == 0)
            {
                setups = _ledgerSetupRepo.getQueryable().ToList();
                ViewBag.bankLedgers = _ledgerRepo.getLedgersUnderAssetsGroup();
                ViewBag.cashLedgers = _ledgerRepo.getLedgersUnderAssetsGroup();
                ViewBag.purchaseLedgers = _ledgerRepo.getLedgersUnderExpensesGroup();
                ViewBag.salesLedgers = _ledgerRepo.getLedgersUnderIncomeGroup();
                ViewBag.salesReturnLedgers = _ledgerRepo.getLedgersUnderExpensesGroup();
                ViewBag.purchaseReturnLedgers = _ledgerRepo.getLedgersUnderIncomeGroup();
                ViewBag.discountAllowedLedgers = _ledgerRepo.getLedgersUnderExpensesGroup();
                ViewBag.discountReceivedLedgers = _ledgerRepo.getLedgersUnderIncomeGroup();
                ViewBag.salesTaxLedgers = _ledgerRepo.getLedgersUnderLiabilitiesGroup();
                ViewBag.debtorsGroup = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_type == Account.Common.Enums.LedgerGroupType.asset);
                ViewBag.creditorsGroup = _ledgerGroupRepo.getQueryable().Where(a => a.ledger_group_type == Account.Common.Enums.LedgerGroupType.liability);

            }
            return View(setups);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(List<LedgerSetup> datas)
        {
            try
            {
                _ledgerSetupService.saveOrUpdate(datas);
                AlertHelper.setMessage(this, "Ledger Setup saved successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index), datas);
        }

    }
}