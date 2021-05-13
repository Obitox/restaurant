using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class CartItemRepository : Repository<CartItem>
    {
        public CartItemRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }

        // public async Task<List<CartItem>> GetAllEager(ulong cartId)
        // {
        //     // TODO: Find better solution, at the moment it's not patched to return false in all cases, but if it doesn't connect to the server returns exception
        //     if (_fastfood_dbContext.Database.CanConnect())
        //         return await _fastfood_dbContext.CartItem
        //         .Where(cartItem => cartItem.CartId == cartId)
        //         .Include(cartItem => cartItem.Item)
        //         .Include(cartItem => cartItem.Portion)
        //         .ToListAsync();
        //     return new List<CartItem>();
        // }
    }
}
