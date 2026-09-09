using DateConverter.Core.Service_Factory;
using LE.Billing.Entities;
using LE.Billing.Infrastructure.Dto;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Billing.Service.Assemblers.Interface;
using LE.Billing.Service.Services.Interface;
using LE.Common.Exceptions;
using System;
using System.Transactions;

namespace LE.Billing.Service.Services.Implementations
{
    public class CounterSalesServiceImpl : CounterSalesService
    {
        private readonly CounterSalesRepository _counterSalesRepo;
        private readonly DayCloseRepository _dayCloseRepository;
        private readonly CounterSalesAssembler _counterSalesAssembler;
        private readonly CounterSalesDetailService _counterSalesDetailService;

        public CounterSalesServiceImpl(CounterSalesRepository counterSalesRepo, CounterSalesAssembler counterSalesAssembler, CounterSalesDetailService counterSalesDetailService, DayCloseRepository dayCloseRepository)
        {
            _counterSalesAssembler = counterSalesAssembler;
            _counterSalesRepo = counterSalesRepo;
            _counterSalesDetailService = counterSalesDetailService;
            _dayCloseRepository = dayCloseRepository;
        }

        public void cancel(long counter_sales_id, long user_id)
        {
            try
            {
                var sales = _counterSalesRepo.getById(counter_sales_id);
                var IsClosed = _dayCloseRepository.getByDate(sales.sales_date.Date);
                if (IsClosed != null)
                    throw new ItemUsedException("Day is already closed. You cannot cancel this bill.");
                sales.cancelled_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                sales.is_cancelled = true;
                _counterSalesRepo.update(sales);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long makeSales(CounterSalesDto sales_dto)
        {
            try
            {
                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required))
                {
                    sales_dto.sales_date = DateFunctionsFactory.getDateFunctionsService().getDateTimeByTimeZone();
                    var IsClosed = _dayCloseRepository.getByDate(sales_dto.sales_date.Date);
                    if (IsClosed != null)
                    {
                        throw new ItemUsedException("Day is already closed. You cannot perform transactions in this date.");
                    }

                    if (!sales_dto.isDiscountAmountValid())
                    {
                        throw new InvalidValueException("Discount amount is not valid.");
                    }

                    if (!sales_dto.isNetTotalValid())
                    {
                        throw new InvalidValueException("Net total is not valid.");
                    }

                    var sales = new CounterSales();
                    _counterSalesAssembler.copy(sales, sales_dto);
                    _counterSalesRepo.insert(sales);

                    sales_dto.counter_sales_details.ForEach(a => a.sales_id = sales.sales_id);
                    _counterSalesDetailService.save(sales_dto.counter_sales_details);

                    tx.Complete();
                    return sales.sales_id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
