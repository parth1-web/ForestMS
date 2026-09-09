using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FirewoodSalesAssemblerImpl : FirewoodSalesAssembler
    {
        private BillingSettingsRepository _billingSettingsRepository;
        public FirewoodSalesAssemblerImpl(BillingSettingsRepository billingSettingsRepository)
        {
            _billingSettingsRepository = billingSettingsRepository;

        }
        public void copy(FirewoodSales firewood_sales, FirewoodSalesDto firewood_sales_dto)
        {
            var dateConverterService =DateConverterFactory.getDateConverterService();
            firewood_sales.firewood_sales_id = _billingSettingsRepository.getFireWoodBillingSequence();
            firewood_sales.user_id = firewood_sales_dto.user_id;
            firewood_sales.total_amount = firewood_sales_dto.total_amount;
            firewood_sales.sales_type = firewood_sales_dto.sales_type;
            firewood_sales.type_id = firewood_sales_dto.type_id;
            firewood_sales.sales_date = firewood_sales_dto.sales_date;
            firewood_sales.remarks = firewood_sales_dto.remarks;
            firewood_sales.others_name = firewood_sales_dto.others_name;
            firewood_sales.address = firewood_sales_dto.address;
            firewood_sales.is_cancelled = firewood_sales_dto.is_cancelled;
            firewood_sales.nep_sales_date = dateConverterService.ToBS(firewood_sales.sales_date.Date,DateFormats.yMd).getFormattedDate();

        }
    }
}
