using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.DAL.MySQL.Repository
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(RestaurantDbContext restaurant) : base(restaurant)
        {
        }

        // public async Task<User> GetUserByEmail(string email)
        // {
        //     return await _fastfood_dbContext.User.Where(u => u.Email == email).FirstOrDefaultAsync();
        // }

        public async Task<bool> UpdateUserEmailConfirmedByEntityId(ulong id)
        {
            if (id == 0)
                return false;

            RestaurantDbContext.User.FirstOrDefault(u => u.UserId == id).IsEmailConfirmed = 1;
            int updated = await RestaurantDbContext.SaveChangesAsync();

            return updated > 0;
        }
    }
}
