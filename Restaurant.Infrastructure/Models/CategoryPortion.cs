using System;
using System.Collections.Generic;

namespace Restaurant.Infrastructure.Models
{
    public partial class CategoryPortion
    {
        public ulong CategoryPortionId { get; set; }
        public ulong CategoryId { get; set; }
        public ulong PortionId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Portion Portion { get; set; }
    }
}
