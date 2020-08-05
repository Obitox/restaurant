using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.RabbitMQ
{
    public interface IPublisher
    {
        bool PublishConfirmationEmailMessage(string nameTo, string emailTo, string confirmLink);
    }
}
