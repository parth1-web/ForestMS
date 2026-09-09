using LE.Entities.User;
using System.Collections.Generic;
using System.Linq;

namespace LE.Service.Repository.Interface
{
    public interface RoleRepository
    {
        void insert(Role role);
        void update(Role role);
        void delete(Role role);
        List<Role> getAll();
        Role getById(long role_id);
        Role getByName(string role_name);
        IQueryable<Role> getQueryable();
    }
}
