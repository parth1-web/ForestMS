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
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var chiran_sales_detail_dto in chiran_sales_detail_dtos)
                    {
                        var woodBillDetail = new ChiranSalesDetail();
                        _chiranSalesDetailAssembler.copy(woodBillDetail, chiran_sales_detail_dto);
                        _chiranSalesDetailRepo.insert(woodBillDetail);
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
