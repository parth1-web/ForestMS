using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    using Service = Billing.Entities.Service;
    public interface ServiceRepository
    {
        void insert(Service service);
        void update(Service service);
        void delete(Service service);
        List<Service> getAll();
        Service getById(long service_id);
        Service getByName(string service_name);
        IQueryable<Service> getQueryable();
    }
}
