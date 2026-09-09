using LE.Billing.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FurnitureSalesDetailRepository
    {
        void insert(FurnitureSalesDetail sales_detail);
        List<FurnitureSalesDetail> getAll();
        FurnitureSalesDetail getById(long sales_detail_id);
        List<FurnitureSalesDetail> getBySalesId(long sales_id);
        IQueryable<FurnitureSalesDetail> getQueryable();
    }
}
