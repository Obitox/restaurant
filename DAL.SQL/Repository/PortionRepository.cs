using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class PortionRepository : Repository<Portion>
    {
        public PortionRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
