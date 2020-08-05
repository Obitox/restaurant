using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        Task<List<Ingredient>> GetAllIngredientsByItemId(ulong itemId);
        Task<List<Ingredient>> GetAllIngredients();
    }
}
