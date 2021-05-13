using Restaurant.DAL.MySQL.Context;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
