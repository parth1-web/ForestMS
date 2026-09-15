using LE.Billing.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface WoodBillMemberRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(WoodBillMembers woodBillMember);
        List<WoodBillMembers> getAll();
        WoodBillMembers getById(long wood_bill_member_id);
        IQueryable<WoodBillMembers> getQueryable();
    }
}
