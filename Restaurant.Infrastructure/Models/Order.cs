using System;
using System.Collections.Generic;

namespace Restaurant.Infrastructure.Models
{
    public partial class Order
    {
        public ulong OrderId { get; set; }
        public TimeSpan DeliveryAt { get; set; }
        public string IsAccepted { get; set; }
        public byte IsCanceled { get; set; }
        public byte IsDelivered { get; set; }
        public uint? Rating { get; set; }
        public string PersonalPreference { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan? DeliveredAt { get; set; }
        public ulong UserId { get; set; }
        public ulong CartId { get; set; }
        public decimal Price { get; set; }
        public string PaymentOption { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual User User { get; set; }
    }
}
