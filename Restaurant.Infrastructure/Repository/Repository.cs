using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Win32.SafeHandles;
using Restaurant.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly fastfood_dbContext _fastfood_dbContext;
        private readonly DbSet<T> entities;
        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public Repository(fastfood_dbContext fastfood)
        {
            _fastfood_dbContext = fastfood;
            entities = fastfood.Set<T>();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Add(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public void Delete(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 
      
            if (disposing) {
                handle.Dispose();
            }
      
            disposed = true;
        }

        public virtual async Task<List<T>> GetAll(params Expression<Func<T, object>>[] includes)
        {
            return await entities.ToListAsync();
        }

        public IQueryable<T> Query(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public T Search(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public T Single(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
