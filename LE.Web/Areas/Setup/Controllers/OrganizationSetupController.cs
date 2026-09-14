using LE.Entities.OrganizationSetup;
using LE.Service.Repository.Interface;
using LE.Service.Services.Interface;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Setup.Controllers
{
    [Authorize]
    [Area("setup")]
    [Route("setup/org-setup")]
    public class OrganizationSetupController : BaseController
    {
        private OrganizationSetupService _orgSetupService;
        private OrganizationSetupRepository _orgSetupRepo;
        private FileHelper _fileHelper;

        public OrganizationSetupController(OrganizationSetupService orgSetupService, OrganizationSetupRepository orgSetupRepo, FileHelper fileHelper)
        {
            _orgSetupService = orgSetupService;
            _orgSetupRepo = orgSetupRepo;
            _fileHelper = fileHelper;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index(List<OrganizationSetup> setups = null)
        {
            if (setups.Count == 0)
            {
                setups = _orgSetupRepo.getQueryable().ToList();
                //ViewBag.bankLedgers = _ledgerGroupRepo.getLedgersUnderBankGroup();
                //ViewBag.cashLedgers = _ledgerGroupRepo.getLedgersUnderCashGroup();
                //ViewBag.purchaseLedgers = _ledgerGroupRepo.getLedgersUnderPurchaseGroup();
                //ViewBag.salesLedgers = _ledgerGroupRepo.getLedgersUnderSalesGroup();
                //ViewBag.salesReturnLedgers = _ledgerGroupRepo.getLedgersUnderSalesReturnGroup();
                //ViewBag.purchaseReturnLedgers = _ledgerGroupRepo.getLedgersUnderPurchaseReturnGroup();
                //ViewBag.discountAllowedLedgers = _ledgerGroupRepo.getLedgersUnderDiscountAllowedGroup();
                //ViewBag.discountReceivedLedgers = _ledgerGroupRepo.getLedgersUnderDiscountReceivedGroup();

            }
            return View(setups);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(List<OrganizationSetup> datas, IFormFile logo)
        {
            try
            {
                if (logo != null)
                {
                    string logos = "logo";
                    var filename = _fileHelper.saveImageAndGetFileName(logo, logos);

                    OrganizationSetup setup = new OrganizationSetup();
                    setup.key = Models.OrganizationSetupKeys.getLogoKey;
                    setup.value = filename;

                    datas.Add(setup);
                }
                _orgSetupService.saveOrUpdate(datas);

                AlertHelper.setMessage(this, "Organization Setup saved successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
            }
            return RedirectToAction(nameof(Index), datas);
        }

    }
}