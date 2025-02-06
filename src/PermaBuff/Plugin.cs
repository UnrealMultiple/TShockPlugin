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

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 5);

    private readonly string PATH = Path.Combine(TShock.SavePath, "permbuff.json");

    private Config config = new();

    private long TimerCount = 0;

    private readonly HashSet<TSPlayer> players = new();

    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => this.LoadConfig();
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        this.LoadConfig();
        DB.Init();
        DB.ReadAll();
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.GameUpdate.Register(this, this.Update);
        Commands.ChatCommands.Add(new Command("permabuff.use", this.PAbuff, "permabuff", "pbuff"));
        Commands.ChatCommands.Add(new Command("gpermabuff.use", this.GPbuff, "gpermabuff", "gpbuff"));
        Commands.ChatCommands.Add(new Command("clearbuffs.use", this.Cbuff, "clearbuffs", "cbuff"));
        GeneralHooks.ReloadEvent += this._reloadHandler;
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.Update);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PAbuff || x.CommandDelegate == this.GPbuff || x.CommandDelegate == this.Cbuff);
            GeneralHooks.ReloadEvent -= this._reloadHandler;
        }
        base.Dispose(disposing);
    }
    private void LoadConfig()
    {
        if (File.Exists(this.PATH))
        {
            try
            {
                this.config = Config.Read(this.PATH);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"permabuff.json 读取错误:{ex}"));
            }
        }
        this.config.Write(this.PATH);
    }

    private void Cbuff(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            var buffs = Playerbuffs.GetBuffs(args.Player.Name);
            if (buffs.Count == 0)
            {
                args.Player.SendSuccessMessage(GetString("没有永久buff，无需清空"));
            }
            else
            {
                buffs.Clear();
                buffs.TrimExcess();
                args.Player.SendSuccessMessage(GetString("已清空所有永久buff"));
                args.Player.SendData(PacketTypes.PlayerBuff, null, args.Player.Index);
            }
        }

        else if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "all")
        {
            if (args.Player.HasPermission("clearbuffs.admin"))
            {
                Playerbuffs.PlayerBuffs.Clear();
                DB.ClearTable();
                args.Player.SendSuccessMessage(GetString("已清空所有玩家永久buff"));
                foreach (var plr in TShock.Players.Where(p => p != null && p.IsLoggedIn && p.Active))
                {
                    plr.SendData(PacketTypes.PlayerBuff, null, plr.Index);
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("权限不足，无法执行此命令"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("未知参数，请使用 '/clearbuffs' 或 '/cbuff all'"));
        }
    }

    private void GPbuff(CommandArgs args)
    {
        if (args.Parameters.Count == 2)
        {
            var ply = TSPlayer.FindByNameOrID(args.Parameters[1]).Find(x => x.Name == args.Parameters[1]);
            if (ply == null)
            {
                args.Player.SendErrorMessage(GetString("玩家不存在或玩家不在线!"));
                return;
            }
            if (int.TryParse(args.Parameters[0], out var buffid))
            {
                if (this.config.LimitBuffs.Contains(buffid))
                {
                    args.Player.SendErrorMessage(GetString("此buff不可被添加!"));
                    return;
                }
                var playerbuffs = Playerbuffs.GetBuffs(ply.Name);
                if (playerbuffs.Contains(buffid))
                {
                    Playerbuffs.DelBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage(GetString($"移除一个永久buff {TShock.Utils.GetBuffName(buffid)}!"));
                    ply.SendSuccessMessage(GetString($"{args.Player.Name} 移除了你的一个永久buff {TShock.Utils.GetBuffName(buffid)}"));
                }
                else
                {
                    Playerbuffs.AddBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage(GetString($"成功为添加一个永久buff {TShock.Utils.GetBuffName(buffid)}!"));
                    ply.SendSuccessMessage(GetString($"{args.Player.Name} 为你添加一个永久buff {TShock.Utils.GetBuffName(buffid)}"));
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("无效的buffid"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("语法错误! 请输入/gpermabuff <buffid> <玩家名>"));
        }
    }

    private void PAbuff(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            if (int.TryParse(args.Parameters[0], out var buffid))
            {
                if (this.config.LimitBuffs.Contains(buffid))
                {
                    args.Player.SendErrorMessage(GetString("此buff不可被添加!"));
                    return;
                }
                var playerbuffs = Playerbuffs.GetBuffs(args.Player.Name);
                if (playerbuffs.Contains(buffid))
                {
                    Playerbuffs.DelBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage(GetString($"移除一个永久buff {TShock.Utils.GetBuffName(buffid)}!"));
                }
                else
                {
                    Playerbuffs.AddBuff(args.Player.Name, buffid);
                    args.Player.SendSuccessMessage(GetString($"成功为自己添加一个永久buff {TShock.Utils.GetBuffName(buffid)}!"));
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("无效的buffid"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("语法错误! 请输入/permabuff <buffid>"));
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null)
        {
            this.players.Remove(ply);
        }
    }

    private void Update(EventArgs args)
    {
        this.TimerCount++;
        if (this.TimerCount % 300 == 0)
        {
            this.UpBuffs();
        }
    }

    private void UpBuffs()
    {
        this.players.ForEach(x => Playerbuffs.GetBuffs(x.Name).ForEach(f => x.SetBuff(f, 18000, true)));
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var ply = TShock.Players[args.Who];
        if (ply != null && !args.Handled)
        {
            this.players.Add(ply);
        }
    }
}