using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class CounterSalesAssemblerImpl : CounterSalesAssembler
    {
        private BillingSettingsRepository _billingSettingsRepository;
        public CounterSalesAssemblerImpl(BillingSettingsRepository billingSettingsRepository)
        {
            _billingSettingsRepository = billingSettingsRepository;

        }
        public void copy(CounterSales counter_sales, CounterSalesDto counter_sales_dto)
        {
            var dateConverterService =DateConverterFactory.getDateConverterService();
            counter_sales.sales_id = _billingSettingsRepository.getCounterBillingSequence();
            counter_sales.user_id = counter_sales_dto.user_id;
            counter_sales.bill_amount = counter_sales_dto.bill_amount;
            counter_sales.discount_amount = counter_sales_dto.discount_amount;
            counter_sales.net_total = counter_sales_dto.net_total;
            counter_sales.return_amount = counter_sales_dto.return_amount;
            counter_sales.remarks = counter_sales_dto.remarks;
            counter_sales.tax_amount = counter_sales_dto.tax;
            counter_sales.sales_date = counter_sales_dto.sales_date;
            counter_sales.nep_sales_date = dateConverterService.ToBS(counter_sales_dto.sales_date.Date,DateFormats.yMd).getFormattedDate();

        }
    }
}
