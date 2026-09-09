using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FurnitureSalesDetailAssemblerImpl : FurnitureSalesDetailAssembler
    {
        private readonly FurnitureSalesRepository _furnitureSalesRepo;

        public FurnitureSalesDetailAssemblerImpl(FurnitureSalesRepository furnitureSalesRepo)
        {
            _furnitureSalesRepo = furnitureSalesRepo;
        }
        public void copy(FurnitureSalesDetail sales_detail, FurnitureSalesDetailDto sales_detail_dto)
        {
            sales_detail.furniture_sales_id = sales_detail_dto.furniture_sales_id;
            sales_detail.rate = sales_detail_dto.rate;
            sales_detail.quantity = sales_detail_dto.quantity;
            sales_detail.amount = sales_detail_dto.amount;
            sales_detail.furniture_id = sales_detail_dto.furniture_id;
            sales_detail.furnitureSales = _furnitureSalesRepo.getById(sales_detail_dto.furniture_sales_id) ?? throw new ItemNotFoundException($"Firewood Sales with the id {sales_detail_dto.furniture_sales_id} doesnot exist.");
        }
    }
}
