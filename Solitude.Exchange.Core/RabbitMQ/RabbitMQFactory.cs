using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitude.Exchange.Core.RabbitMQ
{
    public class RabbitMQFactory
    {
        public IConfigurationSection RMQConfig
        {
            get
            {
                return BaseCore.Configuration.GetSection("RMQConfig");
            }
        }
        public IBusControl MyBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(RMQConfig["Host"]), hst =>
                {
                    hst.Username(RMQConfig["User"]);
                    hst.Password(RMQConfig["Pwd"]);
                });
            });
            return bus;
        }
    }
}
