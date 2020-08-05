using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Repository;
using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }

        public async Task<List<Ingredient>> GetAllIngredients()
        {
            return await _fastfood_dbContext.Ingredient.Include(i => i.ItemIngredient).ToListAsync();
        }

        public async Task<List<Ingredient>> GetAllIngredientsByItemId(ulong itemId)
        {
            var ingredients = new List<Ingredient>();
            var allIngredients = await _fastfood_dbContext.Ingredient.Include(i => i.ItemIngredient).ToListAsync();

            foreach (Ingredient ingredient in allIngredients)
                foreach (ItemIngredient itemIngredient in ingredient.ItemIngredient)
                    if (itemIngredient.ItemId == itemId)
                        ingredients.Add(ingredient);

            return ingredients;

        }
    }
}
