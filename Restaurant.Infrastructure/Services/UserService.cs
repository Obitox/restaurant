using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Infrastructure.Cache;
using Restaurant.Infrastructure.Helpers;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.RabbitMQ;
using Restaurant.Infrastructure.Repository;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly Cache.IRedis _redis;
        private readonly IPublisher _publisher;
        private readonly TokenOptions _tokenOptions;
        private readonly SigningConfigurations _signingConfigurations;

        public UserService(IOptions<TokenOptions> tokenOptions, SigningConfigurations signingConfigurations,IUserRepository userRepository, Cache.IRedis redis, IPublisher publisher)
        {
            _signingConfigurations = signingConfigurations;
            _userRepository = userRepository;
            _tokenOptions = tokenOptions.Value;
            _redis = redis;
            _publisher = publisher;
        }

        public AuthenticateResponse Authenticate(string username, string password, HttpContext context)
        {

            User user = _userRepository.Single(u => u.Username == username);

            if (user == null)
                return null;

            bool isValid = Crypto.Validate(password, user.Salt, user.Password);

            if (!isValid)
                return null;

            if (user.IsEmailConfirmed == 0)
                return null;

            // Auth token
            DateTime accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);
            string csrfToken = Crypto.GenerateCSRFToken();
            user.RequestAntiForgeryToken = csrfToken;

            JwtSecurityToken securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: user.GetClaims(),
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: _signingConfigurations.SigningCredentials
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string authToken = handler.WriteToken(securityToken);


            DateTime refreshTokenExpirationDateTime = DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration);

            RefreshToken refreshToken = new RefreshToken()
            {
                Token = Crypto.GenerateRefreshToken(),
                Expiration = ((DateTimeOffset)refreshTokenExpirationDateTime).ToUnixTimeSeconds()
                //Expiration = refreshTokenExpirationDateTime.Ticks
            };

            string key = $"user:{user.UserId}";
            HashEntry[] hash = new HashEntry[]
            {
                new HashEntry("AuthToken", authToken),
                new HashEntry("RefreshToken", refreshToken.Token),
                new HashEntry("CsrfToken", csrfToken)
            };
            _redis.SetHash(key, hash);


            context.Response.Cookies.Append(
                "AuthToken",
                authToken,
                new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Expires = accessTokenExpiration
                }
            );

            context.Response.Cookies.Append(
                "RefreshToken",
                refreshToken.Token,
                new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Expires = refreshTokenExpirationDateTime
                }
            );

            AuthenticateResponse authenticateResponse = new AuthenticateResponse()
            {
                Username = user.Username,
                AuthToken = authToken,
                RefreshToken = refreshToken.Token,
                CSRFToken = csrfToken,
                ExpiresAt = ((DateTimeOffset)accessTokenExpiration).ToUnixTimeSeconds()
            };

            return authenticateResponse;
        }

        public AuthenticateResponse RefreshToken(string username, string refreshToken)
        {
                User user = _userRepository.Single(u => u.Username == username);

                if (user == null)
                    return null;

                string refreshTokenRedis = _redis.GetHashField($"user:{user.UserId.ToString()}", "RefreshToken");

                if (refreshToken != refreshTokenRedis)
                    return null;

                DateTime accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);
                string csrfToken = Crypto.GenerateCSRFToken();
                user.RequestAntiForgeryToken = csrfToken;

                JwtSecurityToken securityToken = new JwtSecurityToken
                (
                    issuer: _tokenOptions.Issuer,
                    audience: _tokenOptions.Audience,
                    claims: user.GetClaims(),
                    expires: accessTokenExpiration,
                    notBefore: DateTime.UtcNow,
                    signingCredentials: _signingConfigurations.SigningCredentials
                );

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string authToken = handler.WriteToken(securityToken);

                DateTime refreshTokenExpirationDateTime = DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration);

                RefreshToken refreshedToken = new RefreshToken()
                {
                    Token = Crypto.GenerateRefreshToken(),
                    Expiration = ((DateTimeOffset)refreshTokenExpirationDateTime).ToUnixTimeSeconds()
                };

                string key = $"user:{user.UserId}";
                HashEntry[] hash = new HashEntry[]
                {
                new HashEntry("AuthToken", authToken),
                new HashEntry("RefreshToken", refreshedToken.Token),
                new HashEntry("CsrfToken", csrfToken)
                };
                _redis.SetHash(key, hash);

                AuthenticateResponse response = new AuthenticateResponse()
                {
                    Username = user.Username,
                    AuthToken = authToken,
                    RefreshToken = refreshedToken.Token,
                    CSRFToken = csrfToken,
                    ExpiresAt = ((DateTimeOffset)accessTokenExpiration).ToUnixTimeSeconds()
                };

                return response;
        }

        public async Task<User> Register(User user)
        {
            string salt = Crypto.CreateSalt();
            string hash = Crypto.HashPassword(user.Password, salt);

            user.Password = hash;
            user.Salt = salt;

            bool isCreated = await _userRepository.Add(user);

            if (isCreated)
                return await _userRepository.GetUserByEmail(user.Email);

            return null;
        }

        public bool RevokeRefreshToken(string username, string refreshToken)
        {
            ulong userId = _userRepository.Single(u => u.Username == username).UserId;

            if (userId == 0)
                return false;

            string refreshTokenRedis = _redis.GetHashField($"user:{userId.ToString()}", "RefreshToken");

            if (refreshToken != refreshTokenRedis)
                return false;

            bool isRevoked = _redis.DelHash($"user:{userId}");

            return isRevoked;
        }

        public bool SendConfirmatiomEmail(string name, string email, string confirmLink, string confirmToken, string userId)
        {
            bool isSent = false;
            bool isSet = _redis.SetString(userId, confirmToken);

            if (isSet)
                isSent = _publisher.PublishConfirmationEmailMessage(name, email, confirmLink);

            return isSent;
        }
    }
}
