using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Implementations
{
    public class FurnitureSalesDetailServiceImpl : FurnitureSalesDetailService
    {
        private readonly FurnitureSalesDetailRepository _furnitureSalesDetailRepo;
        private readonly FurnitureSalesDetailAssembler _furnitureSalesDetailAssembler;

        public FurnitureSalesDetailServiceImpl(FurnitureSalesDetailRepository furnitureSalesDetailRepo, FurnitureSalesDetailAssembler furnitureSalesDetailAssembler)
        {
            _furnitureSalesDetailRepo = furnitureSalesDetailRepo;
            _furnitureSalesDetailAssembler = furnitureSalesDetailAssembler;
        }

        public void save(List<FurnitureSalesDetailDto> sales_detail_dtos)
        {
            // Runs inside the caller's real EF transaction (see beginTransaction());
            // the previous ambient TransactionScope was a no-op for EF Core.
            foreach (var sales_detail_dto in sales_detail_dtos)
            {
                var salesDetail = new FurnitureSalesDetail();
                _furnitureSalesDetailAssembler.copy(salesDetail, sales_detail_dto);
                _furnitureSalesDetailRepo.insert(salesDetail);
            }
        }
    }
}
