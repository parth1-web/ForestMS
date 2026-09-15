using LE.Inventory.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Interface
{
    public interface PurchaseRepository
    {
        void saveChanges();
        void insert(Purchase purchase);
        void update(Purchase purchase);
        List<Purchase> getAll();
        Purchase getById(long purchase_id);
        IQueryable<Purchase> getQueryable();
        List<Purchase> getPurchasesWithinDate(DateTime start_date, DateTime end_date);
        List<Purchase> getPurchasesOnDate(DateTime start_date);
        IDbContextTransaction beginTransaction();
    }
}
