using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface ToleRepository
    {
        void insert(Tole tole);
        void update(Tole tole);
        void delete(Tole tole);
        List<Tole> getAll();
        Tole getById(long tole_id);
        Tole getByToleNo(string tole_no);
        IQueryable<Tole> getQueryable();
    }
}
