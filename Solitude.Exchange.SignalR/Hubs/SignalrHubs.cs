using Microsoft.AspNetCore.SignalR;
using Solitude.Exchange.Core;
using Solitude.Exchange.Core.Redis;
using Solitude.Exchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solitude.Exchange.SignalR
{
    public class SignalrHubs : Hub
    {
        public static List<OrderInfo> OrderList;
        private readonly string ChatKey = "chat";

        public override Task OnConnectedAsync()
        {
            //加redis锁
            try
            {
                if (RedisHelper.LockTake())
                {
                    var chatHistory = RedisHelper.GetStringByKey(ChatKey);
                    if (!string.IsNullOrWhiteSpace(chatHistory))
                    {
                        Clients.All.SendAsync("GetHistoryMsg", chatHistory);
                    }
                }
            }
            catch (Exception ex)
            {
                Clients.All.SendAsync("print", ex.Message);
            }
            finally
            {
                RedisHelper.LockRelease();
            }

            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 创建signalr链接
        /// </summary>
        public async Task SendMsg(string user, string msg)
        {
            /*
             后端处理逻辑。。。
             */
            //加redis锁
            try
            {
                var now = DateTime.Now;
                if (RedisHelper.LockTake())
                {
                    var chatHistory = RedisHelper.GetDataByKey<List<User>>(ChatKey);
                    if (chatHistory == null)
                    {
                        chatHistory = new List<User>();
                    }
                    chatHistory.Add(new User() { CreateTime=now, UserName=user, Msg=msg } );
                    RedisHelper.SetData(ChatKey, chatHistory,TimeSpan.FromHours(2));
                    //调用客户端的js方法
                    await Clients.All.SendAsync("GetMsg", user, msg, now.ToString());
                }
            }
            catch (Exception ex)
            {
               await Clients.All.SendAsync("print", ex.Message);
            }
            finally
            {
                RedisHelper.LockRelease();
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
