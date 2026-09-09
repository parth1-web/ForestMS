using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FurnitureSalesRepository
    {
        void insert(FurnitureSales counterSales);
        void update(FurnitureSales counterSales);
        List<FurnitureSales> getAll();
        FurnitureSales getById(long sales_id);
        IQueryable<FurnitureSales> getQueryable();
        List<FurnitureSales> getSalesWithinDate(DateTime start_date, DateTime end_date);
        List<FurnitureSales> getSalesOnDate(DateTime start_date);
    }
}
