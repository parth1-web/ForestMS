using LE.Billing.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface MembershipValidityRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(MembershipValidity membershipPeriod);
        void update(MembershipValidity membershipPeriod);
        List<MembershipValidity> getAll();
        MembershipValidity getById(long membership_id);
        IQueryable<MembershipValidity> getQueryable();
    }
}
