using Restaurant.Domain.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.RabbitMQ
{
    public interface IPublisher
    {
        bool PublishConfirmationEmailMessage(string nameTo, string emailTo, string confirmLink);
        bool PublishOrderEmail(ulong userId, string nameTo, string emailTo, OrderView orderView);
    }
}
