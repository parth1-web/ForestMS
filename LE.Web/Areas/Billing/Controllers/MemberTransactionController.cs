using AutoMapper;
using DateConverter.Core.Service_Factory;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Enums;
using LE.Service.Repository.Interface;
using LE.Web.Areas.Billing.ViewModels;
using LE.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace LE.Web.Areas.Billing.Controllers
{
    [Area("billing")]
    [Route("billing/member-transaction")]

    public class MemberTransactionController : BaseController
    {
        private WoodBillMemberTransactionRepository _memberTransactionRepo;
        private IMapper _mapper;
        private readonly OrganizationSetupRepository _organizationSetupRepository;
        private MemberRepository _memberRepo;
        public MemberTransactionController(WoodBillMemberTransactionRepository memberTransactionRepo, IMapper mapper, MemberRepository memberRepo, OrganizationSetupRepository organizationSetupRepository)
        {
            _memberTransactionRepo = memberTransactionRepo;
            _mapper = mapper;
            _memberRepo = memberRepo;
            _organizationSetupRepository = organizationSetupRepository;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index(MemberTransactionIndexViewModel vm)
        {
            var member = _memberRepo.getQueryable().Where(a => a.IsActive == true);
            ViewBag.members = new SelectList(member, "MemberId", "FullName");
            setViewModelFromData(vm);
            return View(vm);
        }

        private void setViewModelFromData(MemberTransactionIndexViewModel vm)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();

            var startDate = dateConverterService.ToAD(vm.start_date).getFormattedDate();
            var endDate = dateConverterService.ToAD(vm.end_date).getFormattedDate();

            var transactions = _memberTransactionRepo.getAll();
            if (vm.member_id > 0)
            {
                transactions = transactions.Where(a => a.member_id == vm.member_id).ToList();
            }

            transactions = transactions.Where(a => a.woodBill.bill_date.Date >= startDate.Date && a.woodBill.bill_date.Date <= endDate.Date && a.is_cancelled == vm.is_cancelled).ToList();
            
            foreach (var transaction in transactions)
            {
                var trans = _mapper.Map<MemberTransactionDetails>(transaction);
                vm.member_transactions.Add(trans);
            }
        }

        [HttpGet]
        [Route("report")]
        public IActionResult report(MemberTransactionIndexViewModel vm)
        {
            setViewModelFromData(vm);
            ViewBag.OrganizationName = _organizationSetupRepository.getByKey(OrganizationSetup.Organization_Name.ToString()).value;
            ViewBag.Address = _organizationSetupRepository.getByKey(OrganizationSetup.Address.ToString()).value;
            return View(vm);
        }
    }
}