using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CartMealRepository : Repository<CartMeal>, ICartMealRepository
    {
        public CartMealRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }
    }
}
