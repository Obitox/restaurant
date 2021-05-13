using System.Collections.Generic;

namespace Restaurant.Domain.ApiModels
{
    public class Home
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Meal> Meals { get; set; }
        public IEnumerable<Ingredient> Ingredients { get; set; }
    }
}