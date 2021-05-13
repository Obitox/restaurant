using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.Domain.Helpers
{
    public static class Crypto
    {
        public static string GenerateCsrfToken()
        {
            byte[] bytes = new byte[40];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                crypto.GetBytes(bytes);

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static string HashPassword(string password, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public static string GenerateEmailConfirmationToken()
        {
            byte[] bytes = new byte[40];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                crypto.GetBytes(bytes);

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        //public static string Create(string value, string salt)
        //{
        //    var valueBytes = KeyDerivation.Pbkdf2(
        //                        password: value,
        //                        salt: Encoding.UTF8.GetBytes(salt),
        //                        prf: KeyDerivationPrf.HMACSHA512,
        //                        iterationCount: 10000,
        //                        numBytesRequested: 256 / 8);

        //    return Convert.ToBase64String(valueBytes);
        //}

        public static bool Validate(string value, string salt, string hash)
                => HashPassword(value, salt) == hash;

        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static IEnumerable<Claim> GetClaims(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return claims;
        }
    }
}
