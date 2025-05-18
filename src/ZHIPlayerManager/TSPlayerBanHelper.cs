using Localizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace ZHIPlayerManager;

public static class TSPlayerBanHelper
{
    public static bool SuperBan(this TSPlayer ply, string reason, string duration, string adminUserName)
    {
        if(!ply.ConnectionAlive)
        {
            return true;
        }
        var expiration = DateTime.MaxValue;
        if(TShock.Utils.TryParseTime(duration, out ulong seconds))
        {
            expiration = seconds > 0 ? DateTime.UtcNow.AddSeconds(seconds) : DateTime.MaxValue;
        }
        TShock.Bans.InsertBan($"{Identifier.IP}{ply.IP}", reason, adminUserName, DateTime.UtcNow, expiration);
        TShock.Bans.InsertBan($"{Identifier.UUID}{ply.UUID}", reason, adminUserName, DateTime.UtcNow, expiration);
        if (ply.Account != null)
        {
            TShock.Bans.InsertBan($"{Identifier.Account}{ply.Account.Name}", reason, adminUserName, DateTime.UtcNow, expiration);
        }

        ply.Disconnect(I18n.GetString("Banned: {0}", reason));
        if (string.IsNullOrWhiteSpace(adminUserName))
        {
            TSPlayer.All.SendInfoMessage(I18n.GetString("{0} was banned for '{1}'.", ply.Name, reason));
        }
        else
        {
            TSPlayer.All.SendInfoMessage(I18n.GetString("{0} banned {1} for '{2}'.", adminUserName, ply.Name, reason));
        }

        return true;
    }
}
