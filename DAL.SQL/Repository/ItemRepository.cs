using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(fastfood_dbContext fastfood) : base(fastfood) { }

        public async Task<IEnumerable<Item>> GetItems()
        {
            // TODO: Find better solution, at the moment it's not patched to return false in all cases, but if it doesn't connect to the server returns exception
            if (_fastfood_dbContext.Database.CanConnect())
                return await _fastfood_dbContext.Item
                    //.Include(item => item.Category)
                    .ToListAsync();
            return new List<Item>();
        }
    }
}
