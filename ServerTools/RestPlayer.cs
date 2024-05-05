using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace ServerTools
{
    public class RestPlayer : TSPlayer
    {
        internal List<string> CommandOutput = new List<string>();

        public RestPlayer(string playerName, Group playerGroup) : base(playerName)
        {
            Group = playerGroup;
            AwaitingResponse = new Dictionary<string, Action<object>>();
        }

        public override void SendMessage(string msg, Color color)
        {
            SendMessage(msg, color.R, color.G, color.B);
        }

        public override void SendMessage(string msg, byte red, byte green, byte blue)
        {
            this.CommandOutput.Add(msg);
        }

        public override void SendInfoMessage(string msg)
        {
            SendMessage(msg, Color.Yellow);
        }

        public override void SendSuccessMessage(string msg)
        {
            SendMessage(msg, Color.Green);
        }

        public override void SendWarningMessage(string msg)
        {
            SendMessage(msg, Color.OrangeRed);
        }

        public override void SendErrorMessage(string msg)
        {
            SendMessage(msg, Color.Red);
        }

        public List<string> GetCommandOutput()
        {
            return this.CommandOutput;
        }
    }
}
