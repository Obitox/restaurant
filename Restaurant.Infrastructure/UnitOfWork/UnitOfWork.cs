using Restaurant.Infrastructure.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<Item> _itemRepository;

        public UnitOfWork(fastfood_dbContext fastfood)
        {
            DbContext = fastfood;
        }

        public fastfood_dbContext DbContext { get; private set; }

        public async Task<bool> Commit()
        {
            try
            {
                int _save = await DbContext.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (System.Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
