using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Security.Claims;

namespace Restaurant.API.Filters
{
    public class AuthorizationFilter : IActionFilter
    {
        private readonly IDatabase _redis;

        public AuthorizationFilter(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null) throw new NotImplementedException("Redis instance not found");
            if (connectionMultiplexer.IsConnected)
                _redis = connectionMultiplexer.GetDatabase();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool isPresentAuth = context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues token);
            bool isPresentRefresh = context.HttpContext.Request.Headers.TryGetValue("Refresh", out StringValues refresh);
            if (isPresentAuth && isPresentRefresh)
            {
                string csrfToken = context.HttpContext.User.Claims.Where(c => c.Type == "CSRF").FirstOrDefault().Value;
                string userId = context.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;

                if(string.IsNullOrEmpty(userId))
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized"
                    };

                HashEntry[] hash = _redis.HashGetAll($"user:{userId}");

                string authToken = "";
                string refreshToken = "";
                string csrf = ""; 

                if(hash.Length > 0)
                {
                    refreshToken = hash[0].Value;
                    csrf = hash[1].Value;
                    authToken = hash[2].Value;
                }

                if (string.IsNullOrEmpty(csrfToken.ToString()))
                    if(csrfToken.Equals(csrf))
                        context.Result = new ContentResult()
                        {
                            Content = "Unauthorized"
                        };

                if (token.ToString().Length > 7) token = token.ToString()[7..];

                if (token != authToken)
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized"
                    };

                if(refreshToken != refresh)
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized"
                    };
            }
        }
    }
}
