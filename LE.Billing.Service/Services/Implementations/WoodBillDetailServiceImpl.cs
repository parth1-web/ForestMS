using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;

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
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var wood_bill_detail_dto in wood_bill_detail_dtos)
                    {
                        var woodBillDetail = new WoodBillDetail();
                        _woodBillDetailAssembler.copy(woodBillDetail, wood_bill_detail_dto);
                        _woodBillDetailRepo.insert(woodBillDetail);
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
