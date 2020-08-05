using System;
using System.Collections.Generic;

namespace Restaurant.Infrastructure.Models
{
    public partial class Portion
    {
        public Portion()
        {
            CategoryPortion = new HashSet<CategoryPortion>();
        }

        public ulong PortionId { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal MassCalorieMultiplier { get; set; }
        public string Title { get; set; }

        public virtual ICollection<CategoryPortion> CategoryPortion { get; set; }
    }
}
