using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface ChiranSalesDetailRepository
    {
        void insert(ChiranSalesDetail chiranSalesDetail);
        void update(ChiranSalesDetail chiranSalesDetail);
        void delete(ChiranSalesDetail chiranSalesDetail);
        List<ChiranSalesDetail> getAll();
        ChiranSalesDetail getById(long chiran_sales_detail_id);
        IQueryable<ChiranSalesDetail> getQueryable();
    }
}
