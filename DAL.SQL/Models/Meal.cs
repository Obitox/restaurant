using System;
using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Meal
    {
        public Meal()
        {
            CartMeal = new HashSet<CartMeal>();
            Image = new HashSet<Image>();
            ItemMeal = new HashSet<ItemMeal>();
        }

        public ulong MealId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<CartMeal> CartMeal { get; set; }
        public virtual ICollection<Image> Image { get; set; }
        public virtual ICollection<ItemMeal> ItemMeal { get; set; }
    }
}
