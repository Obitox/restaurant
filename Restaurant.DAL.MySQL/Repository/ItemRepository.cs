using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class ItemRepository : Repository<Item>
    {
        public ItemRepository(RestaurantDbContext restaurant) : base(restaurant) { }
    }
}
