using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Item
    {
        public Item()
        {
            CartItem = new HashSet<CartItem>();
            Image = new HashSet<Image>();
            ItemIngredient = new HashSet<ItemIngredient>();
            ItemMeal = new HashSet<ItemMeal>();
        }

        public ulong ItemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public uint Mass { get; set; }
        public uint CalorieCount { get; set; }
        public decimal Price { get; set; }
        public ulong CategoryId { get; set; }
        public string RequestAntiForgeryToken { get; set; }
        public byte IsDeleted { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<CartItem> CartItem { get; set; }
        public virtual ICollection<Image> Image { get; set; }
        public virtual ICollection<ItemIngredient> ItemIngredient { get; set; }
        public virtual ICollection<ItemMeal> ItemMeal { get; set; }
    }
}
