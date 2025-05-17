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

    public override string Description => GetString("提供攻击NPC跳出浮动文字效果");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 2, 4);

    public Plugin(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        if (!File.Exists(this.path))
        {
            this.config.Write(this.path);
        }
        this.config.Read(this.path);
        if (this.config.CritMessages.Count == 0)
        {
            this.AddDefaultsToConfig();
        }
        GeneralHooks.ReloadEvent += this.OnReload;
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
    }

    private void OnReload(ReloadEventArgs e)
    {
        if (!File.Exists(this.path))
        {
            this.config.Write(this.path);
        }
        this.config.Read(this.path);
    }

    private void OnGetData(GetDataEventArgs args)
    {
        if (this.config.Enable)
        {
            if ((int) args.MsgID != 28 || args.Msg.whoAmI < 0 || args.Msg.whoAmI > Main.maxNetPlayers)
            {
                return;
            }
            var player = Main.player[args.Msg.whoAmI];
            using var memoryStream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length - 1);
            var num = StreamExt.ReadInt16(memoryStream);
            StreamExt.ReadInt16(memoryStream);
            StreamExt.ReadSingle(memoryStream);
            StreamExt.ReadInt8(memoryStream);
            var flag = Convert.ToBoolean(StreamExt.ReadInt8(memoryStream));
            if ((Main.npc[num] != null && flag) || !this.config.NoCritMessages)
            {
                var item = player.inventory[player.selectedItem];
                var dictionary = (item.ranged && !ItemID.Sets.Explosives[item.type]) ? this.config.CritMessages[WeaponType.Ranged].Messages : (item.melee ? this.config.CritMessages[WeaponType.Melee].Messages : (item.magic ? this.config.CritMessages[WeaponType.Magic].Messages : ((!ItemID.Sets.Explosives[item.type] && item.type != 168 && item.type != 3116 && item.type != 3548 && item.type != 2586) ? this.config.CritMessages[WeaponType.Melee].Messages : this.config.CritMessages[WeaponType.Explosive].Messages)));
                var keyValuePair = dictionary.ElementAt(this.r.Next(0, dictionary.Count));
                var val = new Color(keyValuePair.Value[0], keyValuePair.Value[1], keyValuePair.Value[2]);
                NetMessage.SendData(number: (int) val.PackedValue, msgType: 119, remoteClient: -1, ignoreClient: -1, text: NetworkText.FromLiteral(keyValuePair.Key), number2: Main.npc[num].position.X, number3: Main.npc[num].position.Y);
            }
        }
    }

    private void AddDefaultsToConfig()
    {
        var critMessage = new CritMessage();
        critMessage.Messages.Add("Boom!", new int[3] { 255, 0, 0 });
        critMessage.Messages.Add("Plop!", new int[3] { 255, 0, 0 });
        critMessage.Messages.Add("Pop!", new int[3] { 255, 0, 0 });
        //中文在PE中无法正常显示
        //critMessage.Messages.Add("砰!", new int[3] { 255, 120, 0 });
        //critMessage.Messages.Add("嘭!", new int[3] { 255, 40, 50 });
        // critMessage.Messages.Add("啪!", new int[3] { 255, 255, 0 });
        //critMessage.Messages.Add("噗通!", new int[3] { 255, 0, 0 });
        this.config.CritMessages.Add(WeaponType.Melee, critMessage);
        critMessage = new CritMessage();
        critMessage.Messages.Add("Boom!", new int[3] { 255, 0, 0 });
        //critMessage.Messages.Add("轰隆!", new int[3] { 255, 0, 0 });
        this.config.CritMessages.Add(WeaponType.Explosive, critMessage);
        critMessage = new CritMessage();
        critMessage.Messages.Add("Biu biu!", new int[3] { 50, 255, 10 });
        this.config.CritMessages.Add(WeaponType.Ranged, critMessage);
        critMessage = new CritMessage();
        //critMessage.Messages.Add("啪嗒!", new int[3] { 10, 50, 255 });
        //critMessage.Messages.Add("嗖!", new int[3] { 0, 150, 255 });
        critMessage.Messages.Add("Whoomph!", new int[3] { 0, 200, 255 });
        critMessage.Messages.Add("Crackle!", new int[3] { 0, 200, 255 });
        this.config.CritMessages.Add(WeaponType.Magic, critMessage);
        this.config.Write(this.path);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);

            GeneralHooks.ReloadEvent -= this.OnReload;
        }
        base.Dispose(disposing);
    }
}