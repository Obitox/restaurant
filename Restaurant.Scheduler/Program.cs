using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RabbitMQ.Client;
using Restaurant.Domain.RabbitMQ;
using Restaurant.Scheduler.Helpers;
using Restaurant.Scheduler.Jobs;
using Microsoft.Extensions.Logging;

namespace Restaurant.Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddFile("Logs/restaurant-scheduler-{Date}.txt");

                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    RabbitMqSettings rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
                    EmailSettings emailSettings = configuration.GetSection("Email").Get<EmailSettings>();

                    services.AddSingleton(rabbitMqSettings);
                    services.AddSingleton(emailSettings);
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddSingleton<ConnectionFactory>();

                    services.AddSingleton<ConfirmationEmailJob>();
                    services.AddSingleton(new JobSchedule(
                        jobType: typeof(ConfirmationEmailJob),
                        cronExpression: "0/5 * * * * ?")); // run every 5 seconds

                    services.AddSingleton<OrderEmailJob>();
                    services.AddSingleton(new JobSchedule(
                        jobType: typeof(OrderEmailJob),
                        cronExpression: "0/5 * * * * ?")); // run every 5 seconds

                    //IOrderRepository orderRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository, ICartMealRepository cartMealRepository, IPublisher publisher, IUserRepository userRepository

                    //services.AddSingleton<Infrastructure.Services.IOrderService, Infrastructure.Services.OrderService>();
                    //services.AddSingleton<IOrderRepository, OrderRepository>();
                    //services.AddSingleton<ICartRepository, CartRepository>();
                    //services.AddSingleton<ICartItemRepository, CartItemRepository>();
                    //services.AddSingleton<ICartMealRepository, CartMealRepository>();
                    //services.AddSingleton<IPublisher, Publisher>();
                    //services.AddSingleton<IUserRepository, UserRepository>();



                    services.AddHostedService<QuartzHostedService>();
                    //services.AddDbContextPool<fastfood_dbContext>(options => options
                    //    // replace with your connection string
                    //    .UseMySql(connectionString, mySqlOptions => mySqlOptions
                    //        // replace with your Server Version and Type
                    //        .ServerVersion(new ServerVersion(new Version(8, 0, 12), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql))
                    //));
                });
    }
}
