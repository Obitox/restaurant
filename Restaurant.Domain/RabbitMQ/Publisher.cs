using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Restaurant.Domain.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.RabbitMQ
{
    // TODO: Publish methods can be abstracted to a single method
    public class Publisher : IPublisher
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMqSettings _rabbitMqSettings;

        public Publisher(ConnectionFactory connectionFactory, IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            _connectionFactory = connectionFactory;
            _rabbitMqSettings = rabbitMqSettings.Value;
            _connectionFactory = connectionFactory;
            _connectionFactory.UserName = _rabbitMqSettings.User;
            _connectionFactory.Password = _rabbitMqSettings.Pass;
            _connectionFactory.VirtualHost = "/";
            _connectionFactory.HostName = _rabbitMqSettings.HostName;
        }

        public bool PublishConfirmationEmailMessage(string nameTo, string emailTo, string confirmLink)
        {
            try
            {
                using IConnection conn = _connectionFactory.CreateConnection();
                using IModel channel = conn.CreateModel();

                channel.QueueDeclare(queue: "restaurantSendConfirmationEmail",
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

                string msg = $"{nameTo};{emailTo};{confirmLink}";
                byte[] body = Encoding.UTF8.GetBytes(msg);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                          routingKey: "restaurantSendConfirmationEmail",
                                          basicProperties: properties,
                                          body: body);

                return true;
            }
            catch (Exception)
            {
                // TODO: Logging
                return false;
            }
        }

        public bool PublishOrderEmail(ulong userId, string nameTo, string emailTo, OrderView order)
        {
            try
            {
                using IConnection conn = _connectionFactory.CreateConnection();
                using IModel channel = conn.CreateModel();

                channel.QueueDeclare(queue: "restaurantOrderEmail",
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

                var orderJson = JsonConvert.SerializeObject(order);

                string msg = $"{userId};{nameTo};{emailTo};{orderJson}";
                byte[] body = Encoding.UTF8.GetBytes(msg);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                          routingKey: "restaurantOrderEmail",
                                          basicProperties: properties,
                                          body: body);

                return true;
            }
            catch (Exception)
            {
                // TODO: Logging
                return false;
            }
        }
    }
}
