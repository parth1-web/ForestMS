using LE.Billing.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FirewoodSalesDetailRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(FirewoodSalesDetail sales_detail);
        List<FirewoodSalesDetail> getAll();
        FirewoodSalesDetail getById(long sales_detail_id);
        List<FirewoodSalesDetail> getBySalesId(long sales_id);
        IQueryable<FirewoodSalesDetail> getQueryable();
    }
}
