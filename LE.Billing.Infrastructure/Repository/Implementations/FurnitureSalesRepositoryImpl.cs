using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class FurnitureSalesRepositoryImpl : BaseRepositoryImpl<FurnitureSales>, FurnitureSalesRepository
    {
        private readonly AppDbContext _appDbContext;

        public FurnitureSalesRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<FurnitureSales> getSalesWithinDate(DateTime start_date, DateTime end_date)
        {
            return _appDbContext.furniture_sales.Where(a => a.sales_date.Date >= start_date.Date && a.sales_date.Date <= end_date.Date).ToList();
        }

        public List<FurnitureSales> getSalesOnDate(DateTime start_date)
        {
            return _appDbContext.furniture_sales.Where(a => a.sales_date.Date == start_date.Date).ToList();
        }
    }
}
