using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RazorEngine;
using RazorEngine.Templating;
using Restaurant.Domain.RabbitMQ;
using Restaurant.Domain.Templates;
using Restaurant.Scheduler.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Restaurant.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    class OrderEmailJob : IJob
    {
        private readonly ILogger<ConfirmationEmailJob> _logger;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly EmailSettings _emailSettings;
        private readonly ConnectionFactory _connectionFactory;

        public OrderEmailJob(ILogger<ConfirmationEmailJob> logger, RabbitMqSettings rabbitMqSettings, EmailSettings emailSettings, ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _rabbitMqSettings = rabbitMqSettings;
            _emailSettings = emailSettings;
            _connectionFactory = connectionFactory;
            _connectionFactory.UserName = _rabbitMqSettings.User;
            _connectionFactory.Password = _rabbitMqSettings.Pass;
            _connectionFactory.VirtualHost = "/";
            _connectionFactory.HostName = _rabbitMqSettings.HostName;
        }

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                IConnection conn = _connectionFactory.CreateConnection();
                IModel channel = conn.CreateModel();

                channel.QueueDeclare(queue: "restaurantOrderEmail",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                string[] messageSplitted = Array.Empty<string>();
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body;
                    string message = System.Text.Encoding.UTF8.GetString(body);
                    _logger.LogInformation("ENTERED");
                    _logger.LogInformation(message);
                    if (message.Length > 0)
                        messageSplitted = message.Split(";");

                    if (messageSplitted.Length > 0)
                    {
                        ulong.TryParse(messageSplitted[0], out ulong userId);
                        string nameTo = messageSplitted[1];
                        string emailTo = messageSplitted[2];
                        string order = messageSplitted[3];
                        var orderView = JsonConvert.DeserializeObject<OrderView>(order);

                        SendEmail(nameTo, emailTo, orderView);
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };


                channel.BasicConsume(queue: "restaurantOrderEmail",
                                        autoAck: false,
                                        consumer: consumer);

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                _logger.LogInformation(exception.Message);
                return Task.CompletedTask;
            }
        }

        private void SendEmail(string nameTo, string emailTo, OrderView order)
        {
            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(nameTo, _emailSettings.Username);
            message.From.Add(from);
            MailboxAddress to = new MailboxAddress(nameTo, emailTo);
            message.To.Add(to);
            message.Subject = $"Order for {nameTo}";

            var templateFilePath = "C:/Users/Cybertech/source/repos/restaurant/Restaurant.Scheduler/Views/OrderEmail.cshtml";
            string html = Engine.Razor.RunCompile(File.ReadAllText(templateFilePath), "sfsfsdasdasd", typeof(OrderView), order);

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = $"Order for {nameTo}"
            };

            message.Body = bodyBuilder.ToMessageBody();
            SmtpClient client = new SmtpClient();

            client.Connect(_emailSettings.Domain, _emailSettings.Port, false);
            client.Authenticate(_emailSettings.Username, _emailSettings.Password);
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
