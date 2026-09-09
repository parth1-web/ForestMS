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
    public class FirewoodSalesDetailServiceImpl:FirewoodSalesDetailService
    {
        private readonly FirewoodSalesDetailRepository _firewoodSalesDetailRepo;
        private readonly FirewoodSalesDetailAssembler _firewoodSalesDetailAssembler;

        public FirewoodSalesDetailServiceImpl(FirewoodSalesDetailRepository counterSalesDetailRepo, FirewoodSalesDetailAssembler counterSalesDetailAssembler)
        {
            _firewoodSalesDetailAssembler = counterSalesDetailAssembler;
            _firewoodSalesDetailRepo = counterSalesDetailRepo;
        }

        public void save(List<FirewoodSalesDetailDto> sales_detail_dtos)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var sales_detail_dto in sales_detail_dtos)
                    {
                        var salesDetail = new FirewoodSalesDetail();
                        _firewoodSalesDetailAssembler.copy(salesDetail, sales_detail_dto);
                        _firewoodSalesDetailRepo.insert(salesDetail);
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
