using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace InvincibilityPlugin;

[ApiVersion(2, 1)]
public class InvincibilityPlugin : TerrariaPlugin
{
    public override string Author => "肝帝熙恩";
    public override string Description => GetString("在命令中给予玩家一段时间的无敌状态。");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 0);
    public static Configuration Config = null!;

    private readonly Dictionary<TSPlayer, float> invincibleDurations = new();
    private readonly Dictionary<TSPlayer, float> frameDurations = new();

    public long FrameCount;

    public InvincibilityPlugin(Main game) : base(game)
    {
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);

    }
    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage(GetString("[{0}] 重新加载配置完毕。"), typeof(InvincibilityPlugin).Name);
    }
    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        Commands.ChatCommands.Add(new Command("Invincibility.god", this.InvincibleCommand, "tgod", "限时god无敌"));
        Commands.ChatCommands.Add(new Command("Invincibility.frame", this.ActivateFrameInvincibility, "tframe", "限时无敌帧无敌"));
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        LoadConfig();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InvincibleCommand || x.CommandDelegate == this.ActivateFrameInvincibility);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
        }
        base.Dispose(disposing);
    }

    private void OnUpdate(EventArgs args)
    {
        this.FrameCount++;
        //每100毫秒检测一次
        if (this.FrameCount % 6 == 0)
        {
            foreach (var player in this.invincibleDurations.Keys.ToList())
            {
                if (this.invincibleDurations[player] <= 100 || !player.Active)
                {
                    this.delGodMode(player);
                }
                else
                {
                    this.invincibleDurations[player] -= 100;
                }
            }

            foreach (var player in this.frameDurations.Keys.ToList())
            {
                if (this.frameDurations[player] >= 0 && player.Active)
                {
                    player.SendData(PacketTypes.PlayerDodge, "", player.Index, 2f, 0f, 0f, 0);
                    this.frameDurations[player] -= 100;
                }
                else
                {
                    player.SendSuccessMessage(Config.CustomEndFrameText);
                    this.frameDurations.Remove(player);
                }
            }
        }
    }

    private void InvincibleCommand(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendErrorMessage(GetString("用法: /限时god无敌 或 /tgod <持续时间秒数>"));
            return;
        }

        if (!float.TryParse(args.Parameters[0], out var duration) || duration <= 0)
        {
            args.Player.SendErrorMessage(GetString("无效的持续时间。请输入一个正数。"));
            return;
        }

        var player = TShock.Players[args.Player.Index];

        if (player == null || !player.Active)
        {
            args.Player.SendErrorMessage(GetString("玩家不在线。"));
            return;
        }
        this.addGodMode(player, duration);
    }

    private void addGodMode(TSPlayer player, float duration)
    {
        player.GodMode = true;
        CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, player.GodMode);
        if (Config.EnableInvincibleReminder)
        {
            player.SendSuccessMessage(GetString($"你将在 {duration} 秒内无敌."));
        }

        if (!string.IsNullOrEmpty(Config.CustomInvincibleReminderText))
        {
            player.SendSuccessMessage(Config.CustomInvincibleReminderText);
        }
        NetMessage.SendData((int) PacketTypes.PlayerInfo, -1, -1, null, player.Index, 1f);
        this.invincibleDurations[player] = duration * 1000;
    }

    private void delGodMode(TSPlayer player)
    {
        player.GodMode = false;
        CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, player.GodMode);
        player.SendSuccessMessage($"{Config.CustomInvincibleDisableText}");
        NetMessage.SendData((int) PacketTypes.PlayerInfo, -1, -1, null, player.Index, 1f);
        this.invincibleDurations.Remove(player);
    }

    private void ActivateFrameInvincibility(CommandArgs args)
    {
        if (args.Parameters.Count < 1 || !int.TryParse(args.Parameters[0], out var duration) || duration <= 0)
        {
            args.Player.SendErrorMessage(GetString("用法: /限时无敌帧无敌或tframe <持续时间秒数>"));
            return;
        }
        if (Config.EnableFrameReminder)
        {
            args.Player.SendSuccessMessage(GetString($"你将在 {args.Parameters[0]} 秒内无敌."));
        }
        if (!string.IsNullOrEmpty(Config.CustomStartFrameText))
        {
            args.Player.SendSuccessMessage(Config.CustomStartFrameText);
        }

        this.frameDurations[args.Player] = duration * 1000;
    }
}