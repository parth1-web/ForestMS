using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class WoodDetailsRepositoryImpl : BaseRepositoryImpl<WoodDetails>, WoodDetailsRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodDetailsRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<WoodDetails> getByGoliaNo(string golia_no, string year)
        {
            // P1/B10 fix: was 'goliya_number.Contains(golia_no)' (LIKE %..%) which matched
            // substrings, e.g. golia 12 matched 112. Exact equality is intended for the
            // duplicate check.
            return _appDbContext.wood_details.Where(a => a.goliya_number == golia_no && a.year == year).ToList();
        }

        public int markSoldIfNotSold(List<long> woodDetailsIds, long salesId)
        {
            // P1/B1 fix: the double-selling guard must be atomic. A conditional UPDATE
            // claiming only rows that are still unsold; if another bill just sold the
            // same log, this affects 0 rows for that log and the caller rejects the bill.
            // Runs against the shared context so it participates in the caller's
            // explicit transaction and sees prior uncommitted writes.
            var ids = woodDetailsIds.Distinct().ToList();
            if (ids.Count == 0)
            {
                return 0;
            }

            // A pending tracked entity would be overwritten by the raw UPDATE on
            // SaveChanges, so flush any dirty state first; the rows we claim here
            // are not otherwise being edited on this request.
            _appDbContext.SaveChanges();

            var idList = string.Join(", ", ids);
            return _appDbContext.Database.ExecuteSqlRaw(
                $"UPDATE wood_details SET is_sold = true, sales_id = {salesId} WHERE wood_details_id IN ({idList}) AND is_sold = false");
        }
    }
}
