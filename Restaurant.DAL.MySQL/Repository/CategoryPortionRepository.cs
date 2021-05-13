using Restaurant.DAL.MySQL.Context;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CategoryPortionRepository : Repository<CategoryPortion>
    {
        public CategoryPortionRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}