using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PermaBuff;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new(1, 0, 1);

    private readonly string PATH = Path.Combine(TShock.SavePath, "permbuff.json");

    private Config config = new();

    private long TimerCount = 0;

    private readonly HashSet<TSPlayer> players = new();

    public Plugin(Main game) : base(game)
    {
    }
    private GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        LoadConfig();
        _reloadHandler = (_) => LoadConfig();
        DB.Init();
        DB.ReadAll();
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        ServerApi.Hooks.GameUpdate.Register(this, Update);
        Commands.ChatCommands.Add(new Command("permabuff.use", PAbuff, "permabuff"));
        Commands.ChatCommands.Add(new Command("gpermabuff.use", GPbuff, "gpermabuff"));
        Commands.ChatCommands.Add(new Command("clearbuffs.use", Cbuff, "clearbuffs"));
        GeneralHooks.ReloadEvent += _reloadHandler;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            ServerApi.Hooks.GameUpdate.Deregister(this, Update);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == PAbuff||x.CommandDelegate==GPbuff||x.CommandDelegate==Cbuff);
            GeneralHooks.ReloadEvent -= _reloadHandler;
        }
        base.Dispose(disposing);
    }
    private void LoadConfig()
    {
        if (File.Exists(PATH))
        {
            try
            {
                config = Config.Read(PATH);
            }
            catch (Exception ex)
            {
                TShock.Log.Error($"permabuff.json 读取错误:{ex}");
            }
        }
        config.Write(PATH);
    }

    private void Cbuff(CommandArgs args)
    {
        var buffs = Playerbuffs.GetBuffs(args.Player.Name);
        if (buffs.Count == 0)
        {
            args.Player.SendSuccessMessage("没有永久buff，无需清空");
        }
        else
        {
            buffs.ForEach(x =>
            {
                Playerbuffs.DelBuff(args.Player.Name, x);
            });
            args.Player.SendSuccessMessage("已清空所有永久buff");
        }
    }

    private void GPbuff(CommandArgs args)
    {
        if (args.Parameters.Count == 2)
        {
            var ply = TSPlayer.FindByNameOrID(args.Parameters[1]).Find(x => x.Name == args.Parameters[1]);
            if (ply == null)
            {
                args.Player.SendErrorMessage("玩家不存在或玩家不在线!");
                return;
            }
            if (int.TryParse(args.Parameters[0], out int buffid))
            {
                if (config.LimitBuffs.Contains(buffid))
                {
                    args.Player.SendErrorMessage("此buff不可被添加!");
                    return;
                }
                var playerbuffs = Playerbuffs.GetBuffs(ply.Name);
                if (playerbuffs.Contains(buffid))
                {
                    Playerbuffs.DelBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage($"移除一个永久buff {TShock.Utils.GetBuffName(buffid)}!");
                    ply.SendSuccessMessage($"{args.Player.Name} 移除了你的一个永久buff {TShock.Utils.GetBuffName(buffid)}");
                }
                else
                {
                    Playerbuffs.AddBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage($"成功为添加一个永久buff {TShock.Utils.GetBuffName(buffid)}!");
                    ply.SendSuccessMessage($"{args.Player.Name} 为你添加一个永久buff {TShock.Utils.GetBuffName(buffid)}");
                }
            }
            else
            {
                args.Player.SendErrorMessage("无效的buffid");
            }
        }
        else
        {
            args.Player.SendErrorMessage("语法错误! 请输入/gpermabuff <玩家名> <buffid>");
        }
    }

    private void PAbuff(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            if (int.TryParse(args.Parameters[0], out int buffid))
            {
                if (config.LimitBuffs.Contains(buffid))
                {
                    args.Player.SendErrorMessage("此buff不可被添加!");
                    return;
                }
                var playerbuffs = Playerbuffs.GetBuffs(args.Player.Name);
                if (playerbuffs.Contains(buffid))
                {
                    Playerbuffs.DelBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage($"移除一个永久buff {TShock.Utils.GetBuffName(buffid)}!");
                }
                else
                {
                    Playerbuffs.AddBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage($"成功为自己添加一个永久buff {TShock.Utils.GetBuffName(buffid)}!");
                }
            }
            else
            {
                args.Player.SendErrorMessage("无效的buffid");
            }
        }
        else
        {
            args.Player.SendErrorMessage("语法错误! 请输入/permabuff <buffid>");
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null)
            players.Remove(ply);
    }

    private void Update(EventArgs args)
    {
        TimerCount++;
        if (TimerCount % 300 == 0)
            UpBuffs();
    }

    private void UpBuffs()
    {
        players.ForEach(x =>
        {
            Playerbuffs.GetBuffs(x.Name).ForEach(f =>
            {
                x.SetBuff(f, 18000, true);
            });
        });
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null && !args.Handled)
            players.Add(ply);
    }
}
