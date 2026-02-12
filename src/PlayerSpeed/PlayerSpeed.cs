using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static PlayerSpeed.Configuration;

namespace PlayerSpeed;

[ApiVersion(2, 1)]
public class PlayerSpeed : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "逆光奔跑 羽学";
    public override Version Version => new Version(1, 2, 6);
    public override string Description => GetString("使用指令设置玩家移动速度 并在冲刺或穿上自定义装备跳跃时触发");
    #endregion

    #region 注册与释放
    public static Database DB = new();
    public PlayerSpeed(Main game) : base(game) { }
    public override void Initialize()
    {
        ServerApi.Hooks.NpcKilled.Register(this, this.NpcKilled);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        TShockAPI.Commands.ChatCommands.Add(new Command("vel.use", Commands.vel, "vel", "速度"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcKilled.Deregister(this, this.NpcKilled);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.vel);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 创建玩家数据方法
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (args == null || !Configuration.Instance.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Who];

        if (plr == null)
        {
            return;
        }

        var data = DB.GetData(plr.Name);

        // 如果玩家不在数据表中，则创建新数据
        if (data == null)
        {
            var newData = new Database.PlayerData
            {
                Name = plr.Name,
                Enabled = false,
                Count = 0,
                CoolTime = DateTime.UtcNow,
            };
            DB.AddData(newData); // 添加到数据库
        }
        else
        {
            data.Count = 0;
            DB.UpdateData(data);
        }
    }
    #endregion

    #region 获取NPCID的中文名
    private static void WriteName()
    {
        foreach (var group in Configuration.Instance.BossList)
        {
            var Names = new HashSet<string>(group.Name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            foreach (var id in group.ID)
            {
                string npcName;
                npcName = (string) Lang.GetNPCName(id);
                if (!Names.Contains(npcName))
                {
                    Names.Add(npcName);
                }
            }
            group.Name = string.Join(", ", Names);
        }
    }
    #endregion

    #region NPC死亡事件 自动计算速度值
    internal static Dictionary<int, HashSet<int>> BossDowned = new Dictionary<int, HashSet<int>>();
    private void NpcKilled(NpcKilledEventArgs args)
    {
        if (args.npc == null || !Configuration.Instance.Enabled)
        {
            return;
        }

        //循环遍历进度表
        foreach (var npc in Configuration.Instance.BossList)
        {
            //如果是进度表里的NPC
            if (npc.ID.Contains(args.npc.netID))
            {
                if (!BossDowned.ContainsKey(npc.ID.First()))
                {
                    BossDowned[npc.ID.First()] = new HashSet<int>();
                    npc.Enabled = true;
                    Configuration.Instance.SaveTo();
                }
            }
        }
    }
    #endregion

    #region 触发加速方法
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        var tplr = plr.TPlayer;
        var data = DB.GetData(plr.Name);
        if (plr == null || data == null || !Configuration.Instance.Enabled || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("vel.use"))
        {
            return;
        }

        //没开启进度模式 按Config的值来设置
        if (!Configuration.Instance.KilledBoss)
        {
            AddSpeed(plr, tplr, data, Configuration.Instance.CoolTime, Configuration.Instance.Count, Configuration.Instance.Speed, Configuration.Instance.Height);
        }
        else
        {
            var boss = GetMaxSpeed(Configuration.Instance.BossList);
            if (boss != null && boss.Enabled)
            {
                AddSpeed(plr, tplr, data, boss.CoolTime, boss.Count, boss.Speed, boss.Height);
            }
        }
    }
    #endregion

    #region 加速方法核心逻辑
    private static void AddSpeed(TSPlayer plr, Player tplr, Database.PlayerData? data, int CoolTime, int Count, float Speed, float Height)
    {
        var now = DateTime.UtcNow;
        var CRight = tplr.controlRight && tplr.direction == 1;
        var CLeft = tplr.controlLeft && tplr.direction == -1;
        var armor = tplr.armor.Take(20).Any(x => Configuration.Instance.ArmorItem != null && Configuration.Instance.ArmorItem.Contains(x.type));
        if (data == null)
        {
            return;
        }
        var LastCool = data.CoolTime == default ? 0f : (float) Math.Round((now - data.CoolTime).TotalSeconds, 2);

        //计算冷却过去多久
        if (!data.Enabled && LastCool >= CoolTime)
        {
            data.Count = 0;
            data.Enabled = true;
            DB.UpdateData(data);
            if (Configuration.Instance.Mess)
            {
                plr.SendMessage(GetString($"\n玩家 [c/4EA4F2:{plr.Name}] 疾速[c/FF5265:冷却]完毕!"), 244, 255, 150);

                if (Configuration.Instance.Dash)
                {
                    plr.SendMessage(GetString("使用克盾类饰品[c/47DCBD:冲刺]可冲更远"), 244, 255, 150);
                }

                if (Configuration.Instance.Jump)
                {
                    plr.SendMessage(GetString("装备指定物品[c/47DCBC:可加速跳跃]!"), 244, 255, 150);
                }
            }
        }

        //使用了多少次进入冷却
        if (data.Enabled && data.Count >= Count && !plr.HasPermission("vel.admin"))
        {
            data.Enabled = false;
            data.CoolTime = now;
            DB.UpdateData(data);
            if (Configuration.Instance.Mess)
            {
                plr.SendMessage(GetString($"\n玩家 [c/4EA4F2:{plr.Name}] 因[c/FF5265:冲刺]或[c/DBF34E:跳跃] 超过[c/47DCBD:{Count}次]进入冷却"), 244, 255, 150);
            }
            return;
        }

        //冲刺
        if (Configuration.Instance.Dash && data.Enabled && CheckDash(tplr, data, CLeft, CRight))
        {
            tplr.velocity.X = Speed * Configuration.Instance.DashMultiple * tplr.direction;

            if (CLeft && tplr.controlUp)
            {
                tplr.velocity.Y = Height * tplr.direction;
            }

            if (CRight && tplr.controlUp)
            {
                tplr.velocity.Y = -Height * tplr.direction;
            }

            if (CLeft && tplr.controlDown)
            {
                tplr.velocity.Y = Height * tplr.direction;
            }

            if (CRight && tplr.controlDown)
            {
                tplr.velocity.Y = -Height * tplr.direction;
            }

            data.Count++;
            DB.UpdateData(data);
            TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", plr.Index, 0f, 0f, 0f, 0);
        }

        //跳跃
        if (Configuration.Instance.Jump && data.Enabled && armor && CheckJupm(tplr, data, CLeft, CRight))
        {
            tplr.velocity.X = Speed * tplr.direction;

            if (CLeft && tplr.controlUp)
            {
                tplr.velocity.Y = Height * tplr.direction;
            }

            if (CRight && tplr.controlUp)
            {
                tplr.velocity.Y = -Height * tplr.direction;
            }

            if (CLeft && tplr.controlDown)
            {
                tplr.velocity.Y = Height / Configuration.Instance.JumpMultiple / tplr.direction;
            }

            if (CRight && tplr.controlDown)
            {
                tplr.velocity.Y = -Height / Configuration.Instance.JumpMultiple / tplr.direction;
            }

            data.Count++;
            DB.UpdateData(data);
            TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", plr.Index, 0f, 0f, 0f, 0);
        }
    }
    #endregion

    #region 检查玩家跳跃间隔方法
    private static bool CheckJupm(Player tplr, Database.PlayerData data, bool CLeft, bool CRight)
    {
        var now = DateTime.UtcNow;
        var Jump = tplr.controlJump;
        if ((now - data.RangeTime).TotalMilliseconds < Configuration.Instance.Range)
        {
            return false;
        }

        if ((Jump && CRight) || (CLeft && Jump))
        {
            data.RangeTime = now; // 更新最后一次跳跃的时间
            return true;
        }

        return false;
    }
    #endregion

    #region 检查玩家冲刺间隔方法
    private static bool CheckDash(Player tplr, Database.PlayerData data, bool CLeft, bool CRight)
    {
        var now = DateTime.UtcNow;
        var dash = tplr.dashDelay == -1;
        if ((now - data.RangeTime).TotalMilliseconds < Configuration.Instance.Range)
        {
            return false;
        }

        if ((dash && CRight) || (CLeft && dash))
        {
            data.RangeTime = now; // 更新最后一次跳跃的时间
            return true;
        }

        return false;
    }
    #endregion

    #region 获取进度模式的最大值
    internal static BossData? GetMaxSpeed(List<BossData> bossList)
    {
        if (bossList == null || bossList.Count == 0)
        {
            return null;
        }

        // 初始化最大值
        var maxBoss = bossList.Where(b => b.Enabled).FirstOrDefault()!;
        if (maxBoss == null)
        {
            return null; // 没有已启用的Boss
        }

        // 遍历列表寻找最大值
        foreach (var boss in bossList)
        {
            if (boss.Enabled &&
                (boss.Speed > maxBoss.Speed ||
                boss.Height > maxBoss.Height ||
                boss.Count > maxBoss.Count ||
                boss.CoolTime < maxBoss.CoolTime))
            {
                maxBoss = boss;
            }
        }
        return maxBoss;
    }
    #endregion
}