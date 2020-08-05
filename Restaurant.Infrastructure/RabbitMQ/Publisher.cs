using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.RabbitMQ
{
    // TODO: Publish methods can be abstracted to a single method
    public class Publisher : IPublisher
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public Publisher(ConnectionFactory connectionFactory, IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            _connectionFactory = connectionFactory;
            _rabbitMQSettings = rabbitMQSettings.Value;
            _connectionFactory = connectionFactory;
            _connectionFactory.UserName = _rabbitMQSettings.User;
            _connectionFactory.Password = _rabbitMQSettings.Pass;
            _connectionFactory.VirtualHost = "/";
            _connectionFactory.HostName = _rabbitMQSettings.HostName;
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
            catch (Exception exception)
            {
                // TODO: Logging
                return false;
            }
        }

        public bool PublishOrderEmail(string nameTo, string emailTo, ulong orderId, ulong cartId)
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

                string msg = $"{nameTo};{emailTo};{orderId};{cartId}";
                byte[] body = Encoding.UTF8.GetBytes(msg);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                          routingKey: "restaurantOrderEmail",
                                          basicProperties: properties,
                                          body: body);

                return true;
            }
            catch (Exception exception)
            {
                // TODO: Logging
                return false;
            }
        }
    }
}
