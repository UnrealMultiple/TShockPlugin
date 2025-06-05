using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;

namespace BagPing;

[ApiVersion(2, 1)]
public class BagPing(Main game) : TerrariaPlugin(game)
{

    public override string Author => "Cai";

    public override string Description => GetString("在小地图上标记掉落的宝藏袋");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2025, 6, 1);

    public override void Initialize()
    {
        ServerApi.Hooks.DropBossBag.Register(this, OnNpcKilled);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.DropBossBag.Deregister(this, OnNpcKilled);
        }
        base.Dispose(disposing);
    }


    private static async void OnNpcKilled(DropBossBagEventArgs e)
    {
        try
        {
            
            TSPlayer.All.SendSuccessMessage(TShock.Utils.ItemTag(new Item { netID = e.ItemId, stack = e.Stack, prefix = (byte)e.Prefix })
                                            + GetString($"{Lang.GetItemNameValue(e.ItemId)}已掉落在坐标({(int) e.Position.X / 16},{(int) e.Position.Y / 16}),已在小地图上标记!"));
            for (var i = 0; i < 4; i++)
            {
                foreach (var player in TShock.Players)
                {
                    if (player is { Active: true })
                    {
                        NetManager.Instance.SendToClient(NetPingModule.Serialize(new Vector2(e.Position.X/ 16, e.Position.Y / 16)), player.Index);
                    }
                }
                await Task.Delay(15000);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[BagPing]报错: {ex}"));
        }
    }
}