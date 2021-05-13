using System;
using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class ItemIngredient
    {
        public ulong ItemIngredientId { get; set; }
        public ulong IngredientId { get; set; }
        public ulong ItemId { get; set; }

        public virtual Ingredient Ingredient { get; set; }
        public virtual Item Item { get; set; }
    }
}
