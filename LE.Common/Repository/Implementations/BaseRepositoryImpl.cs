using LE.Common.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LE.Common.Repository.Implementations
{
	public class BaseRepositoryImpl<T> : BaseRepository<T> where T : class
	{
		private readonly DbContext appDbContext;
		public BaseRepositoryImpl(DbContext _appDbContext)
		{
			appDbContext = _appDbContext;
		}

		public void delete(T entity)
		{
			appDbContext.Set<T>().Remove(entity);
			appDbContext.SaveChanges();
		}

		public List<T> getAll()
		{
			return appDbContext.Set<T>().ToList();
		}

		public T getById(long id)
		{
			return appDbContext.Set<T>().Find(id);
		}

		public IQueryable<T> getQueryable()
		{
			return appDbContext.Set<T>();
		}

		public void insert(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			appDbContext.Set<T>().Add(entity);
			appDbContext.SaveChanges();
		}

		public void update(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			appDbContext.Set<T>().Update(entity);
			appDbContext.SaveChanges();
		}
	}
}
