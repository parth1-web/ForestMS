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
    public class FirewoodSalesDetailRepositoryImpl : BaseRepositoryImpl<FirewoodSalesDetail>, FirewoodSalesDetailRepository
    {
        private readonly AppDbContext _appDbContext;

        public FirewoodSalesDetailRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<FirewoodSalesDetail> getBySalesId(long sales_id)
        {
            return _appDbContext.firewood_sales_detail.Where(a => a.firewood_sales_id == sales_id).ToList();
        }

    }
}
