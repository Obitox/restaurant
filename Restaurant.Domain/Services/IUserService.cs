using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Restaurant.Domain.Models;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.Domain.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(string username, string password, HttpContext context);

        Task<User> Register(User user);

        bool SendConfirmatiomEmail(string name, string email, string confirmLink, string confirmToken, string userId);

        AuthenticateResponse RefreshToken(string username, string refreshToken);
        bool RevokeRefreshToken(string username, string refreshToken);
    }
}
