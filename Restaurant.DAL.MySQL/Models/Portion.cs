using System;
using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Portion
    {
        public Portion()
        {
            CartItem = new HashSet<CartItem>();
            CategoryPortion = new HashSet<CategoryPortion>();
        }

        public ulong PortionId { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal MassCalorieMultiplier { get; set; }
        public string Title { get; set; }

        public virtual ICollection<CartItem> CartItem { get; set; }
        public virtual ICollection<CategoryPortion> CategoryPortion { get; set; }
    }
}
