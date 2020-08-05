using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<Order> Process(ulong userId, int paymentMethod, List<Item> cartItems, List<Meal> cartMeals, decimal price);
        //public Order Process(ulong userId, int paymentMethod, List<Tuple<ulong, string, Tuple<ulong, string, decimal, decimal, decimal>, string, int>> items, List<Tuple<ulong, string, decimal, string, int>> meals, decimal price);
    }
}
