using LE.Billing.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface FurnitureSalesDetailRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(FurnitureSalesDetail sales_detail);
        List<FurnitureSalesDetail> getAll();
        FurnitureSalesDetail getById(long sales_detail_id);
        List<FurnitureSalesDetail> getBySalesId(long sales_id);
        IQueryable<FurnitureSalesDetail> getQueryable();
    }
}
