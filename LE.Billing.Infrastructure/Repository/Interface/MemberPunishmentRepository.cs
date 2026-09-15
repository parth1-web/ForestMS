using LE.Billing.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Billing.Infrastructure.Repository.Interface
{
    public interface MemberPunishmentRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(MemberPunishment memPunishment);
        void update(MemberPunishment memPunishment);
        void delete(MemberPunishment memPunishment);
        List<MemberPunishment> getAll();
        MemberPunishment getById(long memPunishment_id);
        IQueryable<MemberPunishment> getQueryable();
    }
}
