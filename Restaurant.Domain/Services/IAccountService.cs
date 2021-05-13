using Restaurant.DAL.MySQL.Models;
using System.Threading.Tasks;

namespace Restaurant.Domain.Services
{
    public interface IAccountService
    {
        Task<bool> ValidateEmailConfirmation(string userId, string confirmToken);
        User Profile(string userId);
    }
}
