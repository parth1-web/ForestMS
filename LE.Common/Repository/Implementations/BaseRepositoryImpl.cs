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
			public System.Threading.Tasks.Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => System.Threading.Tasks.Task.CompletedTask;
			public System.Threading.Tasks.Task RollbackAsync(System.Threading.CancellationToken cancellationToken = default) => System.Threading.Tasks.Task.CompletedTask;
			public System.Threading.Tasks.ValueTask DisposeAsync() => default;
		}

		// Unit-of-work (P2): update/delete no longer SaveChanges by themselves — batched
		// tracked changes are flushed by saveChanges() (single-write operations) or before
		// tx.Commit() (transactional operations). insert() still flushes because PKs are
		// DB-generated (PostgreSQL RETURNING): callers read entity.<pk> immediately after
		// insert to wire up child rows (bill -> details), and deferring would give them 0.
		public void saveChanges()
		{
			appDbContext.SaveChanges();
		}

		public void delete(T entity)
		{
			appDbContext.Set<T>().Remove(entity);
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
			// PKs are DB-generated; flush so callers can read entity.<pk> right away.
			appDbContext.SaveChanges();
		}

		public void update(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			appDbContext.Set<T>().Update(entity);
		}
	}
}
