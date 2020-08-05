using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class MealRepository : Repository<Meal>, IMealRepository
    {

        public MealRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }

        public async Task<IEnumerable<Meal>> GetMeals()
        {
            if (_fastfood_dbContext.Database.CanConnect())
                return await _fastfood_dbContext.Meal
                                            .Include(meal => meal.Image)
                                            .Include(meal => meal.ItemMeal)
                                                .ThenInclude(itemMeal => itemMeal.Meal)
                                            .Include(meal => meal.ItemMeal)
                                                .ThenInclude(itemMeal => itemMeal.Item)
                                            .ToListAsync();

            return new List<Meal>();
        }
    }
}
