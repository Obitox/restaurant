using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Restaurant.Infrastructure.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Filters
{
    public class AuthorizationFilter : IActionFilter
    {
        private readonly Restaurant.Infrastructure.Cache.IRedis _redis;

        public AuthorizationFilter(Restaurant.Infrastructure.Cache.IRedis redis)
        {
            _redis = redis;
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

                HashEntry[] hash = _redis.GetAllHash($"user:{userId}");

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

                token = token.ToString()[7..^(0)];

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
