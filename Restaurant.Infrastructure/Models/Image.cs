using System;
using System.Collections.Generic;

namespace Restaurant.Infrastructure.Models
{
    public partial class Image
    {
        public ulong ImageId { get; set; }
        public string Path { get; set; }
        public ulong? ItemId { get; set; }
        public ulong? MealId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Meal Meal { get; set; }
    }
}
