using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.RabbitMQ;
using Restaurant.Scheduler.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    class OrderEmailJob : IJob
    {
        private readonly ILogger<ConfirmationEmailJob> _logger;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly EmailSettings _emailSettings;
        private readonly ConnectionFactory _connectionFactory;

        public OrderEmailJob(ILogger<ConfirmationEmailJob> logger, RabbitMQSettings rabbitMQSettings, EmailSettings emailSettings, ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _rabbitMQSettings = rabbitMQSettings;
            _emailSettings = emailSettings;
            _connectionFactory = connectionFactory;
            _connectionFactory.UserName = _rabbitMQSettings.User;
            _connectionFactory.Password = _rabbitMQSettings.Pass;
            _connectionFactory.VirtualHost = "/";
            _connectionFactory.HostName = _rabbitMQSettings.HostName;
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
                    string message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation(message);
                    if (message.Length > 0)
                        messageSplitted = message.Split(";");

                    //$"{nameTo};{emailTo};{orderId};{cartId}";
                    if (messageSplitted.Length > 0)
                    {
                        string nameTo = messageSplitted[0];
                        string emailTo = messageSplitted[1];
                        ulong.TryParse(messageSplitted[2], out ulong orderId);
                        ulong.TryParse(messageSplitted[3], out ulong cartId);
                        // TODO: Change from my email
                        SendEmail(nameTo, emailTo, confirmLink);
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };


                channel.BasicConsume(queue: "restaurantSendConfirmationEmail",
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

        private void SendEmail(string nameTo, string emailTo, Order order)
        {
            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("Bojan Miric", _emailSettings.Username);
            message.From.Add(from);
            MailboxAddress to = new MailboxAddress(nameTo, emailTo);
            message.To.Add(to);
            message.Subject = "Test";

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<a href={$"{confirmLink}"}><u>Click here to confirm your email</u></a>",
                TextBody = "Test"
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
