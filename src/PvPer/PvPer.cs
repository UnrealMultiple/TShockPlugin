using Microsoft.Data.Sqlite;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PvPer;

[ApiVersion(2, 1)]
public class PvPer : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 6);
    public override string Author => "Soofa 羽学修改";
    public override string Description => GetString("不是你死就是我活系列");
    public PvPer(Main game) : base(game)
    {
    }

    public static Configuration Config = new Configuration();
    public static DbManager DbManager = new DbManager(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "决斗系统.sqlite")));
    public static List<Pair> Invitations = new List<Pair>();
    public static List<Pair> ActiveDuels = new List<Pair>();

    #region PVP 检查
    readonly List<int> illegalMeleePrefixes = new List<int>();
    readonly List<int> illegalRangedPrefixes = new List<int>();
    readonly List<int> illegalMagicPrefixes = new List<int>();
    public List<string> weaponbans = null!;
    public List<int> buffbans = null!;
    readonly string path = Path.Combine(TShock.SavePath, "决斗系统.json");
    #endregion

    public override void Initialize()
    {
        LoadConfig();
        GetDataHandlers.PlayerTeam += OnPlayerChangeTeam;
        GetDataHandlers.TogglePvp += OnPlayerTogglePvP;
        GetDataHandlers.Teleport += OnPlayerTeleport;
        GetDataHandlers.PlayerUpdate += this.OnPlayerUpdate;
        GetDataHandlers.KillMe += this.OnKill;
        ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
        GeneralHooks.ReloadEvent += LoadConfig;
        TShockAPI.Commands.ChatCommands.Add(new Command("pvper.use", Commands.Duel, "决斗", "pvp"));
        TShockAPI.Commands.ChatCommands.Add(new Command("pvper.use", this.BuffList, "pvp.bl"));
        TShockAPI.Commands.ChatCommands.Add(new Command("pvper.use", this.WeaponList, "pvp.wl"));
        TShockAPI.Commands.ChatCommands.Add(new Command("pvper.admin", this.BanWeapon, "pvp.bw"));
        TShockAPI.Commands.ChatCommands.Add(new Command("pvper.admin", this.BanBuff, "pvp.bb"));

        #region PVP 检查
        this.weaponbans = new List<string>(Config.WeaponList);
        this.buffbans = new List<int>(Config.BuffList);
        this.illegalMeleePrefixes.AddRange(DataIDs.RangedPrefixIDs);
        this.illegalMeleePrefixes.AddRange(DataIDs.MagicPrefixIDs);
        this.illegalRangedPrefixes.AddRange(DataIDs.MeleePrefixIDs);
        this.illegalRangedPrefixes.AddRange(DataIDs.MagicPrefixIDs);
        this.illegalMagicPrefixes.AddRange(DataIDs.MeleePrefixIDs);
        this.illegalMagicPrefixes.AddRange(DataIDs.RangedPrefixIDs);
        #endregion
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.illegalMeleePrefixes.Clear();
            GetDataHandlers.PlayerTeam -= OnPlayerChangeTeam;
            GetDataHandlers.TogglePvp -= OnPlayerTogglePvP;
            GetDataHandlers.Teleport -= OnPlayerTeleport;
            GetDataHandlers.PlayerUpdate -= this.OnPlayerUpdate;
            GetDataHandlers.KillMe -= this.OnKill;
            ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
            GeneralHooks.ReloadEvent -= LoadConfig;
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Duel || x.CommandDelegate == this.BuffList || x.CommandDelegate == this.WeaponList || x.CommandDelegate == this.BanWeapon || x.CommandDelegate == this.BanBuff);
        }
        base.Dispose(disposing);
    }
    #region 创建与加载配置文件方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        var configPath = Configuration.FilePath;

        if (File.Exists(configPath))
        {
            Config = Configuration.Read(configPath);
            Console.WriteLine(GetString($"[决斗系统]已重载"));
        }
        else
        {
            Config = new Configuration();
            Config.Write(configPath);
        }
    }
    #endregion

    #region Hooks
    public void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
    {
        var plr = TShock.Players[args.PlayerId];
        var name = plr.Name;
        if (plr == null)
        {
            return;
        }

        if (Utils.IsPlayerInADuel(args.PlayerId) && !Utils.IsPlayerInArena(plr))
        {
            if (Config.KillPlayer)
            {
                plr.KillPlayer();
                TSPlayer.All.SendMessage(GetString($"{name}[c/E84B54:逃离]竞技场! 已判定为[C/13A1D1:怯战] 执行惩罚：[C/F86565:死亡]"), Color.Yellow);
                return;
            }
            else
            {
                plr.DamagePlayer(Config.SlapPlayer);
                plr.SendMessage(GetString($"[c/E84B54:禁止逃离竞技场!]惩罚：[c/F86565:扣{Config.SlapPlayer}血]"), Color.Yellow);
            }

            if (Config.PullArena)
            {
                var playerX = plr.TPlayer.Center.X;
                var playerY = plr.TPlayer.Center.Y;

                // 计算玩家到竞技场中心的向量（dx, dy）
                var centerX = ((PvPer.Config.ArenaPosX1 * 16) + (PvPer.Config.ArenaPosX2 * 16)) / 2f;
                var centerY = ((PvPer.Config.ArenaPosY1 * 16) + (PvPer.Config.ArenaPosY2 * 16)) / 2f;
                var dx = centerX - playerX;
                var dy = centerY - playerY;

                // 确保拉取半径在竞技场范围内
                var maxR = Math.Max(Math.Abs((PvPer.Config.ArenaPosX1 * 16) - centerX), Math.Abs((PvPer.Config.ArenaPosY1 * 16) - centerY)) / 2f;
                var pullR = Math.Min(maxR, Config.PullRange);

                // 新增配置项：拉取到竞技场中心的距离范围
                float PullRange = Config.PullRange * 16;

                // 计算拉取目标坐标
                var targetX = (float) (centerX + (dx * PullRange / Math.Sqrt((dx * dx) + (dy * dy))));
                var targetY = (float) (centerY + (dy * PullRange / Math.Sqrt((dx * dx) + (dy * dy))));

                PullTP(plr, targetX, targetY, (int) Math.Max(pullR, 0));

                TSPlayer.All.SendMessage(GetString($"{name}[c/E84B54:逃离]竞技场! 执行：[C/4284CD:拉回]"), Color.Yellow);
            }

            if (Config.CheckaAll)
            {
                if (plr == null || !plr.TPlayer.hostile || !args.Control.IsUsingItem)
                {
                    return;
                }

                if (!plr.HasPermission("pvper.admin"))
                {
                    this.CheckWeapons(plr);
                    this.CheckBuffs(plr);
                    this.CheckIllegalPrefixWeapons(plr);
                    this.CheckPrefixAmmo(plr);
                    this.CheckPrefixArmor(plr);
                    this.CheckAccs(plr);
                    this.Check7AccsSlot(plr);
                }
            }
        }
    }

    #region 拉回竞技场方法

    //拉取玩家的方法
    public static void PullTP(TSPlayer plr, float x, float y, int r)
    {
        if (r <= 0)
        {
            plr.Teleport(x, y, 1);
            return;
        }
        var x2 = plr.TPlayer.Center.X;
        var y2 = plr.TPlayer.Center.Y;
        x2 -= x;
        y2 -= y;
        if (x2 != 0f || y2 != 0f)
        {
            var num = Math.Atan2(y2, x2) * 180.0 / Math.PI;
            x2 = (float) (r * Math.Cos(num * Math.PI / 180.0));
            y2 = (float) (r * Math.Sin(num * Math.PI / 180.0));
            x2 += x;
            y2 += y;
            plr.Teleport(x2, y2, 1);
        }
    }
    #endregion


    #region 原插件自带的处罚方式
    public void OnKill(object? sender, GetDataHandlers.KillMeEventArgs args)
    {
        var plr = TShock.Players[args.PlayerId];
        var duel = Utils.GetDuel(plr.Index);

        if (duel != null)
        {
            var winnerIndex = duel.Player1 == plr.Index ? duel.Player2 : duel.Player1;
            duel.EndDuel(winnerIndex);
        }
    }

    public static void OnServerLeave(LeaveEventArgs args)
    {
        var duel = Utils.GetDuel(args.Who);
        if (duel != null)
        {
            var winnerIndex = duel.Player1 == args.Who ? duel.Player2 : duel.Player1;
            duel.EndDuel(winnerIndex);
        }
    }

    public static void OnPlayerTogglePvP(object? sender, GetDataHandlers.TogglePvpEventArgs args)
    {
        var plr = TShock.Players[args.PlayerId];
        var duel = Utils.GetDuel(args.PlayerId);

        if (duel != null)
        {
            args.Handled = true;
            plr.TPlayer.hostile = true;
            plr.SendData(PacketTypes.TogglePvp, number: plr.Index);
        }
    }
    public static void OnPlayerTeleport(object? sender, GetDataHandlers.TeleportEventArgs args)
    {
        var duel = Utils.GetDuel(args.ID);

        if (duel != null && Config.KillPlayer && !Utils.IsLocationInArena((int) (args.X / 16), (int) (args.Y / 16)))
        {
            args.Player.KillPlayer();
        }
    }

    public static void OnPlayerChangeTeam(object? sender, GetDataHandlers.PlayerTeamEventArgs args)
    {
        var plr = TShock.Players[args.PlayerId];
        var duel = Utils.GetDuel(args.PlayerId);

        if (duel != null)
        {
            args.Handled = true;
            plr.TPlayer.team = 0;
            plr.SendData(PacketTypes.PlayerTeam, number: plr.Index);
        }
    }
    #endregion


    #region 检查使用的命令

    public void WeaponList(CommandArgs args)
    {
        if (args.Player == null)
        {
            return;
        }

        var str = GetString("以下武器在PvP中禁止使用： ");
        var num = 0;
        foreach (var weapon in this.weaponbans)
        {
            if (num != 0)
            {
                str += ", ";
            }

            str += weapon;
            ++num;
        }
        args.Player.SendInfoMessage(str + ".");
    }

    public void BuffList(CommandArgs args)
    {
        if (args.Player == null)
        {
            return;
        }

        var str = GetString("以下增益效果在PvP中禁止使用： ");
        var num = 0;
        foreach (var buff in this.buffbans)
        {
            if (num != 0)
            {
                str += ", ";
            }

            str += TShock.Utils.GetBuffName(buff);
            ++num;
        }
        args.Player.SendInfoMessage(str + ".");
    }

    public void BanWeapon(CommandArgs args)
    {
        if (args.Parameters.Count >= 2 && (args.Parameters[0] == "add" || args.Parameters[0] == "del"))
        {
            var input = string.Join(" ", args.Parameters.Skip(1).ToArray());
            string wname;

            var wlist = new List<string>();
            var foundweapons = TShock.Utils.GetItemByName(input);

            foreach (var item in foundweapons)
            {
                wlist.Add(item.Name);
            }

            if (wlist.Count < 1)
            {
                args.Player.SendErrorMessage(GetString("未找到任何以此名称命名的物品。"));
                return;

            }
            if (wlist.Count > 1)
            {
                args.Player.SendMultipleMatchError(wlist);
                return;
            }
            else
            {
                wname = wlist[0];
            }

            switch (args.Parameters[0])
            {
                case "add":
                    this.weaponbans.Add(wname);
                    Config.WeaponList.Add(wname); // 添加到配置文件的 WeaponList
                    args.Player.SendSuccessMessage(GetString($"已禁止 {wname} 在PvP中使用。"));
                    PvPer.Config.Write(Configuration.FilePath);
                    break;

                case "del":
                    this.weaponbans.Remove(wname);
                    Config.WeaponList.Remove(wname); // 从配置文件的 WeaponList 移除
                    args.Player.SendSuccessMessage(GetString($"已解除对 {wname} 在PvP中的禁用。"));
                    PvPer.Config.Write(Configuration.FilePath);
                    break;
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("命令格式不正确。 /pvp.bw add|del <武器名>"));
        }
    }

    public void BanBuff(CommandArgs args)
    {
        if (args.Parameters.Count == 2)
        {

            if (!int.TryParse(args.Parameters[1], out var buffid))
            {
                var blist = new List<int>();
                blist = TShock.Utils.GetBuffByName(args.Parameters[2]);

                if (blist.Count < 1)
                {
                    args.Player.SendErrorMessage(GetString("未找到以此名称命名的增益效果。"));
                    return;
                }
                else if (blist.Count > 1)
                {
                    args.Player.SendMultipleMatchError(blist.Select(p => TShock.Utils.GetBuffName(p)));
                    return;
                }
                else
                {
                    buffid = blist[0];
                }
            }

            switch (args.Parameters[0])
            {
                case "add":
                    this.buffbans.Add(buffid);
                    Config.BuffList.Add(buffid);
                    args.Player.SendMessage(GetString($"已禁止 {TShock.Utils.GetBuffName(buffid)} 在PvP中使用。"), 232, 74, 83);
                    PvPer.Config.Write(Configuration.FilePath);
                    break;

                case "del":
                    this.buffbans.Remove(buffid);
                    Config.BuffList.Remove(buffid);
                    args.Player.SendMessage(GetString($"已解除对 {TShock.Utils.GetBuffName(buffid)} 在PvP中的禁用。"), 238, 232, 76);
                    PvPer.Config.Write(Configuration.FilePath);
                    break;

                default:
                    args.Player.SendErrorMessage(GetString("命令格式不正确。 /pvp.bb add|del <增益名/ID>"));
                    break;
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("命令格式不正确。 /pvp.bb add|del <增益名/ID>"));
        }
    }

    #endregion


    #region 检查需要的反馈
    public void CheckWeapons(TSPlayer plr)
    {
        foreach (var bannedWeapon in this.weaponbans)
        {
            if (plr.ItemInHand.Name == bannedWeapon || plr.SelectedItem.Name == bannedWeapon)
            {
                this.DisableAndSendErrorMessage(plr, GetString("使用了禁止的PvP武器"), GetString($"{bannedWeapon} 在PvP中无法使用，请查看 /pvp.wl"));
                break;
            }
        }
    }

    public void CheckBuffs(TSPlayer plr)
    {
        foreach (var bannedBuff in this.buffbans)
        {
            foreach (var playerBuff in plr.TPlayer.buffType)
            {
                if (playerBuff == bannedBuff)
                {
                    this.DisableAndSendErrorMessage(plr, GetString("使用了禁止的增益效果"), GetString($"{TShock.Utils.GetBuffName(playerBuff)} 在PvP中不可使用，请查看 /pvp.bl"));
                    break;
                }
            }
        }
    }

    public void CheckIllegalPrefixWeapons(TSPlayer plr)
    {
        if (this.IsStackableAndPrefix(plr.ItemInHand) || this.IsStackableAndPrefix(plr.SelectedItem))
        {
            this.DisableAndSendErrorMessage(plr, GetString("使用了非法前缀武器"), GetString("PvP中不允许使用非法前缀武器"));
        }
        else if (this.IsMeleeAndHasIllegalPrefix(plr.ItemInHand, this.illegalMeleePrefixes.ToArray()) ||
                 this.IsMeleeAndHasIllegalPrefix(plr.SelectedItem, this.illegalMeleePrefixes.ToArray()))
        {
            this.DisableAndSendErrorMessage(plr, GetString("使用了非法前缀武器"), GetString("PvP中不允许使用非法前缀武器"));
        }
        else if (this.IsRangedAndHasIllegalPrefix(plr.ItemInHand, this.illegalRangedPrefixes.ToArray()) ||
                 this.IsRangedAndHasIllegalPrefix(plr.SelectedItem, this.illegalRangedPrefixes.ToArray()))
        {
            this.DisableAndSendErrorMessage(plr, GetString("使用了非法前缀武器"), GetString("PvP中不允许使用非法前缀武器"));
        }
        else if (this.IsMagicOrSummonedAndHasIllegalPrefix(plr.ItemInHand, this.illegalMagicPrefixes.ToArray()) ||
                 this.IsMagicOrSummonedAndHasIllegalPrefix(plr.SelectedItem, this.illegalMagicPrefixes.ToArray()))
        {
            this.DisableAndSendErrorMessage(plr, GetString("使用了非法前缀武器"), GetString("PvP中不允许使用非法前缀武器"));
        }
    }

    public void CheckPrefixAmmo(TSPlayer plr)
    {
        foreach (var ammo in DataIDs.ammoIDs)
        {
            foreach (var inventoryItem in plr.TPlayer.inventory)
            {
                if (inventoryItem.netID == ammo && inventoryItem.prefix != 0)
                {
                    this.DisableAndSendErrorMessage(plr, GetString("使用了前缀弹药"), GetString($"请移除PvP中的前缀弹药：{inventoryItem.Name}"));
                    break;
                }
            }
        }
    }

    public void CheckPrefixArmor(TSPlayer plr)
    {
        for (var index = 0; index < 3; index++)
        {
            if (plr.TPlayer.armor[index].prefix != 0)
            {
                this.DisableAndSendErrorMessage(plr, GetString("使用了前缀护甲"), GetString($"请移除PvP中的前缀护甲：{plr.TPlayer.armor[index].Name}"));
                break;
            }
        }
    }

    public void CheckAccs(TSPlayer plr)
    {
        var duplicateItems = new Dictionary<int, bool>();
        foreach (var equippedItem in plr.TPlayer.armor)
        {
            if (duplicateItems.ContainsKey(equippedItem.netID))
            {
                this.DisableAndSendErrorMessage(plr, GetString("使用了重复饰品"), GetString($"请移除PvP中的重复饰品：{equippedItem.Name}"));
                break;
            }
            else if (equippedItem.netID != 0)
            {
                duplicateItems.Add(equippedItem.netID, true);
            }
        }
    }

    public void Check7AccsSlot(TSPlayer plr)
    {
        if (Config.Check7 || plr.TPlayer.armor[9].netID != 0)
        {
            this.DisableAndSendErrorMessage(plr, GetString("使用了第七个饰品槽"), GetString("PvP中不允许使用第七个饰品槽"));
        }
    }

    public bool IsStackableAndPrefix(Item item)
    {
        return item.maxStack > 1 && item.prefix != 0;
    }

    public bool IsMeleeAndHasIllegalPrefix(Item item, int[] illegalPrefixes)
    {
        return item.melee && this.HasIllegalPrefix(item, illegalPrefixes);
    }

    public bool IsRangedAndHasIllegalPrefix(Item item, int[] illegalPrefixes)
    {
        return item.ranged && this.HasIllegalPrefix(item, illegalPrefixes);
    }

    public bool IsMagicOrSummonedAndHasIllegalPrefix(Item item, int[] illegalPrefixes)
    {
        return (item.magic || item.summon || item.DD2Summon) && this.HasIllegalPrefix(item, illegalPrefixes);
    }

    public bool HasIllegalPrefix(Item item, int[] illegalPrefixes)
    {
        return illegalPrefixes.Contains(item.prefix);
    }

    public void DisableAndSendErrorMessage(TSPlayer player, string disableReason, string errorMessage)
    {
        player.Disable(disableReason, DisableFlags.WriteToLog);
        player.SendErrorMessage(errorMessage);
    }


    #endregion


    #endregion
}