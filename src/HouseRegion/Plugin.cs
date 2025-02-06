using LazyAPI;
using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using On.OTAPI;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;


namespace HouseRegion;

[ApiVersion(2, 1)]//api版本
public class HousingPlugin : LazyPlugin
{
    public override string Author => "GK 阁下 改良";
    public override string Description => GetString("一个著名的用于保护房屋的插件。");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 9);
    public HousingPlugin(Main game) : base(game)
    {
    }
    public static LPlayer?[] LPlayers { get; set; } = new LPlayer[256];//L表示local本地的意思
    public static List<House> Houses = new();
    static readonly System.Timers.Timer Update = new(1100);//创建一个1.1秒的时钟
    public static bool ULock = false;


    private void RD()//读取
    {
        var table = new SqlTable("HousingDistrict",
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },//主键id自增
            new SqlColumn("Name", MySqlDbType.VarChar, 255) { Unique = true },//唯一的
            new SqlColumn("TopX", MySqlDbType.Int32),
            new SqlColumn("TopY", MySqlDbType.Int32),
            new SqlColumn("BottomX", MySqlDbType.Int32),
            new SqlColumn("BottomY", MySqlDbType.Int32),//以上四个为两点的XY坐标
            new SqlColumn("Author", MySqlDbType.VarChar, 32),//作者
            new SqlColumn("Owners", MySqlDbType.Text),//拥有者
            new SqlColumn("WorldID", MySqlDbType.Text),//世界代号
            new SqlColumn("Locked", MySqlDbType.Int32),//是否上锁
            new SqlColumn("Users", MySqlDbType.Text));//使用者
        var SQLWriter = new SqlTableCreator(TShock.DB,
            TShock.DB.GetSqlType() == SqlType.Sqlite ?
            new SqliteQueryCreator() : new MysqlQueryCreator());
        SQLWriter.EnsureTableStructure(table);
    }
    private void RH()//读取房屋
    {
        var SQLWriter = TShock.DB.QueryReader("SELECT* FROM HousingDistrict");//开始读取数据库
        Houses.Clear();//清除全部
        while (SQLWriter.Read())//判断循环
        {
            //Console.WriteLine("读入1个 " + Main.worldID.ToString());
            if (SQLWriter.Get<string>("WorldID") != Main.worldID.ToString())
            {
                continue;//世界id不一致到循环尾巴
            }

            var author = SQLWriter.Get<string>("Author");//读到房屋的作者
            var owners = SQLWriter.Get<string>("Owners").Split(',').ToList();//找到拥有者列表
            var locked = SQLWriter.Get<int>("Locked") == 1;//读取上锁
            var user = SQLWriter.Get<string>("Users").Split(',').ToList();//使用者列表
            Houses.Add(new House(new Rectangle(SQLWriter.Get<int>("TopX"),
                SQLWriter.Get<int>("TopY"),
                SQLWriter.Get<int>("BottomX"),
                SQLWriter.Get<int>("BottomY")),
                author,
                owners, SQLWriter.Get<string>("Name"), locked, user));//添加到组
        }
    }

    #region 插件的各种初始化
    public override void Initialize()// 插件启动时，用于初始化各种狗子
    {
        this.RD();
        GetDataHandlers.InitGetDataHandler();//初始化配置值，RH要放在服务器开成后再读不然世界ID读不出
        Commands.ChatCommands.Add(new Command("house.use", this.HCommands, "house") { HelpText = GetString("输入/house help可以显示与房子相关的操作提示。") });
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);//玩家进入服务器
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);//玩家退出服务器
        ServerApi.Hooks.GamePostInitialize.Register(this, this.PostInitialize);//地图读入后
        Hooks.MessageBuffer.InvokeGetData += this.MessageBufferOnInvokeGetData;
    }

    private bool MessageBufferOnInvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {
        var user = TShock.Players[instance.whoAmI];
        using (var data = new MemoryStream(instance.readBuffer, readOffset, length))
        {
            try
            {
                if (GetDataHandlers.HandlerGetData((PacketTypes) packetId, user, data))
                {
                    return false;
                }
            }
            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误传递时出错:") + ex.ToString()); }
        }
        return orig.Invoke(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType,
        maxPackets);
    }

    protected override void Dispose(bool disposing)// 插件关闭时
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.HCommands);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);//玩家进入服务器
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);//玩家退出服务器
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.PostInitialize);//地图读入后
            Update.Elapsed -= this.OnUpdate; Update.Stop();//销毁时钟
            Hooks.MessageBuffer.InvokeGetData -= this.MessageBufferOnInvokeGetData;
        }
        base.Dispose(disposing);
    }
    private void OnGreetPlayer(GreetPlayerEventArgs e)//进入游戏时
    {
        lock (LPlayers)
        {
            LPlayers[e.Who] = new LPlayer(e.Who, TShock.Players[e.Who].TileX, TShock.Players[e.Who].TileY);
        }
    }
    private void OnLeave(LeaveEventArgs e)
    {
        lock (LPlayers)
        {
            if (LPlayers[e.Who] != null)
            {
                LPlayers[e.Who] = null;
            }
        }
    }
    public void PostInitialize(EventArgs e)
    {
        this.RH(); Update.Elapsed += this.OnUpdate; Update.Start();//只有读入地图后才能取得地图ID否则地图ID为空//启动时钟
    }
    #endregion

    #region 插件指令处理
    private void HCommands(CommandArgs args)
    {
        if (!args.Player.IsLoggedIn || args.Player.Account == null || args.Player.Account.ID == 0)
        { args.Player.SendErrorMessage(GetString("你必须登录才能使用房子插件。")); return; }
        var cmd = "help";
        const string AdminHouse = "house.admin";//定义管理权限和定义默认指令即如果指令后面什么都没跟的时候的指令
        if (args.Parameters.Count > 0)
        {
            cmd = args.Parameters[0].ToLower();
        }

        switch (cmd)
        {
            case "name"://确认房子名字
            {
                args.Player.SendMessage(GetString("请敲击一个块查看它属于哪个房子。"), Color.Yellow);
                LPlayers[args.Player.Index]!.Look = true; break;
            }
            case "set"://设置点
            {
                if (args.Parameters.Count == 2 &&
                    int.TryParse(args.Parameters[1], out var choice) &&
                    choice >= 1 && choice <= 2)
                {
                    if (choice == 1)
                    {
                        args.Player.SendMessage(GetString("现在请敲击要保护的区域的左上角。"), Color.Yellow);
                    }

                    if (choice == 2)
                    {
                        args.Player.SendMessage(GetString("现在请敲击要保护的区域的右下角。"), Color.Yellow);
                    }

                    args.Player.AwaitingTempPoint = choice;
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("指令错误! 正确指令: /house set [1/2]"));
                }

                break;
            }
            case "add"://添加房子
            {
                if (args.Parameters.Count > 1)
                {
                    var maxHouses = Utils.MaxCount(args.Player); var authorHouses = 0;
                    for (var i = 0; i < Houses.Count; i++)
                    {
                        if (Houses[i].Author == args.Player.Account.ID.ToString())
                        {
                            authorHouses++;
                        }
                    }
                    if (authorHouses < maxHouses || args.Player.Group.HasPermission("house.bypasscount"))
                    {
                        if (!args.Player.TempPoints.Any(p => p == Point.Zero))
                        {
                            var houseName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            if (string.IsNullOrEmpty(houseName)) { args.Player.SendErrorMessage(GetString("房屋名称不能为空。")); return; }
                            var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
                            var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
                            var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X) + 1;
                            var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y) + 1;
                            var maxSize = Utils.MaxSize(args.Player);
                            if ((width * height <= maxSize && width >= Config.Instance.MinWidth && height >= Config.Instance.MinHeight) || args.Player.Group.HasPermission("house.bypasssize"))
                            {
                                var newHouseR = new Rectangle(x, y, width, height);
                                for (var i = 0; i < Houses.Count; i++)
                                {
                                    if (newHouseR.Intersects(Houses[i].HouseArea))
                                    {
                                        args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                        args.Player.SendErrorMessage(GetString("你选择的区域与其他房子存在重叠，这是不允许的。"));
                                        return;
                                    }
                                }
                                if (newHouseR.Intersects(new Rectangle(Main.spawnTileX, Main.spawnTileY, TShock.Config.Settings.SpawnProtectionRadius, TShock.Config.Settings.SpawnProtectionRadius)))
                                {
                                    args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                    args.Player.SendErrorMessage(GetString("你选择的区域与出生保护范围重叠，这是不允许的。"));
                                    return;
                                }
                                for (var i = 0; i < TShock.Regions.Regions.Count; i++)
                                {
                                    if (newHouseR.Intersects(TShock.Regions.Regions[i].Area))
                                    {
                                        args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                        args.Player.SendErrorMessage(GetString("你选择的区域与Tshock区域 {0} 重叠，这是不允许的。"), TShock.Regions.Regions[i].Name);
                                        return;
                                    }
                                }
                                if (HouseManager.AddHouse(x, y, width, height, houseName, args.Player.Account.ID.ToString()))
                                {
                                    args.Player.SendMessage(GetString("你建造了新房子 ") + houseName, Color.Yellow);
                                    TShock.Log.ConsoleInfo(GetString("{0} 建了新房子: {1}"), args.Player.Account.Name, houseName);
                                }
                                else { args.Player.SendErrorMessage(GetString($"房子 {houseName} 已存在!")); return; }//此情况不清除点
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString("您设置的房屋宽:{0} 高:{1} 面积:{2} 需重新设置。"), width, height, width * height);
                                if (width * height >= maxSize)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房子总面积超过了最大限制 {maxSize} 格块。"));
                                }

                                if (width < Config.Instance.MinWidth)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房屋宽度小于最小限制 {Config.Instance.MinWidth} 格块。"));
                                }

                                if (height < Config.Instance.MinHeight)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房子总面积超过了最大限制 {Config.Instance.MinHeight} 格块。"));
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString("未设置完整的房屋点,建议先使用指令: /house help"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("房屋添加失败:您只能添加{0}个房屋!"), maxHouses);
                    }

                    args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除已设置的点
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house add [屋名]"));
                }

                break;//第二分号是顺序执行
            }
            case "allow"://添加所有者
            {
                if (args.Parameters.Count > 2)
                {
                    var playerName = args.Parameters[1];
                    UserAccount playerID;
                    var housename = string.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2));
                    var house = Utils.GetHouseByName(housename);
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse))
                    {
                        if ((playerID = TShock.UserAccounts.GetUserAccountByName(playerName)) != null)
                        {
                            if (Config.Instance.ProhibitSharingOwner) { args.Player.SendErrorMessage(GetString("无法使用，因为服务器禁止了分享所有者功能。")); return; }
                            if (playerID.ID.ToString() != house.Author && !Utils.OwnsHouse(playerID.ID.ToString(), house))
                            {
                                if (HouseManager.AddNewOwner(house.Name, playerID.ID.ToString()))
                                {
                                    args.Player.SendMessage(GetString($"成功为 {playerName} 添加房屋 {house.Name} 的拥有权!"), Color.Yellow);
                                    TShock.Log.ConsoleInfo(GetString("{0} 添加 {1} 为房屋 {2} 的拥有者。"), args.Player.Account.Name, playerID.Name, house.Name);
                                }
                                else
                                {
                                    args.Player.SendErrorMessage(GetString("添加用户权力失败。"));
                                }
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString($"用户 {playerName} 已拥有此房屋权限。"));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString($"用户 {playerName} 不存在。"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力分享这个房子!"));//只有房子的作者可以
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house allow [名字] [屋名]"));
                }

                break;
            }
            case "disallow":
            {
                if (args.Parameters.Count > 2)
                {
                    var playerName = args.Parameters[1]; UserAccount playerID;
                    var house = Utils.GetHouseByName(string.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2)));
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse))
                    {
                        if ((playerID = TShock.UserAccounts.GetUserAccountByName(playerName)) != null)
                        {
                            if (!Utils.OwnsHouse(playerID.ID.ToString(), house)) { args.Player.SendErrorMessage(GetString("目标非此房屋拥有者。")); return; }
                            if (HouseManager.DeleteOwner(house.Name, playerID.ID.ToString()))
                            {
                                args.Player.SendMessage(GetString($"成功移除 {playerName} 的房屋 {house.Name} 的拥有权!"), Color.Yellow);
                                TShock.Log.ConsoleInfo(GetString("{0} 移除 {1} 的房屋 {2} 的拥有者。"), args.Player.Account.Name, playerID.Name, house.Name);
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString("移除用户权力失败。"));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString($"用户 {playerName} 不存在。"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力管理这个房子!"));//只有房子的作者可以
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house disallow [名字] [屋名]"));
                }

                break;
            }
            case "adduser"://添加使用者
            {
                if (args.Parameters.Count > 2)
                {
                    var playerName = args.Parameters[1]; UserAccount playerID;
                    var housename = string.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2));
                    var house = Utils.GetHouseByName(housename);
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse) || Utils.OwnsHouse(args.Player.Account, house))
                    {
                        if (house.Author != args.Player.Account.ID.ToString() && !args.Player.Group.HasPermission(AdminHouse) && Config.Instance.ProhibitOwnerModifyingUser)
                        { args.Player.SendErrorMessage(GetString("无法使用，因为服务器禁止了所有者分享使用者功能。")); return; }
                        if ((playerID = TShock.UserAccounts.GetUserAccountByName(playerName)) != null)
                        {
                            if (Config.Instance.ProhibitSharingUser) { args.Player.SendErrorMessage(GetString("无法使用，因为服务器禁止了分享使用者功能。")); return; }
                            if (playerID.ID.ToString() != house.Author && !Utils.OwnsHouse(playerID.ID.ToString(), house) && !Utils.CanUseHouse(playerID.ID.ToString(), house))
                            {
                                if (HouseManager.AddNewUser(house.Name, playerID.ID.ToString()))
                                {
                                    args.Player.SendMessage(GetString($"成功移除 {playerName} 的房屋 {house.Name} 的拥有权!"), Color.Yellow);
                                    TShock.Log.ConsoleInfo(GetString("{0} 添加 {1} 为房屋 {2} 的使用者。"), args.Player.Account.Name, playerID.Name, house.Name);
                                }
                                else
                                {
                                    args.Player.SendErrorMessage(GetString("添加用户权力失败。"));
                                }
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString($"用户 {playerName} 已拥有此房屋权限。"));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString($"用户 {playerName} 不存在。"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力分享这个房子!"));//只有房子的作者可以
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house adduser [名字] [屋名]"));
                }

                break;
            }
            case "deluser"://删除使用者
            {
                if (args.Parameters.Count > 2)
                {
                    var playerName = args.Parameters[1]; UserAccount playerID;
                    var house = Utils.GetHouseByName(string.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2)));
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse) || Utils.OwnsHouse(args.Player.Account, house))
                    {
                        if (house.Author != args.Player.Account.ID.ToString() && !args.Player.Group.HasPermission(AdminHouse) && Config.Instance.ProhibitOwnerModifyingUser)
                        { args.Player.SendErrorMessage(GetString("无法使用，因为服务器禁止了所有者修改使用者功能。")); return; }
                        if ((playerID = TShock.UserAccounts.GetUserAccountByName(playerName)) != null)
                        {
                            if (!Utils.CanUseHouse(playerID.ID.ToString(), house)) { args.Player.SendErrorMessage(GetString("目标非此房屋使用者。")); return; }
                            if (HouseManager.DeleteUser(house.Name, playerID.ID.ToString()))
                            {
                                args.Player.SendMessage(GetString($"成功移除 {playerName} 的房屋 {house.Name} 的使用权!"), Color.Yellow);
                                TShock.Log.ConsoleInfo(GetString("{0} 移除 {1} 的房屋 {2} 的使用者。"), args.Player.Account.Name, playerID.Name, house.Name);
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString("移除用户权力失败。"));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString($"用户 {playerName} 不存在。"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力管理这个房子!"));//只有房子的作者可以
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house deluser [名字] [屋名]"));
                }

                break;
            }
            case "delete":
            {
                if (args.Parameters.Count > 1)
                {
                    var houseName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    var house = Utils.GetHouseByName(houseName);
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse))
                    {
                        try
                        {
                            TShock.DB.Query("DELETE FROM HousingDistrict WHERE Name=@0", house.Name);
                        }
                        catch (Exception ex)
                        {
                            TShock.Log.Error(GetString("房屋插件错误删除错误") + ex.ToString());
                            args.Player.SendErrorMessage(GetString("房屋删除失败!"));
                            return;
                        }
                        Houses.Remove(house);
                        args.Player.SendMessage(GetString($"房屋:{house.Name} 删除成功!"), Color.Yellow);
                        TShock.Log.ConsoleInfo(GetString("{0} 删除房屋: {1}"), args.Player.Account.Name, house.Name);
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力删除这个房子!"));//只有房子的作者可以
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house delete [屋名]"));
                }

                break;
            }
            case "clear":
            {
                args.Player.TempPoints[0] = Point.Zero;
                args.Player.TempPoints[1] = Point.Zero;
                args.Player.AwaitingTempPoint = 0;
                args.Player.SendMessage(GetString("临时敲击点清除完毕!"), Color.Yellow);
                break;
            }
            case "list":
            {
                const int pagelimit = 15; const int perline = 5; var page = 0;//每页上限每行上限和当前页码
                if (args.Parameters.Count > 1)
                {
                    if (!int.TryParse(args.Parameters[1], out page) || page < 1)
                    { args.Player.SendErrorMessage(string.Format(GetString("无效页码 ({0})"), page)); return; }
                    page--; //从0开始而不是1
                }
                var Lhouses = new List<House>();
                for (var i = 0; i < Houses.Count; i++)
                {
                    if (args.Player.Group.HasPermission(AdminHouse) || args.Player.Account.ID.ToString() == Houses[i].Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), Houses[i]) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), Houses[i]))
                    {
                        Lhouses.Add(Houses[i]);
                    }
                }
                if (Lhouses.Count == 0) { args.Player.SendMessage(GetString("您目前还没有已定义房屋。"), Color.Yellow); return; }
                var pagecount = Lhouses.Count / pagelimit;
                if (page > pagecount) { args.Player.SendErrorMessage(GetString("页码超过最大页数 ({0}/{1})"), page + 1, pagecount + 1); return; }
                args.Player.SendMessage(string.Format(GetString("目前的房屋 ({0}/{1}) 页:"), page + 1, pagecount + 1), Color.Green);
                var nameslist = new List<string>();
                for (var i = page * pagelimit; i < (page * pagelimit) + pagelimit && i < Lhouses.Count; i++)
                {
                    nameslist.Add(Lhouses[i].Name);
                }

                var names = nameslist.ToArray();
                for (var i = 0; i < names.Length; i += perline)
                {
                    args.Player.SendMessage(string.Join(", ", names, i, Math.Min(names.Length - i, perline)), Color.Yellow);
                }

                if (page < pagecount)
                {
                    args.Player.SendMessage(string.Format(GetString("输入 /house list {0} 查看更多房屋。"), page + 2), Color.Yellow);
                }

                break;
            }
            case "redefine"://重新定义
            {
                if (args.Parameters.Count > 1)
                {
                    if (!args.Player.TempPoints.Any(p => p == Point.Zero))
                    {
                        var houseName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                        var house = Utils.GetHouseByName(houseName);
                        if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }//此时不清楚点
                        if (house.Author == args.Player.Account.ID.ToString() || args.Player.Group.HasPermission(AdminHouse))
                        {
                            var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
                            var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
                            var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X) + 1;
                            var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y) + 1;
                            var maxSize = Utils.MaxSize(args.Player);
                            if ((width * height <= maxSize && width >= Config.Instance.MinWidth && height >= Config.Instance.MinHeight) || args.Player.Group.HasPermission("house.bypasssize"))
                            {
                                var newHouseR = new Rectangle(x, y, width, height);
                                for (var i = 0; i < Houses.Count; i++)
                                {
                                    if (newHouseR.Intersects(Houses[i].HouseArea) && Houses[i].Name != house.Name)//如果发现重叠并且不是本房屋
                                    {
                                        args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                        args.Player.SendErrorMessage(GetString("你选择的区域与其他房子存在重叠，这是不允许的。"));
                                        return;
                                    }
                                }
                                if (newHouseR.Intersects(new Rectangle(Main.spawnTileX, Main.spawnTileY, TShock.Config.Settings.SpawnProtectionRadius, TShock.Config.Settings.SpawnProtectionRadius)))
                                {
                                    args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                    args.Player.SendErrorMessage(GetString("你选择的区域与出生保护范围重叠，这是不允许的。"));
                                    return;
                                }
                                for (var i = 0; i < TShock.Regions.Regions.Count; i++)
                                {
                                    if (newHouseR.Intersects(TShock.Regions.Regions[i].Area))
                                    {
                                        args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除点
                                        args.Player.SendErrorMessage(GetString("你选择的区域与Tshock区域 {0} 重叠，这是不允许的。"), TShock.Regions.Regions[i].Name);
                                        return;
                                    }
                                }
                                if (HouseManager.RedefineHouse(x, y, width, height, houseName))
                                {
                                    args.Player.SendMessage(GetString("重新定义了房子 ") + houseName, Color.Yellow);
                                    TShock.Log.ConsoleInfo(GetString("{0} 重新定义的房子: {1}"), args.Player.Account.Name, houseName);
                                }
                                else { args.Player.SendErrorMessage(GetString("重新定义房屋时出错!")); return; }//此情况不清除点
                            }
                            else
                            {
                                args.Player.SendErrorMessage(GetString("您设置的房屋宽:{0} 高:{1} 面积:{2} 需重新设置。"), width, height, width * height);
                                if (width * height >= maxSize)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房子总面积超过了最大限制 {maxSize} 格块。"));
                                }

                                if (width < Config.Instance.MinWidth)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房屋宽度小于最小限制 {Config.Instance.MinWidth} 格块。"));
                                }

                                if (height < Config.Instance.MinHeight)
                                {
                                    args.Player.SendErrorMessage(GetString($"因为您的房屋高度小于最小限制 {Config.Instance.MinHeight} 格块。"));
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString("你没有权力修改这个房子!"));//只有房子的作者可以
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("未设置完整的房屋点,建议先使用指令: /house help"));
                    }

                    args.Player.TempPoints[0] = Point.Zero; args.Player.TempPoints[1] = Point.Zero;//清除已设置的点
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house redefine [屋名]"));
                }

                break;
            }
            case "info":
            {
                if (args.Parameters.Count > 1)
                {
                    var house = Utils.GetHouseByName(args.Parameters[1]);
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (args.Player.Group.HasPermission(AdminHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
                    {
                        var OwnerNames = ""; var UserNames = ""; var AuthorNames = "";
                        try { AuthorNames = TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(house.Author)).Name; }
                        catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误超标错误:") + ex.ToString()); }
                        for (var i = 0; i < house.Owners.Count; i++)
                        {
                            var ID = house.Owners[i];
                            try { OwnerNames += (string.IsNullOrEmpty(OwnerNames) ? "" : ", ") + TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(ID)).Name; }
                            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误超标错误:") + ex.ToString()); }
                        }
                        for (var i = 0; i < house.Users.Count; i++)
                        {
                            var ID = house.Users[i];
                            try { UserNames += (string.IsNullOrEmpty(UserNames) ? "" : ", ") + TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(ID)).Name; }
                            catch (Exception ex) { TShock.Log.Error(GetString("房屋插件错误超标错误:") + ex.ToString()); }
                        }
                        args.Player.SendMessage(GetString($"房屋 {house.Name} 的信息:"), Color.LawnGreen);
                        args.Player.SendMessage(GetString("作者: ") + AuthorNames, Color.LawnGreen);
                        args.Player.SendMessage(GetString("状态: ") + (!house.Locked || Config.Instance.LimitLockHouse ? GetString("未上锁") : GetString("已上锁")), Color.LawnGreen);
                        args.Player.SendMessage(GetString("拥有者: ") + OwnerNames, Color.LawnGreen);
                        args.Player.SendMessage(GetString("使用者: ") + UserNames, Color.LawnGreen);
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力查看这个房子的信息!"));
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house info [屋名]"));
                }

                break;
            }
            case "lock":
            {
                if (args.Parameters.Count > 1)
                {
                    var houseName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    var house = Utils.GetHouseByName(houseName);
                    if (house == null) { args.Player.SendErrorMessage(GetString("没有找到这个房子!")); return; }
                    if (Config.Instance.LimitLockHouse) { args.Player.SendErrorMessage(GetString("无法使用，因为服务器关闭了锁房屋功能。")); return; }
                    if (args.Player.Group.HasPermission(AdminHouse) || args.Player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(args.Player.Account.ID.ToString(), house) || Utils.CanUseHouse(args.Player.Account.ID.ToString(), house))
                    {
                        if (HouseManager.ChangeLock(house))
                        {
                            args.Player.SendMessage(GetString($"房子: {house.Name} {(house.Locked ? GetString("上锁") : GetString("开锁"))}"), Color.Yellow);
                            TShock.Log.ConsoleInfo(GetString("{0} 修改锁状态: {1}"), args.Player.Account.Name, house.Name);
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString("修改房屋锁失败!"));
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("你没有权力修改这个房子的锁!"));
                    }
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("语法错误! 正确语法: /house lock [屋名]"));
                }

                break;
            }
            default:
            {
                args.Player.SendMessage(GetString("要创建房屋，请使用以下命令:"), Color.Lime);
                args.Player.SendMessage(GetString("/house set 1"), Color.Lime);
                args.Player.SendMessage(GetString("/house set 2"), Color.Lime);
                args.Player.SendMessage(GetString("/house add 房屋名字"), Color.Lime);
                args.Player.SendMessage(GetString("其他命令: list, allow, disallow, redefine, name, delete, clear, info, adduser, deluser, lock"), Color.Lime);
                break;
            }
        }//循环的括号
    }//方法的括号
    #endregion


    #region 时钟事件
    public void OnUpdate(object? sender, ElapsedEventArgs e)//时钟更新数据时
    {
        if (!Config.Instance.HouseRegion || ULock)
        {
            return;//若关闭了提示或时钟正在执行过程中，则这次放弃执行
        }

        ULock = true; var Start = DateTime.Now;//Console.WriteLine("时钟正常启动" );
        lock (LPlayers)
        {
            foreach (var user in LPlayers)
            {
                if (user == null)
                {
                    continue;//空白跳过
                }

                if (Timeout(Start))
                {
                    return;//检测耗时超时
                }

                var player = TShock.Players[user.Who];
                if (player == null) { ULock = false; return; }//玩家组变动，此时不计入
                var house = Utils.InAreaHouse(player.TileX, player.TileY);//当前所在的房子
                var Lhouse = Utils.InAreaHouse(user.TileX, user.TileY);//之前所在的房子
                user.TileX = player.TileX; user.TileY = player.TileY;//使此为前
                if (Lhouse == null && house == null)
                {
                    continue;
                }

                if (Lhouse == null && house != null)//此乃进入了房子
                {
                    if (player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(player.Account.ID.ToString(), house) || Utils.CanUseHouse(player.Account.ID.ToString(), house))
                    {
                        player.SendMessage(GetString("你进入了你的房子: ") + house.Name, Color.LightSeaGreen);
                    }
                    else
                    {
                        player.SendMessage(GetString("你进入了房子: ") + house.Name, Color.LightSeaGreen);
                    }
                }
                else if (Lhouse != null && house == null)//此乃离开了房子
                {
                    if (player.Account.ID.ToString() == Lhouse.Author || Utils.OwnsHouse(player.Account.ID.ToString(), Lhouse) || Utils.CanUseHouse(player.Account.ID.ToString(), Lhouse))
                    {
                        player.SendMessage(GetString("你离开了你的房子: ") + Lhouse.Name, Color.LightSeaGreen);
                    }
                    else
                    {
                        player.SendMessage(GetString("你离开了房子: ") + Lhouse.Name, Color.LightSeaGreen);
                    }
                }
                else//此乃离开了一个房子又进入另外一个房子
                {
                    if (Lhouse!.Name == house!.Name)
                    {
                        continue;//仍然在原来的房子里
                    }

                    if (player.Account.ID.ToString() == house.Author || Utils.OwnsHouse(player.Account.ID.ToString(), house) || Utils.CanUseHouse(player.Account.ID.ToString(), house))
                    {
                        player.SendMessage(GetString("你进入了你的房子: ") + house.Name, Color.LightSeaGreen);
                    }
                    else
                    {
                        player.SendMessage(GetString("你进入了房子: ") + house.Name, Color.LightSeaGreen);
                    }
                }
            }
        }

        ULock = false;
    }
    public static bool Timeout(DateTime Start, bool warn = true, int ms = 500)//参数1启始时间，参数2通告，3，超时时间
    {
        var ret = (DateTime.Now - Start).TotalMilliseconds >= ms; if (ret)
        {
            ULock = false;//超时自动解锁
        }

        if (warn && ret) { TShock.Log.Error(GetString("房子插件提示处理超时,已抛弃部分提示!")); }
        return ret;
    }
    #endregion
}