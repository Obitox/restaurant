using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RabbitMQ.Client;
using Restaurant.Infrastructure.RabbitMQ;
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

                    RabbitMQSettings rabbitMQSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
                    EmailSettings emailSettings = configuration.GetSection("Email").Get<EmailSettings>();

                    services.AddSingleton(rabbitMQSettings);
                    services.AddSingleton(emailSettings);
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddSingleton<ConnectionFactory>();

                    services.AddSingleton<ConfirmationEmailJob>();
                    services.AddSingleton(new JobSchedule(
                        jobType: typeof(ConfirmationEmailJob),
                        cronExpression: "0/5 * * * * ?")); // run every 5 seconds

                    services.AddHostedService<QuartzHostedService>();
                });
    }
}
