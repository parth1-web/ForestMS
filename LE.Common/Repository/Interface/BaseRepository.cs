using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LE.Common.Repository.Interface
{
    public interface BaseRepository<T>
    {
        void delete(T entity);
        void insert(T entity);
        void update(T entity);
        void saveChanges();
        List<T> getAll();
        T getById(long id);
        IQueryable<T> getQueryable();
        IDbContextTransaction beginTransaction();
    }
}
