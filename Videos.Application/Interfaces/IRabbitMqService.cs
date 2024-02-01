using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Application.Interfaces
{
    public interface IRabbitMqService
    {
        void SendMessage(object obj, string queue);
        void SendMessage(string message, string queue);
    }
}
