using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/journal-entries")]
    public class JournalController : BaseController
    {
        private readonly LedgerRepository _ledgerRepo;
        private JournalService _journalService;
        private IMapper _mapper;
        public JournalController(LedgerRepository ledgerRepo, JournalService journalService, IMapper mapper)
        {
            _ledgerRepo = ledgerRepo;
            _journalService = journalService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            JournalModel journalModel = new JournalModel();

            var ledger = _ledgerRepo.getQueryable().Select(a => new { id = a.ledger_id, name = string.Format("{0}-{1}", a.code, a.name) }).ToList();
            ViewBag.ledgers = new SelectList(ledger, "id", "name"); ;

            return View(journalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("new")]
        public JsonResult add([FromBody] JournalModel journalModel)
        {
            try
            {
                JournalDto journalDto = getJournalDtoFromData(journalModel);
                _journalService.makeJournalEntries(journalDto);
                AlertHelper.setMessage(this, "New Journal has been created successfully", messageType.success);
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(ExceptionMessageHelper.buildErrorObject(ex));
            }
        }

        private JournalDto getJournalDtoFromData(JournalModel journalModel)
        {
            JournalDto journalDto = new JournalDto();
            journalDto.journalDetailDto = new List<JournalDetailDto>();
            foreach (var model in journalModel.journalModelDetails)
            {
                var details = _mapper.Map<JournalDetailDto>(model);
                journalDto.journalDetailDto.Add(details);
            }
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var currentDate = dateFunction.getDateTimeByTimeZone();

            journalDto.transaction_date = dateConverterService.ToAD(journalModel.nep_transaction_date).getFormattedDate().Add(currentDate.TimeOfDay);
            journalDto.remarks = journalModel.remarks;
            journalDto.voucher_no = journalModel.voucher_no;
            return journalDto;
        }
    }
}