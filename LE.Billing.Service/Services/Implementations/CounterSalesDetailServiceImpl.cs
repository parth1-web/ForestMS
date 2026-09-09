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
    public class CounterSalesDetailServiceImpl:CounterSalesDetailService
    {
        private readonly CounterSalesDetailRepository _counterSalesDetailRepo;
        private readonly CounterSalesDetailAssembler _counterSalesDetailAssembler;

        public CounterSalesDetailServiceImpl(CounterSalesDetailRepository counterSalesDetailRepo, CounterSalesDetailAssembler counterSalesDetailAssembler)
        {
            _counterSalesDetailAssembler = counterSalesDetailAssembler;
            _counterSalesDetailRepo = counterSalesDetailRepo;
        }

        public void save(List<CounterSalesDetailDto> sales_detail_dtos)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var sales_detail_dto in sales_detail_dtos)
                    {
                        var salesDetail = new CounterSalesDetail();
                        _counterSalesDetailAssembler.copy(salesDetail, sales_detail_dto);
                        _counterSalesDetailRepo.insert(salesDetail);
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
