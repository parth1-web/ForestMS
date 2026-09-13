using Microsoft.EntityFrameworkCore.Storage;
using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface WoodBillRepository
    {
        // Real transaction boundary on the shared AppDbContext (see BaseRepositoryImpl).
        IDbContextTransaction beginTransaction();
        void insert(WoodBill woodBill);
        void update(WoodBill woodBill);
        void delete(WoodBill woodBill);
        List<WoodBill> getAll();
        WoodBill getById(long wood_bill_id);
        IQueryable<WoodBill> getQueryable();
    }
}

