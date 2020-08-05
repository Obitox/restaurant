using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using RabbitMQ.Client;
using Restaurant.Infrastructure.RabbitMQ;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Infrastructure.Helpers;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using Restaurant.DAL.MySQL.Repository;
using Restaurant.Infrastructure.Services;
using StackExchange.Redis;
using Restaurant.Infrastructure.Redis;
using RestaurantAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;

namespace RestaurantAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("DEV_POLICY", builder =>
            {
                builder.WithOrigins("https://localhost:30662", "https://login.microsoftonline.com/common/oauth2/v2.0/authorize", "https://login.microsoftonline.com/common")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddControllers(options =>
            {
                   
            }).AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // configure token options
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            TokenOptions tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            // configure rabbitmq settings
            IConfigurationSection rabbitMQSection = Configuration.GetSection("RabbitMQ");
            services.Configure<RabbitMQSettings>(rabbitMQSection);

            // configure jwt authentication
            SigningConfigurations signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jwtBearerOptions =>
                    {
                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = tokenOptions.Issuer,
                            ValidAudience = tokenOptions.Audience,
                            IssuerSigningKey = signingConfigurations.Key,
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            //var appSettings = appSettingsSection.Get<TokenOptions>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //services.AddAuthentication(auth =>
            //{
            //    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(auth =>
            //{
            //    auth.RequireHttpsMetadata = false;
            //    auth.SaveToken = true;
            //    auth.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = true,
            //        ValidateAudience = true
            //    };
            //});

            string connectionString = Configuration.GetConnectionString("MySQL");
            string redisConnectionString = Configuration.GetSection("Redis")["localhost"];

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddSingleton<ConnectionFactory>();

            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMealRepository, MealRepository>();
            services.AddScoped<IPortionRepository, PortionRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<ICartMealRepository, CartMealRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<Restaurant.Infrastructure.Cache.IRedis, Redis>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPublisher, Publisher>();
            services.AddScoped<AuthorizationFilter>();

            services.AddDbContextPool<fastfood_dbContext>(options => options
                // replace with your connection string
                .UseMySql(connectionString, mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new ServerVersion(new Version(8, 0, 12), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql))
            ));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseCors("DEV_POLICY");
            //app.UseCors(builder => builder.WithOrigins("https://localhost:8080").SetIsOriginAllowed((host) => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });





        }
    }
}
