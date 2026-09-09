using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using static LE.Common.Library.DateConverter.Entity.NepaliDate;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FurnitureSalesAssemblerImpl : FurnitureSalesAssembler
    {
        private BillingSettingsRepository _billingSettingsRepository;
        public FurnitureSalesAssemblerImpl(BillingSettingsRepository billingSettingsRepository)
        {
            _billingSettingsRepository = billingSettingsRepository;

        }
        public void copy(FurnitureSales furniture_sales, FurnitureSalesDto furniture_sales_dto)
        {
            var dateConverterService = DateConverterFactory.getDateConverterService();
            furniture_sales.furniture_sales_id = _billingSettingsRepository.getFurnitureBillingSequence();
            furniture_sales.user_id = furniture_sales_dto.user_id;
            furniture_sales.total_amount = furniture_sales_dto.total_amount;
            furniture_sales.sales_type = furniture_sales_dto.sales_type;
            furniture_sales.type_id = furniture_sales_dto.type_id;
            furniture_sales.sales_date = furniture_sales_dto.sales_date;
            furniture_sales.remarks = furniture_sales_dto.remarks;
            furniture_sales.others_name = furniture_sales_dto.others_name;
            furniture_sales.address = furniture_sales_dto.address;
            furniture_sales.is_cancelled = furniture_sales_dto.is_cancelled;
            furniture_sales.nep_sales_date = dateConverterService.ToBS(furniture_sales.sales_date.Date, DateFormats.yMd).getFormattedDate();

        }
    }
}
