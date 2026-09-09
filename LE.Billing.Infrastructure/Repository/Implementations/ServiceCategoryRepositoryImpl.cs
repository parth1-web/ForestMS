using LE.Billing.Entities;
using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    public class ServiceCategoryRepositoryImpl : BaseRepositoryImpl<ServiceCategory>, ServiceCategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public ServiceCategoryRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ServiceCategory getByName(string service_category_name)
        {
            return _appDbContext.service_categories.Where(a => a.name == service_category_name).SingleOrDefault();
        }
    }
}
