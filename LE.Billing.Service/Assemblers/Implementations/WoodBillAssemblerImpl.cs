using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class WoodBillAssemblerImpl : WoodBillAssembler
    {
        private BillingSettingsRepository _billingSettingsRepository;
        public WoodBillAssemblerImpl(BillingSettingsRepository billingSettingsRepository)
        {
            _billingSettingsRepository = billingSettingsRepository;

        }
        public void copy(WoodBill woodBill, WoodBillDto woodBillDto)
        {
            var dateConverter = DateConverterFactory.getDateConverterService();
            woodBill.wood_bill_id = _billingSettingsRepository.getWoodBillingSequence();
            woodBill.bill_date = woodBillDto.bill_date;
            woodBill.amount = woodBillDto.amount;
            woodBill.remarks = woodBillDto.remarks;
            woodBill.user_id = woodBillDto.user_id;
            woodBill.address = woodBillDto.address;
            woodBill.name = woodBillDto.name;
            woodBill.tax_amount = woodBillDto.tax_amount;
            woodBill.sales_type = woodBillDto.sales_type;
            woodBill.is_cancelled = woodBillDto.is_cancelled;
            woodBill.nep_bill_date = dateConverter.ToBS(woodBillDto.bill_date.Date,DateFormats.yMd).getFormattedDate();
        }
    }
}
