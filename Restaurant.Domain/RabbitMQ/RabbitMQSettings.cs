using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.RabbitMQ
{
    public class RabbitMqSettings
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}
