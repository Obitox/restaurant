using System.Collections.Generic;

namespace Restaurant.DAL.MySQL.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartItem = new HashSet<CartItem>();
            CartMeal = new HashSet<CartMeal>();
            Order = new HashSet<Order>();
        }

        public ulong CartId { get; set; }
        public ulong UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<CartItem> CartItem { get; set; }
        public virtual ICollection<CartMeal> CartMeal { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}
