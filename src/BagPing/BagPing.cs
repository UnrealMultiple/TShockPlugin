using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;

namespace BagPing;

[ApiVersion(2, 1)]
public class BagPing : TerrariaPlugin
{

    public override string Author => "Cai";

    public override string Description => GetString("在小地图上标记掉落的宝藏袋");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 4);

    public BagPing(Main game)
    : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKilled);
    }



    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKilled);

        }
        base.Dispose(disposing);
    }


    private void OnNpcKilled(NpcKilledEventArgs e)
    {
        if (e.npc.boss)
        {
            TSPlayer.All.SendSuccessMessage(TShock.Utils.ItemTag(new Item() { netID = 3318, stack = 1, prefix = 0 })
                + GetString($"宝藏袋掉落在坐标({(int) e.npc.position.X / 16},{(int) e.npc.position.Y / 16}),已在小地图上标记!"));
            Task.Run(() =>
            {
                for (var i = 0; i < 4; i++)
                {
                    foreach (var player in TShock.Players)
                    {
                        if (player != null && player.Active)
                        {
                            NetManager.Instance.SendToClient(NetPingModule.Serialize(new Vector2(e.npc.position.X / 16, e.npc.position.Y / 16)), player.Index);
                        }
                    }
                    Task.Delay(15000).Wait();
                }
            });
        }
    }
}