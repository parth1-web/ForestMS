using LE.Billing.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface ServiceCategoryRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(ServiceCategory service_category);
        void update(ServiceCategory service_category);
        void delete(ServiceCategory service_category);
        List<ServiceCategory> getAll();
        ServiceCategory getById(long service_category_id);
        ServiceCategory getByName(string service_category_name);
        IQueryable<ServiceCategory> getQueryable();
    }
}
