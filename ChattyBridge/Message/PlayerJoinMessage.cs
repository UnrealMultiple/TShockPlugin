using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ChattyBridge.Message;

internal class PlayerJoinMessage : PlayerMessage
{
    public PlayerJoinMessage(TSPlayer player) : base(player)
    {
        Type = MsgType.Join;
    }
    public PlayerJoinMessage()
    {
        
    }
}
