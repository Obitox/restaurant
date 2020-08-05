using Restaurant.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Helpers
{
    public static class ExtensionMethods
    {
        public static User WithoutPasswordAndSalt(this User user)
        {
            user.Password = null;
            user.Salt = null;
            return user;
        }

        public static User Profile(this User user)
        {
            user.Password = null;
            user.Salt = null;
            user.Role = null;
            user.RequestAntiForgeryToken = null;
            return user;
        }

        public static IEnumerable<Claim> GetClaims(this User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("CSRF", user.RequestAntiForgeryToken),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return claims;
        }

        public static bool IsExpired(this RefreshToken token) => DateTime.UtcNow.Ticks > token.Expiration;
    }
}
