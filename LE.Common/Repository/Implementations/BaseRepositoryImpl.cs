using LE.Common.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

		// All repositories in this application share one scoped DbContext and each
		// insert/update calls SaveChanges immediately. The ambient System.Transactions
		// scopes used by callers are no-ops for EF Core (the AmbientTransactionWarning
		// is suppressed), so multi-step money operations need an explicit database
		// transaction on the shared context. Begin it once at the outermost layer;
		// nested calls join the already-open transaction via this no-op placeholder
		// (commit/rollback belong solely to the outermost owner).
		public IDbContextTransaction beginTransaction()
		{
			if (appDbContext.Database.CurrentTransaction != null)
			{
				return NestedDbContextTransaction.Instance;
			}
			return appDbContext.Database.BeginTransaction();
		}

		private sealed class NestedDbContextTransaction : IDbContextTransaction
		{
			public static readonly NestedDbContextTransaction Instance = new NestedDbContextTransaction();

			public Guid TransactionId => Guid.Empty;

			public void Commit() { }
			public void Rollback() { }
			public void Dispose() { }

			// Not part of the EF Core 2.1 interface this project compiles against, but
			// required by the EF Core 3.x interface resolved at runtime; declared as a
			// plain method so it satisfies both.
			public System.Threading.Tasks.ValueTask DisposeAsync() => default;
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
