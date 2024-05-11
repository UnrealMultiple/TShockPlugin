using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class BasePlayerEventArgs : BaseEventArgs
{
    public TSPlayer Player { get; init; }
}
