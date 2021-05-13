using System.Collections.Generic;

namespace Restaurant.Domain.ApiModels
{
    public class Ingredient
    {
        public ulong IngredientId { get; set; }
        public string Title { get; set; }
        public string Allergens { get; set; }
        public bool? IsBase { get; set; }
        public virtual ICollection<ItemIngredient> ItemIngredient { get; set; }
    }
}