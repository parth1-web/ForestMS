using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class ChiranSalesAssemblerImpl : ChiranSalesAssembler
    {
        private BillingSettingsRepository _billingSettingsRepository;
        public ChiranSalesAssemblerImpl(BillingSettingsRepository billingSettingsRepository)
        {
            _billingSettingsRepository = billingSettingsRepository;

        }
        public void copy(ChiranSales chiranSales, ChiranSalesDto chiranSalesDto)
        {
            var dateConverter = DateConverterFactory.getDateConverterService();
            chiranSales.chiran_sales_id = _billingSettingsRepository.getChiranSalesSequence();
            chiranSales.sales_date = chiranSalesDto.sales_date;
            chiranSales.amount = chiranSalesDto.amount;
            chiranSales.remarks = chiranSalesDto.remarks;
            chiranSales.user_id = chiranSalesDto.user_id;
            chiranSales.address = chiranSalesDto.address;
            chiranSales.name = chiranSalesDto.name;
            chiranSales.tax_amount = chiranSalesDto.tax_amount;
            chiranSales.sales_type = chiranSalesDto.sales_type;
            chiranSales.is_cancelled = chiranSalesDto.is_cancelled;
            chiranSales.member_id = chiranSalesDto.member_id;
            chiranSales.nep_sales_date = dateConverter.ToBS(chiranSalesDto.sales_date.Date,DateFormats.yMd).getFormattedDate();
        }
    }
}
