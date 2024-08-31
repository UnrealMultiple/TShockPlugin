using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChestRestore;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    public MainPlugin(Main game) : base(game)
    {
    }
    public override string Name => "ChestRestore";
    public override Version Version => new Version(1, 0, 1);
    public override string Author => "Cjx修改，肝帝熙恩简单修改";
    public override string Description => "无限宝箱插件";

    public override void Initialize()
    {
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        GetDataHandlers.ChestOpen += this.OnChestOpen;
    }
    private void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs args)
    {
        var num = Chest.FindChest(args.X, args.Y);
        var chest = Main.chest[num];
        if (chest != null)
        {
            var hasItems = false;
            foreach (var item in chest.item)
            {
                if (item.stack != 0)
                {
                    hasItems = true;
                    break;
                }
            }
            if (hasItems)
            {
                var list = new List<NetItem>();
                for (var j = 0; j < chest.item.Length; j++)
                {
                    var item = chest.item[j];
                    list.Add(new NetItem(item.netID, item.stack, item.prefix));
                }
                args.Player.SetData("chestrestore", JsonConvert.SerializeObject(list));
                args.Player.SetData("chestx", args.X);
                args.Player.SetData("chesty", args.Y);
            }
        }
    }
    private void OnGetData(GetDataEventArgs args)
    {
        if (args.MsgID == PacketTypes.ChestOpen)
        {
            using (var binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
            {
                var tsplayer = TShock.Players[args.Msg.whoAmI];
                int num = binaryReader.ReadInt16();
                var num2 = Chest.FindChest(tsplayer.GetData<int>("chestx"), tsplayer.GetData<int>("chesty"));
                Chest chest = null;
                if (num2 != -1)
                {
                    chest = Main.chest[num2];
                }
                if (num == -1 && chest != null)
                {
                    var list = JsonConvert.DeserializeObject<List<NetItem>>(tsplayer.GetData<string>("chestrestore"));
                    for (var i = 0; i < chest.item.Length; i++)
                    {
                        var item = chest.item[i];
                        item.netDefaults(list[i].NetId);
                        item.stack = list[i].Stack;
                        item.prefix = list[i].PrefixId;
                        TSPlayer.All.SendData(PacketTypes.ChestItem, "", num2, (float) i, 0f, 0f, 0);
                    }
                    tsplayer.SetData("chestrestore", "");
                    tsplayer.SetData("chestx", 0);
                    tsplayer.SetData("chesty", 0);
                }
            }
        }
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            GetDataHandlers.ChestOpen -= this.OnChestOpen;
        }
        base.Dispose(disposing);
    }
}