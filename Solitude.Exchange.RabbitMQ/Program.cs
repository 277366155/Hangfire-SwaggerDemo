using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Solitude.Exchange.Core;
using Solitude.Exchange.Core.RabbitMQ;
using System;
using System.Collections.Generic;
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
            MassTransitTest();
            Console.ReadLine();
        }

        #region 原生调用方法
        private void RabbitMQTest()
        {
            Task.Run(() => { new RabbitMQFac().Publish(ExchangeName, "tomcat2", "MyRoute.*"); });
            ClientTest("boo1", "tomcat2", "MyRoute.bo");
            ClientTest("boo2", "tomcat2", "my.bo");           
        }

        public static void ClientTest(string cname,string queueName= "tomcat",string routeKey= "MyRoute.#")
        {           
            //var queueName = "tomcat";
            var factory = new RabbitMQFac();
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
        #endregion

        #region MassTransit
        private static void MassTransitTest()
        {
            var factory = new RabbitMQFactory();
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(factory.RMQConfig["Host"]), hst =>
                {
                    hst.Username(factory.RMQConfig["User"]);
                    hst.Password(factory.RMQConfig["Pwd"]);                    
                });

                //factory.RMQConfig["Queue"]
                cfg.ReceiveEndpoint(host, "market_1", e =>
                {
                    e.Consumer(() => new MessageContractConsumer());
                });
            });


            var msg = Console.ReadLine();
            var uri = new Uri(factory.RMQConfig["Host"]);
            bus.Start();
            while (msg != null)
            {
                //Task.Run(()=> SendCommand(bus,uri,msg));
                var command = new MatchingNotifyInfo
                {
                    Info = new TradeMarketInfo()
                    {                         
                        Ask = new List<DishModel>(),
                        Bid = new List<DishModel>(),
                        CurrencyUnit = msg,
                        High = 100,
                        Low = 1,
                        Open = 20,
                         
                    },
                    Response = new MatchingResponse()
                    {
                        Orders = new List<SimpleOrderInfo>(),
                        UserAsset = new List<SimpleUserAssetInfo>(),
                        HasError = false,
                        ErrMsgs = new string[] { "error..", "波波。。" },
                        AssetLogs = new Dictionary<int, List<UserAssetRecord>>()
                    }
                };
                var request = new ExchangeRequest<MatchingNotifyInfo>() { Data = command, CorrelationId = Guid.NewGuid() };
                bus.Publish(request);
                msg = Console.ReadLine();
            }
            bus.Stop();
        }

        //private static async void SendCommand(IBusControl bus, Uri sendToUri, string unit)
        //{           
        //    var endPoint = await bus.GetSendEndpoint(sendToUri);            
        //    var command = new MatchingNotifyInfo
        //    {
        //        Info = new TradeMarketInfo()
        //        {
        //            Ask = new List<DishModel>(),
        //            Bid = new List<DishModel>(),
        //            CurrencyUnit = unit,
        //            High = 100,
        //            Low = 1,
        //            Open = 20
        //        },
        //        Response = new MatchingResponse()
        //        {
        //            Orders = new List<SimpleOrderInfo>(),
        //            UserAsset = new List<SimpleUserAssetInfo>(),
        //            HasError = false,
        //            ErrMsgs = new string[] { "error..", "波波。。" },
        //            AssetLogs = new Dictionary<int, List<UserAssetRecord>>()
        //        }
        //    };
        //    await endPoint.Send(command);

        //    Console.WriteLine($"You Sended :币种{command.Info.CurrencyUnit}，最高价{command.Info.High}，最低价{command.Info.Low}");
        //}
        
        
        #endregion
    }

    public class MessageContractConsumer : IConsumer<IExchangeRequest<MatchingNotifyInfo>>
    {
        public async Task Consume(ConsumeContext<IExchangeRequest<MatchingNotifyInfo>> context)
        {
            Console.WriteLine($"Recevied By Consumer:{context.Message.ToJson()}");
        }
    }

    public class MessageConsumer : IObserver<ConsumeContext<MatchingNotifyInfo>>
    {
        public void OnCompleted()
        {
            //throw new NotImplementedException();            
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ConsumeContext<MatchingNotifyInfo> value)
        {
            Console.WriteLine($"Received By Observer:{value.Message.ToJson()}");
        }
    }
}
