using LE.Billing.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface CounterSalesDetailRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(CounterSalesDetail sales_detail);
        List<CounterSalesDetail> getAll();
        CounterSalesDetail getById(long sales_detail_id);
        List<CounterSalesDetail> getBySalesId(long sales_id);
        IQueryable<CounterSalesDetail> getQueryable();
    }
}
