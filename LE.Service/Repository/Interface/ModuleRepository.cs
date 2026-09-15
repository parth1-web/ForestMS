using LE.Entities.User;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Service.Repository.Interface
{
    public interface ModuleRepository
    {
        IDbContextTransaction beginTransaction();
        void saveChanges();
        void insert(Module module);
        void update(Module module);
        void delete(Module module);
        List<Module> getAll();
        Module getById(long area_id);
        Module getByName(string module_name);
        Module getByCode(string module_code);
        IQueryable<Module> getQueryable();
    }
}
