using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }

        public IEnumerable<Category> GetCategories()
        {
            // TODO: Find better solution, at the moment it's not patched to return false in all cases, but if it doesn't connect to the server returns exception
            if (_fastfood_dbContext.Database.CanConnect())
                return _fastfood_dbContext.Category.ToList();

            return new List<Category>();
        }
    }
}
