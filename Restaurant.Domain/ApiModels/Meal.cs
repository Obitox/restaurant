using System.Collections.Generic;

namespace Restaurant.Domain.ApiModels
{
    public class Meal
    {
        public ulong MealId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public ICollection<Image> Image { get; set; }
        public virtual ICollection<ItemMeal> ItemMeal { get; set; }
    }
}