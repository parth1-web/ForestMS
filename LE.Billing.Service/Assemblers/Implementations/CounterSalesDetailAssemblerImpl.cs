using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class CounterSalesDetailAssemblerImpl:CounterSalesDetailAssembler
    {
        private readonly CounterSalesRepository _counterSalesRepo;

        public CounterSalesDetailAssemblerImpl(CounterSalesRepository counterSalesRepo)
        {
            _counterSalesRepo = counterSalesRepo;
        }
        public void copy(CounterSalesDetail sales_detail, CounterSalesDetailDto sales_detail_dto)
        {
            sales_detail.sales_id = sales_detail_dto.sales_id;
            sales_detail.rate = sales_detail_dto.rate;
            sales_detail.qty = sales_detail_dto.qty;
            sales_detail.tax_amount = sales_detail_dto.tax;
            sales_detail.service_id = sales_detail_dto.service_id;
            
        }
    }
}
