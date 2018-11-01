using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solitude.Exchange.Core.RabbitMQ
{
   public  class RabbitMQFactory
    {
        private static IConfigurationSection _rabbitMQConfig;
        private  ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public IConfigurationSection RabbitMQConfig
        {
            get
            {
                if (_rabbitMQConfig == null)
                {
                    _rabbitMQConfig = BaseCore.Configuration.GetSection("RabbitMQConfig");
                }
                return _rabbitMQConfig;
            }
        }
        public ConnectionFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new ConnectionFactory()
                    {
                        HostName = RabbitMQConfig["Host"],
                        Port=Convert.ToInt32(RabbitMQConfig["Port"]),
                        UserName = RabbitMQConfig["User"],
                        Password = RabbitMQConfig["Pwd"]
                    };
                }
                return _factory;
            }
        }

        public IConnection Connection
        {
            get
            {
                if(_connection==null)
                    _connection=Factory.CreateConnection();
                return _connection;
            }
        }
        
        //创建通道
        public IModel Channel
        {
            get
            {
                if (_channel == null)
                    _channel = Connection.CreateModel();
                return _channel;
            }
        }


        public void Publish(string exchangeName="",string queueName="",string routeKey="")
        {
            //定义队列
            Channel.QueueDeclare(queueName, false, false, false, null);
            //队列绑定到交换机
            Channel.QueueBind(queueName, exchangeName, routeKey, null);
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入q退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                Channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
            } while (input.Trim().ToLower() != "q");
}

        ~RabbitMQFactory()
        {
            if (_channel != null && !_channel.IsClosed)
                _channel.Close();
                _channel.Dispose();
            if (_connection != null && _connection.IsOpen)
                _connection.Close();
                _connection.Dispose();
        }
    }
}
