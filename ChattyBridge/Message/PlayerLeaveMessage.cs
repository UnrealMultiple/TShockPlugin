using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerLeaveMessage : PlayerMessage
{
    public PlayerLeaveMessage(TSPlayer player) : base(player)
    {
        Type = MsgType.Leave;
    }
    public PlayerLeaveMessage()
    {
        
    }
}
