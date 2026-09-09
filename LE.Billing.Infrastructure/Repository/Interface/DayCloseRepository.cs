using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface DayCloseRepository
    {
        void insert(DayClose tole);
        List<DayClose> getAll();
        DayClose getById(long tole_id);
        DayClose getByDate(DateTime date);
        IQueryable<DayClose> getQueryable();
    }
}
