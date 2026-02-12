using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Plugin;

[ApiVersion(2, 1)]
public class TestPlugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "GK 阁下";
    public override string Description => GetString("让死者瞬间原地复活的神奇力量！");
    public override Version Version => new Version(1, 0, 0, 6);
    #endregion

    #region 实例与变量
    private LPlayer?[] LPlayers { get; set; }
    #endregion

    #region 注册与释放
    public TestPlugin(Main game) : base(game)
    {
        this.Order = 99;
        this.LPlayers = new LPlayer[256];
    }

    public override void Initialize()
    {
        LoaderComfig();
        GeneralHooks.ReloadEvent += this.ReloadConfig;
        ServerApi.Hooks.NetGetData.Register(this, this.GetData);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.ReloadConfig;
            ServerApi.Hooks.NetGetData.Deregister(this, this.GetData);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    public static Configuration Config { get; set; } = new Configuration();
    public static void LoaderComfig()
    {
        try
        {
            if (File.Exists(Configuration.FilePath))
            {
                Config = Configuration.Read(Configuration.FilePath);
            }
            else
            {
                TShock.Log.ConsoleError(GetString("未找到复活币配置，已为您创建！修改配置后输入指令 /reload 重新读取"));
            }
            Config.Write();
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.ToString());
        }

        //服务器没开启强制开荒或插件已开启，则关闭插件
        if (!Main.ServerSideCharacter && Config.Enabled)
        {
            TShock.Log.ConsoleError(GetString("启动复活币需要先启动SSC功能，检测到服务器未开启SSC功能已自动关闭复活币功能"));
            Config.Enabled = false;
            Config.Write();
        }
    }

    private void ReloadConfig(ReloadEventArgs args)
    {
        LoaderComfig();
        args.Player.SendInfoMessage(GetString("[复活币]重新加载配置完毕。"));
    }
    #endregion

    #region 验证每个玩家数据是否能和服务器接轨用的
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
    #endregion

    #region 获取玩家复活数据
    private void GetData(GetDataEventArgs args)
    {
        var plr = TShock.Players[args.Msg.whoAmI];

        //玩家为空则返回
        if (plr == null)
        {
            return;
        }

        lock (this.LPlayers)
        {
            //玩家索引为空则返回
            if (this.LPlayers[plr.Index] == null)
            {
                return;
            }
        }

        //如果玩家连接状态无效，或事件已处理则返回
        if (!plr.ConnectionAlive || !plr.Active || args.Handled)
        {
            return;
        }

        //玩家没死亡或没权限 返回
        if ((int) args.MsgID != 118 || !plr.HasPermission("RebirthCoin"))
        {
            return;
        }

        //如果玩家生成（指重生）
        if ((int) args.MsgID == 12)
        {
            using (var binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
            {
                binaryReader.ReadByte();
                var num = binaryReader.ReadInt16();
                var num2 = binaryReader.ReadInt16();
                var num3 = binaryReader.ReadInt32();
                var val2 = (PlayerSpawnContext) binaryReader.ReadByte();
                if (val2 == 0 && num3 <= 0 && this.LPlayers[plr.Index]!.tp)
                {
                    plr.Teleport(this.LPlayers[plr.Index]!.x, this.LPlayers[plr.Index]!.y, 1);
                    this.LPlayers[plr.Index]!.tp = false;
                    args.Handled = true;
                }
                return;
            }
        }

        //如果玩家没开启SSC,或插件配置开关没启动 则返回
        if (!plr.SaveServerCharacter() && Config.Enabled)
        {
            Console.WriteLine(GetString("启动复活币需要先启动SSC功能! "));
            return;
        }

        //背包格子的索引值，定义为空
        var slot = -1;

        //如果插件开启
        if (Config.Enabled)
        {
            //背包58格只取前50格遍历，51-54是钱币栏,55-58是弹药栏（不可能存物品）
            for (var i = 0; i < 50; i++)
            {
                //如果背包格子出现这个数组里的ID 则选择这个格子的索引值
                if (Config.ItemID.Contains(plr.TPlayer.inventory[i].type))
                {
                    slot = i;
                    break;
                }
            }

            //如果格子索引为空，返回
            if (slot == -1)
            {
                return;
            }
        }

        try
        {
            using var binaryReader2 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
            binaryReader2.ReadByte();
            var val3 = PlayerDeathReason.FromReader(new BinaryReader(binaryReader2.BaseStream));
            var num5 = binaryReader2.ReadInt16();
            var b = (byte) (binaryReader2.ReadByte() - 1);
            BitsByte val4 = binaryReader2.ReadByte();
            var flag = val4[0];
            binaryReader2.Close();

            if (!flag || Config.PVP)
            {
                if (Config.Enabled && slot != -1)
                {
                    var inv = plr.TPlayer.inventory[slot];
                    inv.stack--;
                    plr.SendData((PacketTypes) 5, "", plr.Index, slot, plr.TPlayer.inventory[0].prefix, 0f, 0);
                }

                this.LPlayers[plr.Index]!.x = plr.X;
                this.LPlayers[plr.Index]!.y = plr.Y;
                this.LPlayers[plr.Index]!.tp = true;

                //玩家生成
                plr.Spawn(0, null);

                //发送消息
                plr.SendInfoMessage(Config.Mess.Replace("{0}", plr.Name), Convert.ToByte(Config.Colors[0]), Convert.ToByte(Config.Colors[1]), Convert.ToByte(Config.Colors[2]));
                args.Handled = true;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.ToString());
        }
    }
    #endregion
}