using LE.Billing.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface WoodBillMemberTransactionRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(WoodBillMemberTransaction woodBillMemberTransaction);
        void update(WoodBillMemberTransaction woodBillMemberTransaction);
        List<WoodBillMemberTransaction> getAll();
        WoodBillMemberTransaction getById(long wood_bill_member_transaction_id);
        IQueryable<WoodBillMemberTransaction> getQueryable();
    }
}
