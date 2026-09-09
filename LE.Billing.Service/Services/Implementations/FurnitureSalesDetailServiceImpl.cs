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
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var sales_detail_dto in sales_detail_dtos)
                    {
                        var salesDetail = new FurnitureSalesDetail();
                        _furnitureSalesDetailAssembler.copy(salesDetail, sales_detail_dto);
                        _furnitureSalesDetailRepo.insert(salesDetail);
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
