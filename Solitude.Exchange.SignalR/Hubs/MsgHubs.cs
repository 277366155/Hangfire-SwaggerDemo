using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Solitude.Exchange.SignalR
{
    public class MsgHubs:Hub
    {

        public async Task SaveMsgAsync(string userName, string msg)
        {
            await Clients.All.SendAsync("UpdateClientsCount",0);
        }
    }
}
