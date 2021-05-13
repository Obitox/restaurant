using Restaurant.DAL.MySQL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.Templates;

namespace Restaurant.Domain.Services
{
    public interface IOrderService
    {
        Task<OrderView> Process(ulong userId, int paymentMethod, List<Item> cartItems, List<Meal> cartMeals, decimal price);
        Task<object> OrderEmailView(ulong cartId, ulong orderId);
        //public Order Process(ulong userId, int paymentMethod, List<Tuple<ulong, string, Tuple<ulong, string, decimal, decimal, decimal>, string, int>> items, List<Tuple<ulong, string, decimal, string, int>> meals, decimal price);
    }
}
