using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Infrastructure.Models;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
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
