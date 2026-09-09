using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Common.Exceptions;

namespace LE.Billing.Service.Assemblers.Implementations
{
    public class FirewoodSalesDetailAssemblerImpl:FirewoodSalesDetailAssembler
    {
        private readonly FirewoodSalesRepository _firewoodSalesRepo;

        public FirewoodSalesDetailAssemblerImpl(FirewoodSalesRepository firewoodSalesRepo)
        {
            _firewoodSalesRepo = firewoodSalesRepo;
        }
        public void copy(FirewoodSalesDetail sales_detail, FirewoodSalesDetailDto sales_detail_dto)
        {
            sales_detail.firewood_sales_id = sales_detail_dto.firewood_sales_id;
            sales_detail.rate = sales_detail_dto.rate;
            sales_detail.quantity = sales_detail_dto.quantity;
            sales_detail.amount = sales_detail_dto.amount;
            sales_detail.stock_item_id = sales_detail_dto.stock_item_id;
            sales_detail.firewoodSales = _firewoodSalesRepo.getById(sales_detail_dto.firewood_sales_id) ?? throw new ItemNotFoundException($"Firewood Sales with the id {sales_detail_dto.firewood_sales_id} doesnot exist.");
        }
    }
}
