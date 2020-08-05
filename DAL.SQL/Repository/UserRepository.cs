using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.DAL.MySQL.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(fastfood_dbContext fastfood) : base(fastfood)
        {
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _fastfood_dbContext.User.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUserEmailConfirmedByEntityId(ulong id)
        {
            if (id == 0)
                return false;

            _fastfood_dbContext.User.Where(u => u.UserId == id).FirstOrDefault().IsEmailConfirmed = 1;
            int updated = await _fastfood_dbContext.SaveChangesAsync();

            return updated > 0;
        }
    }
}
