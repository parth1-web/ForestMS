using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System.Collections.Generic;

namespace LE.Billing.Service.Services.Implementations
{
    public class WoodBillDetailServiceImpl:WoodBillDetailService
    {
        private readonly WoodBillDetailRepository _woodBillDetailRepo;
        private readonly WoodBillDetailAssembler _woodBillDetailAssembler;

        public WoodBillDetailServiceImpl(WoodBillDetailRepository woodBillDetailRepo, WoodBillDetailAssembler woodBillDetailAssembler)
        {
            _woodBillDetailAssembler = woodBillDetailAssembler;
            _woodBillDetailRepo = woodBillDetailRepo;
        }

        public void insert(List<WoodBillDetailDto> wood_bill_detail_dtos)
        {
            // Runs inside the caller's real EF transaction (see beginTransaction());
            // the previous ambient TransactionScope was a no-op for EF Core.
            foreach (var wood_bill_detail_dto in wood_bill_detail_dtos)
            {
                var woodBillDetail = new WoodBillDetail();
                _woodBillDetailAssembler.copy(woodBillDetail, wood_bill_detail_dto);
                _woodBillDetailRepo.insert(woodBillDetail);
            }
        }
    }
}
