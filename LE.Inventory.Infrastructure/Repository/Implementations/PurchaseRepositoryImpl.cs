using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class PurchaseRepositoryImpl : BaseRepositoryImpl<Purchase>, PurchaseRepository
    {
        private readonly AppDbContext _appDbContext;

        public PurchaseRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<Purchase> getPurchasesWithinDate(DateTime start_date, DateTime end_date)
        {
            return _appDbContext.purchase.Where(a => a.purchase_date.Date >= start_date.Date && a.purchase_date.Date <= end_date.Date).ToList();
        }

        public List<Purchase> getPurchasesOnDate(DateTime start_date)
        {
            return _appDbContext.purchase.Where(a => a.purchase_date.Date == start_date.Date).ToList();
        }
    }
}
