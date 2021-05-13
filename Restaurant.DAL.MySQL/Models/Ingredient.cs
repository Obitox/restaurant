using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            ItemIngredient = new HashSet<ItemIngredient>();
        }

        public ulong IngredientId { get; set; }
        public string Title { get; set; }
        public string Allergens { get; set; }
        public bool? IsBase { get; set; }

        public virtual ICollection<ItemIngredient> ItemIngredient { get; set; }
    }
}
