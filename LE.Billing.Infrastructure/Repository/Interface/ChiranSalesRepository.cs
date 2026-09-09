using LE.Billing.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface ChiranSalesRepository
    {
        void insert(ChiranSales chiran_sales_id);
        void update(ChiranSales chiran_sales_id);
        void delete(ChiranSales chiran_sales_id);
        List<ChiranSales> getAll();
        ChiranSales getById(long chiran_sales_id);
        IQueryable<ChiranSales> getQueryable();
    }
}
