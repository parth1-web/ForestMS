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
    [Route("accounting/fiscal-year-setup")]
    public class FiscalYearSetupController : BaseController
    {
        private AccountSettingsRepository _accountSettingRepo;
        private FiscalYearSetupService _fiscalYearSetupService;
        public FiscalYearSetupController(AccountSettingsRepository accountSettingRepo, FiscalYearSetupService fiscalYearSetupService)
        {
            _accountSettingRepo = accountSettingRepo;
            _fiscalYearSetupService = fiscalYearSetupService;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index(List<AccountSettings> setups = null)
        {
            try
            {
                if (setups.Count == 0)
                {
                    setups = _accountSettingRepo.getQueryable().ToList();

                }
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return View(setups);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(List<AccountSettings> datas)
        {
            try
            {
                _fiscalYearSetupService.saveOrUpdate(datas);
                AlertHelper.setMessage(this, "Fiscal Year Setup saved successfully.", messageType.success);
            }
            catch (Exception ex)
            {
                AlertHelper.setMessage(this, ex.Message, messageType.error);
            }
            return RedirectToAction(nameof(Index), datas);
        }
    }
}