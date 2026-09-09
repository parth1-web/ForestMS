using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FirewoodSalesRepository
    {
        void insert(FirewoodSales counterSales);
        void update(FirewoodSales counterSales);
        List<FirewoodSales> getAll();
        FirewoodSales getById(long sales_id);
        IQueryable<FirewoodSales> getQueryable();
        List<FirewoodSales> getSalesWithinDate(DateTime start_date, DateTime end_date);
        List<FirewoodSales> getSalesOnDate(DateTime start_date);
    }
}
