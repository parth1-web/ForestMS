using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class CounterSalesRepositoryImpl : BaseRepositoryImpl<CounterSales>, CounterSalesRepository
    {
        private readonly AppDbContext _appDbContext;

        public CounterSalesRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<CounterSales> getSalesWithinDate(DateTime start_date, DateTime end_date)
        {
            return _appDbContext.counter_sales.Where(a => a.sales_date.Date >= start_date.Date && a.sales_date.Date <= end_date.Date).ToList();
        }

        public List<CounterSales> getSalesOnDate(DateTime start_date)
        {
            return _appDbContext.counter_sales.Where(a => a.sales_date.Date == start_date.Date && a.is_cancelled == false).ToList();
        }
    }
}
