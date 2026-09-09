using LE.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface WoodBillMemberRepository
    {
        void insert(WoodBillMembers woodBillMember);
        List<WoodBillMembers> getAll();
        WoodBillMembers getById(long wood_bill_member_id);
        IQueryable<WoodBillMembers> getQueryable();
    }
}
