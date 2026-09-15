using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    using Service = Billing.Entities.Service;
    public interface ServiceRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(Service service);
        void update(Service service);
        void delete(Service service);
        List<Service> getAll();
        Service getById(long service_id);
        Service getByName(string service_name);
        IQueryable<Service> getQueryable();
    }
}
