using System;
using System.Configuration;
using System.Linq;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using RabbitMQ.Client;
using Restaurant.Domain.RabbitMQ;
using Restaurant.DAL.MySQL.Context;
using Restaurant.Domain.Helpers;
using Restaurant.DAL.MySQL.Repository;
using Restaurant.Domain.Services;
using StackExchange.Redis;
using Restaurant.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SimpleInjector;

namespace Restaurant.API
{
    public class Startup
    {
        private readonly Container _container = new Container()
        {
            Options =
            {
                DefaultLifestyle = Lifestyle.Scoped
            }
        };
        
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

            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // configure token options
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            // configure rabbitmq settings
            var rabbitMqSection = Configuration.GetSection("RabbitMQ");
            services.Configure<RabbitMqSettings>(rabbitMqSection);

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

            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation();
                
                // Optional
                // options.AddLogging();
                // options.AddLocalization();
            });
            
            var connectionString = Configuration.GetConnectionString("MySQL");
            services.AddDbContextPool<RestaurantDbContext>(options => options
                // replace with your connection string
                .UseMySql(connectionString, mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new ServerVersion(new Version(8, 0, 12), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql))
            ));

            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var redisConnectionString = Configuration.GetSection("Redis")["localhost"];
            // Manual config
            
            // Mapster
            var config = new TypeAdapterConfig();
            _container.RegisterInstance(config);
            _container.Register<IMapper>(() => new Mapper(config), Lifestyle.Scoped); 
            
            _container.RegisterInstance<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString)); 
            _container.Register<ConnectionFactory>(Lifestyle.Singleton);
            _container.Register<IPublisher, Publisher>(Lifestyle.Scoped);
            
            _container.Register<AuthorizationFilter>(Lifestyle.Scoped);
            
            // Automatic repo register
            var repositoryAssembly = typeof(UserRepository).Assembly;

            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace != null && type.Namespace.StartsWith("Restaurant.DAL.MySQL.Repository")
                                             && type != typeof(IRepository<>)
                select type;                             
                // from service in type.GetInterfaces()
                // select new { service, implementation = type };
            
            foreach (var reg in registrations)
            {
                _container.Register(reg);
            } 
            
            
            // Automatic service config
            var serviceAssembly = typeof(UserService).Assembly;
            
            var serviceRegistrations =
                from type in serviceAssembly.GetExportedTypes()
                where type.Namespace != null && type.Namespace.StartsWith("Restaurant.Domain.Services")
                from service in type.GetInterfaces()
                select new { service, implementation = type };
            

            foreach (var reg in serviceRegistrations)
            {
                _container.Register(reg.service, reg.implementation);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(_container);
            
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

            _container.Verify();
        }
    }
}
