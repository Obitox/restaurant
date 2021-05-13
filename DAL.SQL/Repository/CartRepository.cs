using Restaurant.DAL.MySQL.Context;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CartRepository : Repository<Cart>
    {
        public CartRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
