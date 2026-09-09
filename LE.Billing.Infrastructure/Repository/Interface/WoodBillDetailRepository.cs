using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface WoodBillDetailRepository
    {
        void insert(WoodBillDetail woodBillDetail);
        void update(WoodBillDetail woodBillDetail);
        void delete(WoodBillDetail woodBillDetail);
        List<WoodBillDetail> getAll();
        WoodBillDetail getById(long wood_bill_id);
        IQueryable<WoodBillDetail> getQueryable();
    }
}
