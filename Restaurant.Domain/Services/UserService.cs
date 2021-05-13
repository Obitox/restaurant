using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Restaurant.Domain.Helpers;
using Restaurant.Domain.RabbitMQ;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Repository;
using Restaurant.Domain.Models;
using Restaurant.DAL.MySQL.Models;

namespace Restaurant.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IPublisher _publisher;
        private readonly TokenOptions _tokenOptions;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IDatabase _redis;

        public UserService(IOptions<TokenOptions> tokenOptions, SigningConfigurations signingConfigurations, UserRepository userRepository, IConnectionMultiplexer connectionMultiplexer, IPublisher publisher)
        {
            _signingConfigurations = signingConfigurations ?? throw new NotImplementedException("Signing configuration not implemented");
            _userRepository = userRepository ?? throw new NotImplementedException("User repository not implemented");
            _tokenOptions = tokenOptions.Value;
            _publisher = publisher ?? throw new NotImplementedException("Publisher not implemented");
            if (connectionMultiplexer == null) throw new NotImplementedException("Redis instance not found");
            if (connectionMultiplexer.IsConnected) _redis = connectionMultiplexer.GetDatabase();
        }

        public AuthenticateResponse Authenticate(string username, string password, HttpContext context)
        {

            var user = _userRepository.Single(u => u.Username == username);

            if (user == null)
                return null;

            var isValid = Crypto.Validate(password, user.Salt, user.Password);

            if (!isValid)
                return null;

            if (user.IsEmailConfirmed == 0)
                return null;

            // Auth token
            var accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);
            var csrfToken = Crypto.GenerateCsrfToken();
            user.RequestAntiForgeryToken = csrfToken;

            var securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: user.GetClaims(),
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: _signingConfigurations.SigningCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var authToken = handler.WriteToken(securityToken);


            var refreshTokenExpirationDateTime = DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration);

            var refreshToken = new RefreshToken()
            {
                Token = Crypto.GenerateRefreshToken(),
                Expiration = ((DateTimeOffset)refreshTokenExpirationDateTime).ToUnixTimeSeconds()
                //Expiration = refreshTokenExpirationDateTime.Ticks
            };

            var key = $"user:{user.UserId}";
            var hash = new[]
            {
                new HashEntry("AuthToken", authToken),
                new HashEntry("RefreshToken", refreshToken.Token),
                new HashEntry("CsrfToken", csrfToken)
            };
            _redis.HashSet(key, hash);


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

            var authenticateResponse = new AuthenticateResponse()
            {
                Username = user.Username,
                AuthToken = authToken,
                RefreshToken = refreshToken.Token,
                CsrfToken = csrfToken,
                ExpiresAt = ((DateTimeOffset)accessTokenExpiration).ToUnixTimeSeconds()
            };

            return authenticateResponse;
        }

        public AuthenticateResponse RefreshToken(string username, string refreshToken)
        {
            var user = _userRepository.Single(u => u.Username == username);

            if (user == null)
                return null;

            string refreshTokenRedis = _redis.HashGet($"user:{user.UserId.ToString()}", "RefreshToken");

            if (refreshToken != refreshTokenRedis)
                return null;

            var accessTokenExpiration = DateTime.UtcNow.AddSeconds(_tokenOptions.AccessTokenExpiration);
            var csrfToken = Crypto.GenerateCsrfToken();
            user.RequestAntiForgeryToken = csrfToken;

            var securityToken = new JwtSecurityToken
            (
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: user.GetClaims(),
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: _signingConfigurations.SigningCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var authToken = handler.WriteToken(securityToken);

            var refreshTokenExpirationDateTime = DateTime.UtcNow.AddSeconds(_tokenOptions.RefreshTokenExpiration);

            var refreshedToken = new RefreshToken()
            {
                Token = Crypto.GenerateRefreshToken(),
                Expiration = ((DateTimeOffset)refreshTokenExpirationDateTime).ToUnixTimeSeconds()
            };

            var key = $"user:{user.UserId}";
            var hash = new[]
            {
                new HashEntry("AuthToken", authToken),
                new HashEntry("RefreshToken", refreshedToken.Token),
                new HashEntry("CsrfToken", csrfToken)
            };
            _redis.HashSet(key, hash);

            var response = new AuthenticateResponse()
            {
                Username = user.Username,
                AuthToken = authToken,
                RefreshToken = refreshedToken.Token,
                CsrfToken = csrfToken,
                ExpiresAt = ((DateTimeOffset)accessTokenExpiration).ToUnixTimeSeconds()
            };

            return response;
        }

        public async Task<User> Register(User user)
        {
            var salt = Crypto.CreateSalt();
            var hash = Crypto.HashPassword(user.Password, salt);

            user.Password = hash;
            user.Salt = salt;

            var isCreated = await _userRepository.Add(user);

            if (isCreated)
                return await _userRepository.SingleAsync(u => u.Email == user.Email);

            return null;
        }

        public bool RevokeRefreshToken(string username, string refreshToken)
        {
            var userId = _userRepository.Single(u => u.Username == username).UserId;

            if (userId == 0)
                return false;

            string refreshTokenRedis = _redis.HashGet($"user:{userId.ToString()}", "RefreshToken");

            if (refreshToken != refreshTokenRedis)
                return false;

            var isRevoked = _redis.KeyDelete($"user:{userId}");

            return isRevoked;
        }

        public bool SendConfirmatiomEmail(string name, string email, string confirmLink, string confirmToken, string userId)
        {
            var isSent = false;
            var isSet = _redis.StringSet(userId, confirmToken);

            if (isSet)
                isSent = _publisher.PublishConfirmationEmailMessage(name, email, confirmLink);

            return isSent;
        }
    }
}
