using Restaurant.DAL.MySQL.Context;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CartMealRepository : Repository<CartMeal>
    {
        public CartMealRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
