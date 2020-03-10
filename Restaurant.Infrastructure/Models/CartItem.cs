using System;
using System.Collections.Generic;

namespace Restaurant.Infrastructure.Models
{
    public partial class CartItem
    {
        public ulong CartItemId { get; set; }
        public ulong CartId { get; set; }
        public ulong ItemId { get; set; }
        public uint Amount { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Item Item { get; set; }
    }
}
