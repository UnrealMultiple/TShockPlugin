using Microsoft.Xna.Framework;
using System.IO.Streams;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace CriticalHit;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    internal Config config = new Config();

    internal Random r = new Random();

    private readonly string path = Path.Combine(TShock.SavePath, "CriticalConfig.json");

    public override string Author => "White制作,Stone·Free汉化整合，肝帝熙恩更新适配1449";

    public override string Description => "提供攻击NPC跳出浮动文字效果";

    public override string Name => "CriticalHit!";

    public override Version Version => new Version(1, 2);

    public Plugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        if (!File.Exists(path))
        {
            config.Write(path);
        }
        config.Read(path);
        if (config.CritMessages.Count == 0)
        {
            AddDefaultsToConfig();
        }
        GeneralHooks.ReloadEvent += OnReload;
        ServerApi.Hooks.NetGetData.Register(this, OnGetData);
    }

    private void OnReload(ReloadEventArgs e)
    {
        if (!File.Exists(path))
        {
            config.Write(path);
        }
        config.Read(path);
    }

    private void OnGetData(GetDataEventArgs args)
    {
        if (config.Enable)
        {
            if ((int)args.MsgID != 28 || args.Msg.whoAmI < 0 || args.Msg.whoAmI > Main.maxNetPlayers)
            {
                return;
            }
            Player player = Main.player[args.Msg.whoAmI];
            using MemoryStream memoryStream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
            short num = StreamExt.ReadInt16(memoryStream);
            StreamExt.ReadInt16(memoryStream);
            StreamExt.ReadSingle(memoryStream);
            StreamExt.ReadInt8(memoryStream);
            bool flag = Convert.ToBoolean(StreamExt.ReadInt8(memoryStream));
            if ((Main.npc[num] != null && flag) || !config.NoCritMessages)
            {
                Item item = player.inventory[player.selectedItem];
                Dictionary<string, int[]> dictionary = ((item.ranged && !ItemID.Sets.Explosives[item.type]) ? config.CritMessages[WeaponType.Ranged].Messages : (item.melee ? config.CritMessages[WeaponType.Melee].Messages : (item.magic ? config.CritMessages[WeaponType.Magic].Messages : ((!ItemID.Sets.Explosives[item.type] && item.type != 168 && item.type != 3116 && item.type != 3548 && item.type != 2586) ? config.CritMessages[WeaponType.Melee].Messages : config.CritMessages[WeaponType.Explosive].Messages))));
                KeyValuePair<string, int[]> keyValuePair = dictionary.ElementAt(r.Next(0, dictionary.Count));
                Color val = new Color(keyValuePair.Value[0], keyValuePair.Value[1], keyValuePair.Value[2]);
                NetMessage.SendData(number: (int)val.PackedValue, msgType: 119, remoteClient: -1, ignoreClient: -1, text: NetworkText.FromLiteral(keyValuePair.Key), number2: Main.npc[num].position.X, number3: Main.npc[num].position.Y);
            }
        }
    }

    private void AddDefaultsToConfig()
    {
        CritMessage critMessage = new CritMessage();
        critMessage.Messages.Add("砰!", new int[3] { 255, 120, 0 });
        critMessage.Messages.Add("嘭!", new int[3] { 255, 40, 50 });
        critMessage.Messages.Add("啪!", new int[3] { 255, 255, 0 });
        critMessage.Messages.Add("噗通!", new int[3] { 255, 0, 0 });
        config.CritMessages.Add(WeaponType.Melee, critMessage);
        critMessage = new CritMessage();
        critMessage.Messages.Add("Boom!", new int[3] { 255, 0, 0 });
        critMessage.Messages.Add("轰隆!", new int[3] { 255, 0, 0 });
        config.CritMessages.Add(WeaponType.Explosive, critMessage);
        critMessage = new CritMessage();
        critMessage.Messages.Add("Biu biu!", new int[3] { 50, 255, 10 });
        config.CritMessages.Add(WeaponType.Ranged, critMessage);
        critMessage = new CritMessage();
        critMessage.Messages.Add("啪嗒!", new int[3] { 10, 50, 255 });
        critMessage.Messages.Add("嗖!", new int[3] { 0, 150, 255 });
        critMessage.Messages.Add("Whoomph!", new int[3] { 0, 200, 255 });
        config.CritMessages.Add(WeaponType.Magic, critMessage);
        config.Write(path);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);

            GeneralHooks.ReloadEvent -= OnReload;
        }
        base.Dispose(disposing);
    }
}
