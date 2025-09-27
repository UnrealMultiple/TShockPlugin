using LazyAPI;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BP;

[ApiVersion(2, 1)]
public class BackPlugin : LazyPlugin
{
    private readonly Dictionary<int, DateTime> cooldowns = new Dictionary<int, DateTime>();

    public override string Author => "Megghy,熙恩改";
    public override string Description => GetString("允许玩家传送回死亡地点");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 0, 0, 10);

    public BackPlugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerLeave.Register(this, this.ResetPos);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnPlayerJoin);
        GetDataHandlers.KillMe += this.OnDead;

        this.addCommands.Add(new Command("back", this.Back, "back")
        {
            HelpText = GetString("返回最后一次死亡的位置"),
            AllowServer = false
        });
        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerLeave.Deregister(this, this.ResetPos);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnPlayerJoin);
            GetDataHandlers.KillMe -= this.OnDead;
        }

        base.Dispose(disposing);
    }

    private void ResetPos(LeaveEventArgs args)
    {
        var list = TSPlayer.FindByNameOrID(Main.player[args.Who].name);
        if (list.Count > 0)
        {
            list[0].RemoveData("DeadPoint");
        }
    }

    private void Back(CommandArgs args)
    {
        var data = args.Player.GetData<Point>("DeadPoint");

        if (args.Player.TPlayer.dead)
        {
            args.Player.SendErrorMessage(GetString("你尚未复活，无法传送回死亡地点."));
        }
        else if (data != Point.Zero)
        {
            if (!this.CanUseCommand(args.Player))
            {
                var remainingCooldown = this.GetRemainingCooldown(args.Player);
                args.Player.SendErrorMessage(GetString($"你还需要等待 {remainingCooldown.TotalSeconds:F} 秒才能再次使用此命令."));
                return;
            }

            try
            {
                args.Player.Teleport(data.X, data.Y, 1);
                args.Player.SendSuccessMessage(GetString($"已传送至死亡地点 [c/8DF9D8:<{data.X / 16} - {data.Y / 16}>]."));

                this.SetCooldown(args.Player);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"BackPlugin: 传送玩家 {args.Player.Name} 时发生错误: {ex}"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("你还未死亡过"));
        }
    }

    private void OnDead(object? o, GetDataHandlers.KillMeEventArgs args)
    {
        args.Player.SetData("DeadPoint", new Point((int) args.Player.X, (int) args.Player.Y));
    }

    private void OnPlayerJoin(GreetPlayerEventArgs args)
    {
        var list = TSPlayer.FindByNameOrID(Main.player[args.Who].name);
        if (list.Count > 0)
        {
            list[0].SetData("DeadPoint", Point.Zero);
        }
    }

    private bool CanUseCommand(TSPlayer player)
    {
        if (this.cooldowns.ContainsKey(player.Index))
        {
            var cooldownEnd = this.cooldowns[player.Index];
            if (DateTime.Now < cooldownEnd)
            {
                return false;
            }
        }

        return true;
    }

    private TimeSpan GetRemainingCooldown(TSPlayer player)
    {
        if (this.cooldowns.ContainsKey(player.Index))
        {
            var cooldownEnd = this.cooldowns[player.Index];
            var remainingTime = cooldownEnd - DateTime.Now;
            return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
        }

        return TimeSpan.Zero;
    }

    private void SetCooldown(TSPlayer player)
    {
        var cooldownDuration = TimeSpan.FromSeconds(Configuration.Instance.BackCooldown);
        var cooldownEnd = DateTime.Now.Add(cooldownDuration);

        if (this.cooldowns.ContainsKey(player.Index))
        {
            this.cooldowns[player.Index] = cooldownEnd;
        }
        else
        {
            this.cooldowns.Add(player.Index, cooldownEnd);
        }
    }
}