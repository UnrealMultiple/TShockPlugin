using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.UI.Chat;
using TShockAPI;
using TShockAPI.DB;

namespace CaiBot;

public class CaiBotPlayer : TSPlayer
{
    private readonly List<string> _commandOutput = new ();

    public CaiBotPlayer()
        : base("CaiBot")
    {
        this.Group = new SuperAdminGroup();
        this.AwaitingResponse = new Dictionary<string, Action<object>>();
        this.Account = new UserAccount { Name = "CaiBot",ID = -1 };
    }

    public override void SendMessage(string msg, Color color)
    {
        this.SendMessage(msg, color.R, color.G, color.B);
    }

    public override void SendMessage(string msg, byte red, byte green, byte blue)
    {
        var result1 = "";
        foreach (var item in ChatManager.ParseMessage(msg, new Color(red, green, blue)))
        {
            result1 += item.Text;
        }

        Regex regex = new (@"\[i(tem)?(?:\/s(?<Stack>\d{1,4}))?(?:\/p(?<Prefix>\d{1,3}))?:(?<NetID>-?\d{1,4})\]");

        var result = regex.Replace(result1, m =>
        {
            var netId = m.Groups["NetID"].Value;
            var prefix = m.Groups["Prefix"].Success ? m.Groups["Prefix"].Value : "0";
            var stack = m.Groups["Stack"].Success ? m.Groups["Stack"].Value : "0";
            if (stack == "0")
            {
                return "";
            }

            if (stack == "1")
            {
                if (prefix == "0")
                {
                    return $"[{Lang.GetItemName(int.Parse(netId))}]";
                }

                return
                    $"[{Lang.prefix[int.Parse(prefix)]} {Lang.GetItemName(int.Parse(netId))}]"; //return $"[{Terraria.Lang.prefix[int.Parse(netID)]}]";
            }

            return $"[{Lang.prefix[int.Parse(prefix)]} {Lang.GetItemName(int.Parse(netId))} ({stack})]";
        });
        this._commandOutput.Add(result);
    }

    public override void SendInfoMessage(string msg)
    {
        this.SendMessage(msg, Color.Yellow);
    }

    public override void SendSuccessMessage(string msg)
    {
        this.SendMessage(msg, Color.Green);
    }

    public override void SendWarningMessage(string msg)
    {
        this.SendMessage(msg, Color.OrangeRed);
    }

    public override void SendErrorMessage(string msg)
    {
        this.SendMessage(msg, Color.Red);
    }

    public List<string> GetCommandOutput()
    {
        return this._commandOutput;
    }
}