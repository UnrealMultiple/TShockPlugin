using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DisableGodMod;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public class LPlayer
    {
        public int Index { get; set; }

        public int Tr { get; set; }

        public int Dm { get; set; }

        public int LHp { get; set; }

        public int LMaxHp { get; set; }

        public bool Heal { get; set; }

        public bool Skip { get; set; }

        public bool BAA { get; set; }

        public int Mis { get; set; }

        public DateTime LastTiem { get; set; }

        public DateTime LCheckTiem { get; set; }

        public int KickL { get; set; }

        public DateTime LastTiemKickL { get; set; }

        public LPlayer(int index)
        {
            this.Tr = 0;
            this.Dm = 0;
            this.Mis = 0;
            this.LHp = 0;
            this.LMaxHp = 0;
            this.Skip = true;
            this.Heal = false;
            this.BAA = false;
            this.Index = index;
            this.LastTiem = DateTime.UtcNow;
            this.KickL = 0;
            this.LastTiemKickL = DateTime.UtcNow;
            this.LCheckTiem = DateTime.UtcNow;
        }
    }

    private static readonly System.Timers.Timer Update = new System.Timers.Timer(3000.0);

    public static bool ULock = false;

    public override string Author => "GK 修改：羽学";

    public override string Description => "如果玩家无敌那么就断开它！";

    public override string Name => "阻止玩家无敌";

    public override Version Version => new Version(1, 0, 2, 1);

    private LPlayer[] LPlayers { get; set; }

    public Plugin(Main game)
        : base(game)
    {
        this.LPlayers = new LPlayer[256];
        base.Order = 1000;
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.NetGetData.Register(this, this.GetData);
        ServerApi.Hooks.NpcStrike.Register(this, this.NpcStrike);
        ServerApi.Hooks.NetSendData.Register(this, this.SendData);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnSpawn);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.NetGetData.Deregister(this, this.GetData);
            ServerApi.Hooks.NetSendData.Deregister(this, this.SendData);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.NpcStrike);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnSpawn);
            Update.Elapsed -= this.OnUpdate;
            Update.Stop();
        }
        base.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        Update.Elapsed += this.OnUpdate;
        Update.Start();
    }

    public void OnUpdate(object sender, ElapsedEventArgs e)
    {
    }

    public static bool Timeout(DateTime Start, bool warn = true, int ms = 500)
    {
        var flag = (DateTime.Now - Start).TotalMilliseconds >= ms;
        if (flag)
        {
            ULock = false;
        }
        if (warn && flag)
        {
            TShock.Log.Error("阻止无敌超时,已抛弃部分提示");
        }
        return flag;
    }

    private void OnGreetPlayer(GreetPlayerEventArgs e)
    {
        lock (this.LPlayers)
        {
            this.LPlayers[e.Who] = new LPlayer(e.Who);
        }
    }

    private void OnLeave(LeaveEventArgs e)
    {
        lock (this.LPlayers)
        {
            if (this.LPlayers[e.Who] != null)
            {
                this.LPlayers[e.Who] = null;
            }
        }
    }

    private void OnSpawn(NpcSpawnEventArgs args)
    {
        if (args.Handled || !Main.npc[args.NpcId].boss || !Main.npc[args.NpcId].active)
        {
            return;
        }
        lock (this.LPlayers)
        {
            for (var i = 0; i < this.LPlayers.Length; i++)
            {
                if (this.LPlayers[i] != null)
                {
                    this.LPlayers[i].LCheckTiem = DateTime.UtcNow;
                    this.LPlayers[i].Tr = 0;
                    this.LPlayers[i].Mis = 0;
                }
            }
        }
    }

    private void SendData(SendDataEventArgs args)
    {
        if (args.Handled || args.MsgId != PacketTypes.PlayerHealOther)
        {
            return;
        }
        var number = args.number;
        if (number < 0)
        {
            return;
        }
        lock (this.LPlayers)
        {
            if (this.LPlayers[number] != null && this.LPlayers[number].Tr == 3)
            {
                this.LPlayers[number].Heal = true;
            }
        }
    }

    private void NpcStrike(NpcStrikeEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }
        lock (this.LPlayers)
        {
            if (this.LPlayers[args.Player.whoAmI] != null && (DateTime.UtcNow - this.LPlayers[args.Player.whoAmI].LCheckTiem).TotalMilliseconds > 600000.0)
            {
                this.LPlayers[args.Player.whoAmI].LCheckTiem = DateTime.UtcNow;
                this.LPlayers[args.Player.whoAmI].Tr = 0;
                this.LPlayers[args.Player.whoAmI].Mis = 0;
            }
        }
    }
    // 定义方法GetData用于获取并处理游戏数据
    private void GetData(GetDataEventArgs args)
    {
        // 获取当前处理的TSPlayer对象实例
        var tSPlayer = TShock.Players[args.Msg.whoAmI];
        // 如果玩家不存在、已断开连接、事件已被处理、或者该玩家在LPlayers数组中不存在，则直接返回
        if (tSPlayer == null || !tSPlayer.ConnectionAlive || args.Handled || this.LPlayers[args.Msg.whoAmI] == null || tSPlayer.Group.HasPermission("免检无敌") || tSPlayer.Group.Name == "owner")
        {
            return;
        }
        // 对LPlayers加锁以确保线程安全
        lock (this.LPlayers)
        {
            // 判断消息类型是否为玩家血量更新
            if (args.MsgID == PacketTypes.PlayerHp)
            {
                // 如果玩家当前正处于禁用状态，则直接返回
                if (tSPlayer.IsBeingDisabled())
                {
                    return;
                }
                // 从二进制流中读取相关数据
                using var binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
                var b = binaryReader.ReadByte();
                var num = binaryReader.ReadInt16();
                var num2 = binaryReader.ReadInt16();

                // 如果玩家的检测状态未初始化
                if (this.LPlayers[args.Msg.whoAmI].Tr == 0)
                {
                    // 如果玩家还未进行首次判断，则进行初始化
                    if (!this.LPlayers[args.Msg.whoAmI].BAA)
                    {
                        // 检查所有在线玩家，如果有任何玩家正在进行无敌检测，则跳过此次检查
                        for (var i = 0; i < this.LPlayers.Length; i++)
                        {
                            if (this.LPlayers[i] != null && this.LPlayers[i].BAA)
                            {
                                return;
                            }
                        }
                        this.LPlayers[args.Msg.whoAmI].BAA = true;
                    }
                    // 如果玩家已跳过此次伤害检测，并且超过1.5秒，则重置跳过状态
                    if (this.LPlayers[args.Msg.whoAmI].Skip && (DateTime.UtcNow - this.LPlayers[args.Msg.whoAmI].LastTiem).TotalMilliseconds > 1500.0)
                    {
                        this.LPlayers[args.Msg.whoAmI].Skip = false;
                    }
                    // 如果血量大于1且未跳过此次伤害检测，则开始无敌检测流程
                    if (num > 1 && !this.LPlayers[args.Msg.whoAmI].Skip)
                    {
                        this.LPlayers[args.Msg.whoAmI].Tr = 1;
                        this.LPlayers[args.Msg.whoAmI].LHp = num;
                        tSPlayer.DamagePlayer(1);
                    }
                }
                // 如果玩家正处于无敌检测流程中
                else if (this.LPlayers[args.Msg.whoAmI].Tr == 1)
                {
                    if (this.LPlayers[args.Msg.whoAmI].LHp != num)
                    {
                        this.LPlayers[args.Msg.whoAmI].Tr = 2;
                        this.LPlayers[args.Msg.whoAmI].BAA = false;
                    }
                    // 否则继续无敌检测流程
                    else
                    {
                        // 如果已累计错误次数达到1次及以上
                        if (this.LPlayers[args.Msg.whoAmI].Mis >= 1)
                        {
                            // 踢出玩家，并显示无敌违规提示
                            tSPlayer.Kick($"玩家 {tSPlayer.Name} 因无敌被踢出.", force: true, silent: false, "Server");
                            return;
                        }
                        this.LPlayers[args.Msg.whoAmI].Mis++;
                        this.LPlayers[args.Msg.whoAmI].Tr = 0;
                        // 重置跳过状态（同上）
                        if (this.LPlayers[args.Msg.whoAmI].Skip && (DateTime.UtcNow - this.LPlayers[args.Msg.whoAmI].LastTiem).TotalMilliseconds > 1500.0)
                        {
                            this.LPlayers[args.Msg.whoAmI].Skip = false;
                        }
                        // 继续无敌检测流程（同上）
                        if (num > 1 && !this.LPlayers[args.Msg.whoAmI].Skip)
                        {
                            this.LPlayers[args.Msg.whoAmI].Tr = 1;
                            this.LPlayers[args.Msg.whoAmI].LHp = num;
                            tSPlayer.DamagePlayer(1);
                        }
                    }
                }
                //血量溢出检测...

                //else if (LPlayers[args.Msg.whoAmI].Tr == 2 && num > tSPlayer.TPlayer.statLifeMax2 && num > tSPlayer.TPlayer.statLifeMax2 + (num2 - tSPlayer.TPlayer.statLifeMax))
                //{
                //    if ((DateTime.UtcNow - LPlayers[args.Msg.whoAmI].LastTiemKickL).TotalMilliseconds > 30000.0)
                //    {
                //        LPlayers[args.Msg.whoAmI].KickL = 0;
                //        LPlayers[args.Msg.whoAmI].LastTiemKickL = DateTime.UtcNow;
                //    }
                //    LPlayers[args.Msg.whoAmI].KickL++;
                //    if (LPlayers[args.Msg.whoAmI].KickL > 3)
                //    {
                //        tSPlayer.Kick($"玩家 {tSPlayer.Name} 血量溢出被踢出.", force: true, silent: false, "Server");
                //    }
                //    return;
                //}

                // 更新玩家当前和最大血量
                if (this.LPlayers[args.Msg.whoAmI].LMaxHp != 0 && num2 != this.LPlayers[args.Msg.whoAmI].LMaxHp)
                {
                    // 检测玩家是否非法修改血量上限，如有异常将踢出玩家
                    // 若玩家原血量上限在400至500之间，且新血量上限不是增加5点或10点，则视为异常
                    if (this.LPlayers[args.Msg.whoAmI].LMaxHp >= 400 && this.LPlayers[args.Msg.whoAmI].LMaxHp < 500)
                    {
                        if (num2 != this.LPlayers[args.Msg.whoAmI].LMaxHp + 5 && num2 != this.LPlayers[args.Msg.whoAmI].LMaxHp + 10)
                        {
                            var text = $"玩家 {tSPlayer.Name} 修改血量上限({this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2 - this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2})";
                            // 封禁玩家账号，并给出封禁原因，此处为模拟注释，实际操作请替换对应API调用
                            //TShock.Bans.InsertBan($"{Identifier.Account}{tSPlayer.Account.Name}", text + "被封号.", "阻止玩家无敌", DateTime.UtcNow, DateTime.MaxValue);
                            tSPlayer.Kick(text + "被踢出.", force: true, silent: false, "Server");
                            return;
                        }
                    }
                    // 若玩家原血量上限小于400，且新血量上限不是增加20点或40点，则视为异常
                    else if (this.LPlayers[args.Msg.whoAmI].LMaxHp < 400)
                    {
                        if (num2 != this.LPlayers[args.Msg.whoAmI].LMaxHp + 20 && num2 != this.LPlayers[args.Msg.whoAmI].LMaxHp + 40)
                        {
                            var text2 = $"玩家 {tSPlayer.Name} 修改血量上限({this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2 - this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2})";
                            // 封禁玩家账号，并给出封禁原因，此处为模拟注释，实际操作请替换对应API调用
                            //TShock.Bans.InsertBan($"{Identifier.Account}{tSPlayer.Account.Name}", text2 + "被封号.", "阻止玩家无敌", DateTime.UtcNow, DateTime.MaxValue);
                            tSPlayer.Kick(text2 + "被踢出.", force: true, silent: false, "Server");
                            return;
                        }
                    }
                    // 若玩家的新血量上限大于原血量上限，且不在上述合法范围内，则视为异常
                    else if (num2 > this.LPlayers[args.Msg.whoAmI].LMaxHp)
                    {
                        var text3 = $"玩家 {tSPlayer.Name} 修改血量上限({this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2 - this.LPlayers[args.Msg.whoAmI].LMaxHp}>{num2})";
                        // 封禁玩家账号，并给出封禁原因，此处为模拟注释，实际操作请替换对应API调用
                        //TShock.Bans.InsertBan($"{Identifier.Account}{tSPlayer.Account.Name}", text3 + "被封号.", "阻止玩家无敌", DateTime.UtcNow, DateTime.MaxValue);
                        tSPlayer.Kick(text3 + "被踢出.", force: true, silent: false, "Server");
                        return;
                    }
                }
                // 更新玩家当前血量和最大血量数值
                this.LPlayers[args.Msg.whoAmI].LHp = num;
                this.LPlayers[args.Msg.whoAmI].LMaxHp = num2;
                return;
            }
            // 检查不同消息类型并作出相应处理
            if (args.MsgID == PacketTypes.PlayerHurtV2)
            {
                // 若玩家防御力低于200且处于特定检测状态
                if (this.LPlayers[args.Msg.whoAmI].Tr == 2 && tSPlayer.TPlayer.statDefense <= 199)
                {
                    // 此处可能需要添加针对玩家防御力低的具体处理逻辑，目前为空
                }
            }
            else if (args.MsgID == PacketTypes.PlayerStealth)
            {
                // 若玩家处于未检测状态（Tr=0），则标记跳过下次伤害检测，并记录当前时间
                if (this.LPlayers[args.Msg.whoAmI].Tr == 0)
                {
                    this.LPlayers[args.Msg.whoAmI].Skip = true;
                    this.LPlayers[args.Msg.whoAmI].LastTiem = DateTime.UtcNow;
                }
                // 若玩家处于特定检测状态（Tr=1），则改变检测状态，并关闭当前检测标志位
                else if (this.LPlayers[args.Msg.whoAmI].Tr == 1)
                {
                    this.LPlayers[args.Msg.whoAmI].Tr = 2;
                    this.LPlayers[args.Msg.whoAmI].BAA = false;
                }
            }
            // 若玩家处于未检测状态（Tr=0），则标记跳过下次伤害检测，并记录当前时间
            else if (args.MsgID == PacketTypes.PlayerDodge)
            {
                if (this.LPlayers[args.Msg.whoAmI].Tr == 0)
                {
                    this.LPlayers[args.Msg.whoAmI].Skip = true;
                    this.LPlayers[args.Msg.whoAmI].LastTiem = DateTime.UtcNow;
                }
                // 若玩家处于特定检测状态（Tr=1），则改变检测状态，并关闭当前检测标志位
                else if (this.LPlayers[args.Msg.whoAmI].Tr == 1)
                {
                    this.LPlayers[args.Msg.whoAmI].Tr = 2;
                    this.LPlayers[args.Msg.whoAmI].BAA = false;
                }
            }
            else if (args.MsgID == PacketTypes.EffectHeal)
            {
                // 若玩家处于特定恢复状态（Tr=3），则标记为正在接受治疗
                if (this.LPlayers[args.Msg.whoAmI].Tr == 3)
                {
                    this.LPlayers[args.Msg.whoAmI].Heal = true;
                }
            }
            else if (args.MsgID == PacketTypes.PlayerHealOther)
            {
                // 若玩家处于特定恢复状态（Tr=3），则标记为正在对他方进行治疗
                if (this.LPlayers[args.Msg.whoAmI].Tr == 3)
                {
                    this.LPlayers[args.Msg.whoAmI].Heal = true;
                }
            }
            else if (args.MsgID == PacketTypes.PlayerSpawn)
            {
                // 若玩家处于未检测状态（Tr=0），则标记跳过下次伤害检测，并记录当前时间
                if (this.LPlayers[args.Msg.whoAmI].Tr == 0)
                {
                    this.LPlayers[args.Msg.whoAmI].Skip = true;
                    this.LPlayers[args.Msg.whoAmI].LastTiem = DateTime.UtcNow;
                }
                // 若玩家处于特定恢复状态（Tr=3），则标记为正在接受治疗
                else if (this.LPlayers[args.Msg.whoAmI].Tr == 3)
                {
                    this.LPlayers[args.Msg.whoAmI].Heal = true;
                }
            }
        }
    }
}