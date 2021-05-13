using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restaurant.Domain.Repository
{
    public interface IRepository<T> 
        // : IDisposable where T : class
    {
        Task<List<T>> GetAll(params Expression<Func<T, object>>[] includes);
        IQueryable<T> Query(string sql, params object[] parameters);
        T Search(params object[] keyValues);
        T Single(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null,
                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                    Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                    bool disableTracking = true);

        Task<bool> Add(T entity);
        void Add(params T[] entities);
        void Add(IEnumerable<T> entities);

        void Delete(T entity);
        void Delete(object id);
        void Delete(params T[] entities);
        void Delete(IEnumerable<T> entities);

        void Update(T entity);
        void Update(params T[] entities);
        void Update(IEnumerable<T> entities);
    }
}
