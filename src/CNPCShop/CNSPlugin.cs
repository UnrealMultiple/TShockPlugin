using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CNPCShop;

[ApiVersion(2, 1)]
public class CNSPlugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "Megghy，肝帝熙恩更新1449";
    public override Version Version => new Version(1, 0, 5);
    public override string Description => GetString("自定义NPC商店出售的物品");
    public CNSPlugin(Main game) : base(game) { }
    public static List<CNSConfig.Shop> AviliableShops { get; internal set; } = new List<CNSConfig.Shop>();
    public static CNSConfig Config { get; internal set; } = new CNSConfig();
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnPostInitialize);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        GeneralHooks.ReloadEvent += this.OnReload;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnPostInitialize);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            GeneralHooks.ReloadEvent -= this.OnReload;
            AviliableShops.Clear();
        }
        base.Dispose(disposing);
    }
    private void OnPostInitialize(EventArgs args)
    {
        CNSConfig.Load();
    }

    private void OnReload(ReloadEventArgs args)
    {
        CNSConfig.Load();
        TShock.Log.ConsoleInfo(GetString("<CNPCShop> 成功读取配置文件"));
    }
    private void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled || args.MsgID != PacketTypes.NpcTalk)
        {
            return;
        }

        var index = args.Msg.readBuffer[args.Index];
        var npcID = (short) (args.Msg.readBuffer[args.Index + 1]
                         + (args.Msg.readBuffer[args.Index + 2] << 8));
        if (index != args.Msg.whoAmI || npcID == -1)
        {
            return;
        }

        this.OnShopOpen(TShock.Players[index], npcID);
    }
    void OnShopOpen(TSPlayer plr, int npcID)
    {
        if (plr != null && npcID != -1 && npcID != plr.TPlayer.talkNPC)
        {
            if (AviliableShops.FirstOrDefault(s => s.NPC == Main.npc[npcID].type && (s.Groups.Contains(plr.Group.Name) || !s.Groups.Any())) is { } shop)
            {
                Task.Run(() =>
                {
                    if (shop.OpenMessage.Any())
                    {
                        plr.SendMessage(shop.OpenMessage[new Random().Next(shop.OpenMessage.Count)].Replace("{name}", plr.Name), Color.White);
                    }

                    while (plr.TPlayer.talkNPC == npcID)
                    {
                        shop.RawData.ForEach(r => plr.SendRawData(r));
                        Task.Delay(Config.UpdateTime).Wait();
                    }
                    if (shop.CloseMessage.Any())
                    {
                        plr.SendMessage(shop.CloseMessage[new Random().Next(shop.CloseMessage.Count)].Replace("{name}", plr.Name), Color.White);
                    }
                });
            }
        }
    }
}