using LE.Billing.Infrastructure.Repository.Interface;
using LE.Common.Repository.Implementations;
using LE.Context.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Implementations
{
    using Service = Billing.Entities.Service;
    public class ServiceRepositoryImpl : BaseRepositoryImpl<Service>, ServiceRepository
    {
        private readonly AppDbContext _appDbContext;

        public ServiceRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Service getByName(string service_name)
        {
            return _appDbContext.services.Where(a => a.name == service_name).SingleOrDefault();
        }
    }
}
