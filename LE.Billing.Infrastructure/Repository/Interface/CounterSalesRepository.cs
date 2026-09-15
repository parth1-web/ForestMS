using LE.Billing.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface CounterSalesRepository
    {
        void saveChanges();
        // Real transaction boundary on the shared AppDbContext (see BaseRepositoryImpl).
        IDbContextTransaction beginTransaction();
        void insert(CounterSales counterSales);
        void update(CounterSales counterSales);
        List<CounterSales> getAll();
        CounterSales getById(long sales_id);
        IQueryable<CounterSales> getQueryable();
        List<CounterSales> getSalesWithinDate(DateTime start_date, DateTime end_date);
        List<CounterSales> getSalesOnDate(DateTime start_date);
    }
}
