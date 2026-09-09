using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface CounterSalesDetailRepository
    {
        void insert(CounterSalesDetail sales_detail);
        List<CounterSalesDetail> getAll();
        CounterSalesDetail getById(long sales_detail_id);
        List<CounterSalesDetail> getBySalesId(long sales_id);
        IQueryable<CounterSalesDetail> getQueryable();
    }
}
