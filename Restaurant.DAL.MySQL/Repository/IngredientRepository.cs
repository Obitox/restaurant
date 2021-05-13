using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class IngredientRepository : Repository<Ingredient>
    {
        public IngredientRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }
    }
}
