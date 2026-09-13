using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Implementations
{
    public class ChiranSalesDetailServiceImpl:ChiranSalesDetailService
    {
        private readonly ChiranSalesDetailRepository _chiranSalesDetailRepo;
        private readonly ChiranSalesDetailAssembler _chiranSalesDetailAssembler;

        public ChiranSalesDetailServiceImpl(ChiranSalesDetailRepository woodBillDetailRepo, ChiranSalesDetailAssembler woodBillDetailAssembler)
        {
            _chiranSalesDetailAssembler = woodBillDetailAssembler;
            _chiranSalesDetailRepo = woodBillDetailRepo;
        }

        public void insert(List<ChiranSalesDetailDto> chiran_sales_detail_dtos)
        {
            // Runs inside the caller's real EF transaction (see beginTransaction());
            // the previous ambient TransactionScope was a no-op for EF Core.
            foreach (var chiran_sales_detail_dto in chiran_sales_detail_dtos)
            {
                var woodBillDetail = new ChiranSalesDetail();
                _chiranSalesDetailAssembler.copy(woodBillDetail, chiran_sales_detail_dto);
                _chiranSalesDetailRepo.insert(woodBillDetail);
            }
        }
    }
}
