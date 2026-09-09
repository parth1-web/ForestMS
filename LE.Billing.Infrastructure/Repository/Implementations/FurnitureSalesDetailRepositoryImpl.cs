using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System.Collections.Generic;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class FurnitureSalesDetailRepositoryImpl : BaseRepositoryImpl<FurnitureSalesDetail>, FurnitureSalesDetailRepository
    {
        private readonly AppDbContext _appDbContext;

        public FurnitureSalesDetailRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<FurnitureSalesDetail> getBySalesId(long sales_id)
        {
            return _appDbContext.furniture_sales_detail.Where(a => a.furniture_sales_id == sales_id).ToList();
        }

    }
}
