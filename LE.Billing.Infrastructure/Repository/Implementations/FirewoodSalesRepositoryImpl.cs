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
    public class FirewoodSalesRepositoryImpl : BaseRepositoryImpl<FirewoodSales>, FirewoodSalesRepository
    {
        private readonly AppDbContext _appDbContext;

        public FirewoodSalesRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<FirewoodSales> getSalesWithinDate(DateTime start_date, DateTime end_date)
        {
            return _appDbContext.firewood_sales.Where(a => a.sales_date.Date >= start_date.Date && a.sales_date.Date <= end_date.Date).ToList();
        }

        public List<FirewoodSales> getSalesOnDate(DateTime start_date)
        {
            return _appDbContext.firewood_sales.Where(a => a.sales_date.Date == start_date.Date).ToList();
        }
    }
}
