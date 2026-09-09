using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class DayCloseRepositoryImpl : BaseRepositoryImpl<DayClose>, DayCloseRepository
    {
        private readonly AppDbContext _appDbContext;

        public DayCloseRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public DayClose getByDate(DateTime date)
        {
            return _appDbContext.day_close.Where(a => a.eng_close_date.Date == date.Date).SingleOrDefault();
        }
    }
}
