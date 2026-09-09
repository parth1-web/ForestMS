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
    public class CounterSalesDetailRepositoryImpl : BaseRepositoryImpl<CounterSalesDetail>, CounterSalesDetailRepository
    {
        private readonly AppDbContext _appDbContext;

        public CounterSalesDetailRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<CounterSalesDetail> getBySalesId(long sales_id)
        {
            return _appDbContext.counter_sales_details.Where(a => a.sales_id == sales_id).ToList();
        }

    }
}
