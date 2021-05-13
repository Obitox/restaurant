using Restaurant.DAL.MySQL.Context;
using System.Collections.Generic;
using System.Linq;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
