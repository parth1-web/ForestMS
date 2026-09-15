using LE.Entities.User;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface RoleRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(Role role);
        void update(Role role);
        void delete(Role role);
        List<Role> getAll();
        Role getById(long role_id);
        Role getByName(string role_name);
        IQueryable<Role> getQueryable();
    }
}
