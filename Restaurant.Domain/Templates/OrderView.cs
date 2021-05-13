using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Templates
{
    public class OrderView
    {
        public ulong OrderId { get; set; }
        public decimal Price { get; set; }
        public string PaymentOption { get; set; }
        public DateTime DeliveryAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<OrderMeal> OrderMeals { get; set; }
    }
}
