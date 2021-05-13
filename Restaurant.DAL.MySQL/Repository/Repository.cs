using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Restaurant.DAL.MySQL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly RestaurantDbContext RestaurantDbContext;
        private readonly DbSet<T> _entities;

        public Repository(RestaurantDbContext restaurant)
        {
            RestaurantDbContext = restaurant;
            _entities = restaurant.Set<T>();
        }

        public async Task<bool> Add(T entity)
        {
            if (entity == null)
                return false;

            await _entities.AddAsync(entity);

            var created = await RestaurantDbContext.SaveChangesAsync();

            return created > 0;
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

        public virtual async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            if(predicate == null && includes == null)
                return await _entities.ToListAsync();
            
            var query = _entities.AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                {
                    if (include.Body is MemberExpression memberExpression)
                        query = query.Include(memberExpression.Member.Name);
                }

            if(predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
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
            return _entities.SingleOrDefault(predicate);
        }

        public void Update(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var entityFound = await _entities.FirstOrDefaultAsync(predicate);
            _entities.Update(entityFound);
            return await (RestaurantDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true)
        {
            return await _entities.SingleOrDefaultAsync(predicate);
        }
    }
}
