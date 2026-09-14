using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Account.Common.Enums;
using LE.Account.Infrastructure.Dto;
using LE.Account.Infrastructure.Repository.Interface;
using LE.Account.Service.Services.Interface;
using LE.Common.Enums;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Accounting.Models;
using LE.Web.Areas.Accounting.ViewModels;
using LE.Web.Controllers;
using LE.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Web.Areas.Accounting.Controllers
{
    [Authorize]
    [Area("accounting")]
    [Route("accounting/payment")]
    public class PaymentController : BaseController
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly LedgerRepository _ledgerRepo;
        private readonly IMapper _mapper;
        private readonly PaymentService _paymentService;
        private OrganizationSetupRepository _organizationSetupRepository;

        public PaymentController(PaymentRepository paymentRepo, LedgerRepository ledgerRepo, IMapper mapper, PaymentService paymentService, OrganizationSetupRepository organizationSetupRepository)
        {
            _paymentRepo = paymentRepo;
            _ledgerRepo = ledgerRepo;
            _mapper = mapper;
            _paymentService = paymentService;
            _organizationSetupRepository = organizationSetupRepository;
        }

        [Route("")]
        [Route("index")]
        public IActionResult index(PaymentIndexViewModel vm)
        {
            vm = getViewModel(vm);
            return View(vm);
        }

        [Route("report")]
        public IActionResult report(PaymentIndexViewModel vm)
        {
            ViewBag.organizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;

            vm = getViewModel(vm);
            return View(vm);
        }

        [HttpGet]
        [Route("receipt-pdf")]
        public ViewAsPdf receiptPdf(PaymentIndexViewModel vm)
        {
            vm = getViewModel(vm);
            return new ViewAsPdf("report", vm);
        }

        private PaymentIndexViewModel getViewModel(PaymentIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var payments = _paymentRepo.getQueryable().Where(a => a.transaction_date.Date >= startDate.Date && a.transaction_date.Date <= endDate.Date).ToList();
            vm.payments = new List<PaymentDetails>();
            foreach (var payment in payments)
            {
                var details = new PaymentDetails();
                details.payment_from_name = _ledgerRepo.getById(payment.payment_from).name;
                details.payment_to_name = _ledgerRepo.getById(payment.payment_to).name;
                details.nep_transaction_date = payment.nep_transaction_date;
                details.remarks = payment.remarks;
                details.discount = payment.discount;
                details.amount = payment.amount;
                details.cheque_date = payment.cheque_date;
                details.cheque_no = payment.cheque_no;
                details.payment_id = payment.payment_id;
                details.is_cancelled = payment.is_cancelled;
                vm.payments.Add(details);

            }
            return vm;
        }

        [HttpGet]
        [Route("new")]
        public IActionResult add()
        {
            PaymentModel paymentModel = new PaymentModel();
            var ledgers = _ledgerRepo.getQueryable();
            var payment_to_Ledgers = ledgers.Where(l => l.ledger_group.ledger_group_type != LedgerGroupType.asset);
            ViewBag.paymentToLedgers = new SelectList(payment_to_Ledgers, "ledger_id", "name"); ;

            var payment_from_ledgers = ledgers.Where(l => l.ledger_group.ledger_group_type == LedgerGroupType.asset && l.ledger_group_id == 16 || l.ledger_group_id == 19);
            ViewBag.paymentFromLedgers = new SelectList(payment_from_ledgers, "ledger_id", "name");
            return View(paymentModel);
        }

        [HttpPost]
        [Route("new")]
        public IActionResult add(PaymentModel paymentModel)
        {
            try
            {
                PaymentDto paymentDto = getDtoFromPaymentDetails(paymentModel);
                paymentDto.user_id = getLoggedInUserId();
                _paymentService.doPayment(paymentDto);
                AlertHelper.setMessage(this, "Payment made successfully.", messageType.success);
                return RedirectToAction("add");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("add");
            }
        }

        private PaymentDto getDtoFromPaymentDetails(PaymentModel paymentModel)
        {
            var paymentDto = _mapper.Map<PaymentDto>(paymentModel);
            var dateConverterService = DateConverterFactory.getDateConverterService();
            var dateFunction = DateFunctionsFactory.getDateFunctionsService();
            var currentDate = dateFunction.getDateTimeByTimeZone();

            paymentDto.transaction_date = dateConverterService.ToAD(paymentModel.transaction_date).getFormattedDate().Add(currentDate.TimeOfDay);
            return paymentDto;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("cancel/{payment_id}")]
        public IActionResult cancel(long payment_id)
        {
            try
            {
                long userId = getLoggedInUserId();
                _paymentService.cancel(payment_id, userId);
                AlertHelper.setMessage(this, "Payment Cancelled Successfully", messageType.success);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ExceptionMessageHelper.setMessage(this, ex, messageType.error);
                return RedirectToAction("index");
            }
        }
    }

}