using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Solitude.Exchange.Core.RabbitMQ;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Solitude.Exchange.RabbitMQ
{
    class Program
    {
        private static string ExchangeName = "singnalr";
        private static string QueueName = "queueName";
        private static string RouteKey = "MyRoute.*";
        static void Main(string[] args)
        {

             Task.Run(()=> { new RabbitMQFactory().Publish(ExchangeName, "tomcat2", "MyRoute.*"); });
            ClientTest("boo1", "tomcat2", "MyRoute.bo");
            ClientTest("boo2", "tomcat2","my.bo");
            Console.ReadLine();
        }

        public static void ClientTest(string cname,string queueName= "tomcat",string routeKey= "MyRoute.#")
        {
           
            //var queueName = "tomcat";
            var factory = new RabbitMQFactory();
            factory.Channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, false, false, null);
            factory.Channel.QueueDeclare(queueName, false, false, false, null);
            factory.Channel.QueueBind(queueName,ExchangeName, routeKey, null);
            var consumer = new EventingBasicConsumer(factory.Channel);
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine($"【{cname}】收到消息：{message}");
                //确认该消息已被消费
                factory.Channel.BasicAck(ea.DeliveryTag, false);
            };

            factory.Channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine($"消费者{cname}已启动");
        }

    }
}
