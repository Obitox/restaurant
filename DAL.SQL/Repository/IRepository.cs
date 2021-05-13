using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public interface IRepository<T> 
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);
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

        Task<bool> Update(T entity, Expression<Func<T, bool>> predicate = null);
        void Update(params T[] entities);
        void Update(IEnumerable<T> entities);
    }
}
