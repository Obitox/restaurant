using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> GetItems();
    }
}
