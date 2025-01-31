using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace ProgressControl;

public partial class PControl : TerrariaPlugin
{
    /// <summary>
    /// 重置函数
    /// </summary>
    private static async void ResetGame()
    {
        //在服务器关闭前执行指令
        config.CommandForBeforeResetting.ForEach(x => Commands.HandleCommand(TSPlayer.Server, "/" + x.Trim('/', '.')));
        //不重置玩家数据
        if (!config.ResetTSCharacter)
        {
            TShock.Players.ForEach(x =>
            {
                if (x != null)
                {
                    if (x.IsLoggedIn)
                    {
                        x.SaveServerCharacter();
                    }

                    x.Disconnect(GetString("服务器正在重置"));
                }
            });
        }
        else//重置玩家数据
        {
            TShock.Players.ForEach(x =>
            {
                if (x != null && x.IsLoggedIn)
                {
                    x.Disconnect(GetString("服务器正在重置"));
                }
            });
            try
            {
                TShock.DB.Query("delete from tsCharacter");
            }
            catch { }
        }
        config.DeleteSQLiteForBeforeResetting?.ForEach(x =>
        {
            try
            {
                TShock.DB.Query("DROP TABLE " + x);
            }
            catch { }
        });
        if (!config.DeleteWorldForReset)
        {
            TShock.Utils.SaveWorld();
        }

        if (config.DeleteWorldForReset && File.Exists(Main.worldPathName))
        {
            File.Delete(Main.worldPathName);
            if (File.Exists(Main.worldPathName + ".bak"))
            {
                File.Delete(Main.worldPathName + ".bak");
            }

            if (File.Exists(Main.worldPathName + ".bak2"))
            {
                File.Delete(Main.worldPathName + ".bak2");
            }

            if (File.Exists(Main.worldPathName + ".crash.bak"))
            {
                File.Delete(Main.worldPathName + ".crash.bak");
            }

            if (File.Exists(Main.worldPathName + ".crash.bak2"))
            {
                File.Delete(Main.worldPathName + ".crash.bak2");
            }
        }

        try
        {
            if (config.DeleteFileForBeforeResetting.Count > 0)
            {
                foreach (var v in config.DeleteFileForBeforeResetting)
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        continue;
                    }

                    if (File.Exists(v))//删除指定文件
                    {
                        File.Delete(v);
                    }

                    if (Directory.Exists(v))//删除指定文件夹
                    {
                        Directory.Delete(v, true);
                    }
                    //分析删除后缀为 *.txt(举例) 的文件
                    var files = v.Split('/');
                    var theDirectory = v.Remove(v.Length - files[^1].Length);
                    files[^1] = files[^1].Trim();
                    if (files[^1].StartsWith("*.") && Directory.Exists(theDirectory))
                    {
                        var dir = new DirectoryInfo(theDirectory);
                        foreach (var i in dir.GetFileSystemInfos())
                        {
                            if (i.Extension.Equals(files[^1].TrimStart('*'), StringComparison.OrdinalIgnoreCase) && File.Exists(i.FullName))
                            {
                                File.Delete(i.FullName);      //删除指定后缀名文件
                                TShock.Log.Info(GetString($"{i.Name} 删除成功"));
                                Console.WriteLine(GetString($"{i.Name} 删除成功"));
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TSPlayer.All.SendErrorMessage(GetString($"重置前删除哪些文件错误：{ex}"));
            TShock.Log.Error(GetString($"重置前删除哪些文件错误：{ex}"));
            Console.WriteLine(GetString($"使用指令删除哪些文件发生错误：{ex}"));
        }
        TShock.Log.Info(GetString("服务器正在重置——来自插件：计划书ProgressControl"));

        /*
        if (config.自动重置前是否删除日志)
        {
            string name = TShock.Log.FileName;
            TShock.Log.Dispose();
            new DirectoryInfo(TShock.Config.Settings.LogPath).GetFiles().ForEach(x =>
            {
                if (name.Contains(x.Name))
                {
                    x.Delete();
                }
            });
        }
        */

        config.StartServerDate = DateTime.Now;
        config.LasetServerRestartDate = DateTime.Now;
        config.LasetAutoCommandDate = DateTime.Now;

        //如果 你有提供预备名字 且 这个预备名字有对应地图，则直接调用
        if (config.ExpectedUsageWorldFileNameForAotuReset.Count > 0 && ExistWorldNamePlus(config.ExpectedUsageWorldFileNameForAotuReset.First(), out var worldname))
        {
            TShock.Config.Settings.ServerPassword = config.AfterResetServerPassword;
            TShock.Config.Settings.MaxSlots = config.AfterResetPeople;
            Config.SaveTConfig();
            //成功后删掉预备名字
            config.ExpectedUsageWorldFileNameForAotuReset.Remove(config.ExpectedUsageWorldFileNameForAotuReset.First());
            config.SaveConfigFile();
            //这里worldname必须加.wld后缀
            Process.Start(Path.Combine(Environment.CurrentDirectory, "Tshock.Server.exe", "Tshock.Server"),
                $"-lang 7 -world \"{config.path()}/{worldname}.wld\" -maxplayers {TShock.Config.Settings.MaxSlots} -port {config.AfterResetPort} -c");

        }
        //否则生成一个
        else
        {
            if (config.ExpectedUsageWorldFileNameForAotuReset.Count > 0)
            {
                worldname = config.AddNumberFile(CorrectFileName(config.ExpectedUsageWorldFileNameForAotuReset.First()));
                config.ExpectedUsageWorldFileNameForAotuReset.Remove(config.ExpectedUsageWorldFileNameForAotuReset.First());
            }
            else
            {
                worldname = config.AddNumberFile(CorrectFileName(config.WorldNameForAfterReset));
            }
            //将密码和最多在线人数写入配置文件中。
            //???: 启动参数里端口的修改有效，密码的修改无效，人数的修改仅重启第一次有效。密码和人数都会强制参考config.json的内容，为了修改成功，只能先写入config.json了
            TShock.Config.Settings.ServerPassword = config.AfterResetServerPassword;//密码这个东西恒参考config.json，在启动参数里改无效
            TShock.Config.Settings.MaxSlots = config.AfterResetPeople;//端口这个东西恒参考启动参数，我就不改config.json里的了（我知道这一行不是端口，但我就是要把注释写在这）
            Config.SaveTConfig();
            config.SaveConfigFile();//需要保存下，保存对开服时间等的修改
            //-autocreate可以不用加.wld后缀
            Process.Start(Path.Combine(Environment.CurrentDirectory, "Tshock.Server.exe", "Tshock.Server"),
                $"-lang 7 -autocreate {config.MapSizeForAfterReset} -seed {config.WorldSeedForAfterReset} -world {config.path()}/{worldname} -difficulty {config.MapDifficultyForAfterReset} -maxplayers {config.AfterResetPeople} -port {config.AfterResetPort} -c");
        }

        if (config.ServerLogWriterEnabledForAotuResetting)
        {
            try//关闭serverlog
            {
                var property = ServerApi.LogWriter.GetType().GetProperty("DefaultLogWriter", BindingFlags.Instance | BindingFlags.NonPublic);
                var serverLogWriter = (property != null) ? (ServerLogWriter?) property.GetValue(ServerApi.LogWriter) : null;
                serverLogWriter?.Dispose();
            }
            catch { }
        }

        //关闭restapi,只有你开启了才会起效
        try
        {
            TShock.RestApi.Dispose();
        }
        catch { }
        Netplay.SaveOnServerExit = false; //不保存地图
        TShock.ShuttingDown = true;
        Netplay.Disconnect = true;

        await Task.Delay(5000); //考虑到用户使用自动重启的启动项
        Environment.Exit(0);
    }

    /*
    /// <summary>
    /// 在服务器关闭后执行指令
    /// </summary>
    /// <param name="args"></param>
    private void RunResetCmd(string[] args)
    {
        if (Terraria.Utils.ParseArguements(args).TryGetValue("cmd", out string? text) && text != null && text.Equals("run", StringComparison.OrdinalIgnoreCase))
        {
            config.重置后执行的指令.ForEach(x =>
            {
                Commands.HandleCommand(TSPlayer.Server, x);
                for(int i = 0;i < 100; i++)
                {
                    Console.WriteLine(i);
                }
            });
        }
    }
     */


    /// <summary>
    /// 重启游戏
    /// </summary>
    private static void RestartGame()
    {
        config.LasetServerRestartDate = DateTime.Now;
        config.SaveConfigFile();
        config.CommandForBeforeRestart.ForEach(x => Commands.HandleCommand(TSPlayer.Server, "/" + x.Trim('/', '.')));
        TShock.Players.ForEach(x =>
        {
            if (x != null)
            {
                if (x.IsLoggedIn)
                {
                    x.SaveServerCharacter();
                }

                x.Disconnect(GetString("服务器正在重启"));
            }
        });
        TShock.Utils.SaveWorld();
        //将密码和最多在线人数写入配置文件中。
        //bug: 启动参数里端口的修改有效，密码的修改无效，人数的修改仅重启第一次有效。密码和人数都会强制参考config.json的内容，为了修改成功，只能先写入config.json了
        TShock.Config.Settings.ServerPassword = config.AfterRestartServerPassword;//密码这个东西恒参考config.json，在启动参数里改无效
        TShock.Config.Settings.MaxSlots = config.AfterRestartPeople;//端口这个东西恒参考启动参数，我就不改config.json里的了（我知道这一行不是端口）
        Config.SaveTConfig();
        TShock.Log.Info(GetString("服务器正在重启——来自插件：计划书ProgressControl"));
        try//关闭日志占用
        {
            var property = ServerApi.LogWriter.GetType().GetProperty("DefaultLogWriter", BindingFlags.Instance | BindingFlags.NonPublic);
            var serverLogWriter = (property != null) ? (ServerLogWriter?) property.GetValue(ServerApi.LogWriter) : null;
            serverLogWriter?.Dispose();
        }
        catch { }
        try//关闭restapi,只有你开启了才会起效
        {
            TShock.RestApi.Dispose();
        }
        catch { }
        //Main.worldPathName是有.wld后缀的
        Process.Start(Path.Combine(Environment.CurrentDirectory, "Tshock.Server.exe", "Tshock.Server"),
            $"-lang 7 -world \"{Main.worldPathName}\" -maxplayers {config.AfterRestartPeople} -port {config.AfterRestartPort} -c");
        Netplay.Disconnect = true;
        TShock.ShuttingDown = true;
        Environment.Exit(0);
    }


    /// <summary>
    /// 自动执行指令
    /// </summary>
    private static void ActiveCommands()
    {
        config.LasetAutoCommandDate = DateTime.Now;
        if (config.HowLongTimeOfAutoCommand < 0)
        {
            config.HowLongTimeOfAutoCommand = 0;
        }

        config.SaveConfigFile();

        config.AutoCommandList.ForEach(x =>
        {
            if (x.Contains("pco com add", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(GetString($"请不要对自动执行的指令进行套娃！[/{x}]"));
                TShock.Log.Warn(GetString($"请不要对自动执行的指令进行套娃！[/{x}]"));
                TSPlayer.All.SendErrorMessage(GetString($"请不要对自动执行的指令进行套娃！[/{x}]"));
                return;
            }
            try
            {
                Commands.HandleCommand(TSPlayer.Server, "/" + x.Trim('/', '.'));
            }
            catch { }
        });
        try
        {
            if (config.AutoCommandOfBroadcast)
            {
                TShock.Log.Info(GetString("服务器执行指令成功——来自插件：计划书ProgressControl"));
                Console.WriteLine(GetString("服务器自动执行指令成功"));
                TSPlayer.All.SendMessage(GetString("服务器自动执行指令成功"), Color.Yellow);
            }
        }
        catch { }
    }


    /// <summary>
    /// 小功能函数
    /// </summary>
    /// <param name="bossname"></param>
    /// <param name="enableBC">是否发出广播</param>
    private void Function(NPC npc, string bossname, bool enableBC = true)
    {
        var jiange = (DateTime.Now - config.StartServerDate).TotalHours;
        if (jiange < config.ProgressLockTimeForStartServerDate[bossname])
        {
            npc.active = false;
            npc.type = 0;
            TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npc.whoAmI);
            if (enableBC)
            {
                TSPlayer.All.SendInfoMessage(GetString($"{npc.FullName} 未到解锁时间，还剩{HoursToM(config.ProgressLockTimeForStartServerDate[bossname] - jiange, "28FFB8")}"));
            }
        }
    }


    /// <summary>
    /// 小功能函数
    /// </summary>
    /// <param name="bossname"></param>
    private void Function(TSPlayer player, string bossname, double addtime)
    {
        config.ProgressLockTimeForStartServerDate[bossname] += addtime;
        config.SaveConfigFile();
        var st = addtime > 0
            ? GetString("推迟") + (player.IsLoggedIn ? HoursToM(addtime, "28FFB8") : HoursToM(addtime))
            : addtime < 0 ? GetString("提前") + (player.IsLoggedIn ? HoursToM(-1 * addtime, "28FFB8") : HoursToM(-1 * addtime)) : GetString("正常");
        if (!config.OpenAutoControlProgressLock)
        {
            player.SendWarningMessage(GetString("警告，未开启自动控制NPC进度计划，你的修改不会有任何效果"));
            player.SendSuccessMessage(GetString($"定位成功，{bossname}将{st}解锁"));
        }
        else
        {
            SendMessageAllAndMe(player, GetString($"定位成功，{bossname}将{st}解锁"), Color.LimeGreen);
        }
    }


    /// <summary>
    /// 随机返回一个颜色，网上抄的
    /// </summary>
    /// <returns></returns>
    private static Color TextColor()
    {
        int Hue, Saturation, Lightness;
        Hue = Main.rand.Next(0, 360);
        Saturation = Main.rand.Next(80, 246);
        Lightness = Main.rand.Next(180, 256);

        var num4 = 0.0;
        var num5 = 0.0;
        var num6 = 0.0;
        var num = Hue % 360.0;
        var num2 = Saturation / 255.0;
        var num3 = Lightness / 255.0;
        if (num2 == 0.0)
        {
            num4 = num3;
            num5 = num3;
            num6 = num3;
        }
        else
        {
            var d = num / 60.0;
            var num11 = (int) Math.Floor(d);
            var num10 = d - num11;
            var num7 = num3 * (1.0 - num2);
            var num8 = num3 * (1.0 - (num2 * num10));
            var num9 = num3 * (1.0 - (num2 * (1.0 - num10)));
            switch (num11)
            {
                case 0:
                    num4 = num3;
                    num5 = num9;
                    num6 = num7;
                    break;
                case 1:
                    num4 = num8;
                    num5 = num3;
                    num6 = num7;
                    break;
                case 2:
                    num4 = num7;
                    num5 = num3;
                    num6 = num9;
                    break;
                case 3:
                    num4 = num7;
                    num5 = num8;
                    num6 = num3;
                    break;
                case 4:
                    num4 = num9;
                    num5 = num7;
                    num6 = num3;
                    break;
                case 5:
                    num4 = num3;
                    num5 = num7;
                    num6 = num8;
                    break;
            }
        }
        return new Color((int) (num4 * 255.0), (int) (num5 * 255.0), (int) (num6 * 255.0));
    }


    /// <summary>
    /// 将 xxh 转化为 { xx.xxx 时 xx 分 xx 秒 }，数字用彩色强调，颜色不填时只返回纯文本
    /// </summary>
    /// <param name="h"></param>
    /// <param name="color"> 修改数字的颜色,不填时默认原色，泰拉游戏中的颜色 [c/十六进制:文本]，只用填十六进制即可 </param>
    /// <returns></returns>
    private static string HoursToM(double hours, string color = "")
    {
        var mess = "";
        if (hours >= 0)
        {
            int h, s, m; //时分秒
            h = (int) hours;
            m = (int) (hours % 1 * 60);//分
            var tempS = hours % 1 * 60 % 1 * 60;//秒
            s = tempS % 1 >= 0.5 ? (int) tempS + 1 : (int) tempS;
            //五入考虑进位
            if (s >= 60)
            {
                s -= 60;
                m += 1;
                if (m >= 60)
                {
                    m -= 60;
                    h += 1;
                }
            }
            if (!string.IsNullOrWhiteSpace(color))
            {
                if (h > 0)
                {
                    mess += GetString($" [c/{color}:{h}] 时");
                }

                if (m > 0)
                {
                    mess += GetString($" [c/{color}:{m}] 分");
                }

                if (s > 0 || (h == 0 && m == 0 && s == 0))
                {
                    mess += GetString($" [c/{color}:{s}] 秒");
                }
            }
            else
            {
                if (h > 0)
                {
                    mess += GetString($" {h} 时");
                }

                if (m > 0)
                {
                    mess += GetString($" {m} 分");
                }

                if (s > 0 || (h == 0 && m == 0 && s == 0))
                {
                    mess += GetString($" {s} 秒");
                }
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(color))
            {
                mess += GetString($" [c/{color}:0] 秒");
            }
            else
            {
                mess += GetString(" 0 秒");
            }
        }
        return mess;
    }


    /// <summary>
    /// 将 xxx 转化为 [c/十六进制:xxx] 带彩色的那种，颜色不填时只返回纯文本
    /// </summary>
    /// <param name="m"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private static string MtoM(string m, string color = "")
    {
        return color == "" ? m : $"[c/{color}:{m}]";
    }


    /// <summary>
    /// 完全纠正名字： 删除文件名中的非法字符，对null和空字符的默认World化，修剪空格前后缀，移除.wld后缀
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string CorrectFileName(string? name)
    {
        //对null和空字符的默认World化
        if (string.IsNullOrWhiteSpace(name))
        {
            return "World";
        }
        //删除文件名中的非法字符
        for (var i = 0; i < name.Length; ++i)
        {
            var flag = name[i] == '\\' || name[i] == '/' || name[i] == ':' || name[i] == '*' || name[i] == '?' || name[i] == '"' || name[i] == '<' || name[i] == '>' || name[i] == '|';
            if (flag)
            {
                name = name.Remove(i, 1);
                i--;
            }
        }
        //修剪空格前后缀
        while (name.StartsWith(' ') || name.EndsWith(' '))
        {
            name = name.Trim();
        }
        //移除.wld后缀
        if (name.EndsWith(".wld", StringComparison.OrdinalIgnoreCase))
        {
            name = name.Remove(name.Length - 4, 4);//name = name.Remove(name.LastIndexOf('.'), 4);
        }
        //再修剪下
        while (name.StartsWith(' ') || name.EndsWith(' '))
        {
            name = name.Trim();
        }
        return name;
    }


    /// <summary>
    /// 完全纠正一个指令上的格式错误：移除前缀"/","."，移除前后空格，缩短中间空格直1格，对null返回""
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string CorrectCommand(string? c)
    {
        if (string.IsNullOrWhiteSpace(c))
        {
            return "";
        }

        while (c.StartsWith('/') || c.StartsWith('.') || c.StartsWith(' ') || c.EndsWith(' ') || c.Contains("  "))
        {
            c = c.TrimStart('/', '.');
            c = c.Trim();
            c = c.Replace("  ", " ");
        }
        return c;
    }


    /// <summary>
    /// 泰拉在生成一个带空格的名字时会在文件名上用_代替空格，地图内部名称依然有空格，也就是说，泰拉本身生成的地图名称不会有空格。
    /// 功能：查找文件夹内是否有把空格换成_后相同的文件 或 名字直接相同的,即 ("a bc" == "a bc" || "a bc" == "a_bc") "a bc"为形参
    /// </summary>
    /// <param name="name"> 不含.wld的名称 </param>
    /// <param name="worldname"> 返回目录中确定"相同"的名字，若无返回 "" </param>
    /// <returns></returns>
    private static bool ExistWorldNamePlus(string name, out string worldname)
    {
        name = CorrectFileName(name);
        //如果名字完全一样，直接返回（我就当用户提供了带空格的并视为相同）
        if (File.Exists(config.path() + "/" + name + ".wld"))
        {
            worldname = name;
            return true;
        }
        //把提供的带空格的名字中的空格换成_，若存在也视为有
        if (File.Exists(config.path() + "/" + name.Replace(' ', '_') + ".wld"))
        {
            worldname = name.Replace(' ', '_');
            return true;
        }
        worldname = "";
        return false;
    }


    /// <summary>
    /// 用name或者net_id搜索有多少种npc,返回npc的net_id和name，注意是netid不是id
    /// </summary>
    /// <param name="nameOrID"></param>
    /// <returns></returns>
    private static Dictionary<int, string> FindNPCNameAndIDByNetid(string nameOrID)
    {
        var npcs = new Dictionary<int, string>();
        if (string.IsNullOrWhiteSpace(nameOrID))
        {
            return npcs;
        }

        if (int.TryParse(nameOrID, out var netID))
        {
            for (var i = -65; i < NPCID.Count; i++)
            {
                if (i == netID)
                {
                    npcs.Add(i, Lang.GetNPCNameValue(i));
                }
            }
            return npcs;
        }
        else
        {
            for (var i = -65; i < NPCID.Count; i++)
            {
                if (Lang.GetNPCNameValue(i).Contains(nameOrID))
                {
                    npcs.Add(i, Lang.GetNPCNameValue(i));
                }
            }
            return npcs;
        }
    }


    /// <summary>
    /// 获取这个玩家能使用的所有指令
    /// </summary>
    /// <returns></returns>
    private static List<string> getAllComCannotRun(TSPlayer player)
    {
        var list = new List<string>();
        Commands.ChatCommands.ForEach(x =>
        {
            if (!(x.CanRun(player) && (x.Name != "setup" || TShock.SetupToken != 0)))
            {
                list.AddRange(x.Names);
            }
        });
        return list;
    }


    /// <summary>
    /// 最好的发送消息，根据player的登录情况发送消息(我懒)
    /// </summary>
    /// <param name="player"></param>
    /// <param name="message"></param>
    /// <param name="color">Color.LimeGreen, Color.Yellow, Color.OrangeRed, Color.Red</param>
    private static void SendMessageAllAndMe(TSPlayer player, string message, Color color)
    {
        if (!player.IsLoggedIn)
        {
            player.SendMessage(message, color);
        }

        TSPlayer.All.SendMessage(message, color);
    }


    /// <summary>
    /// 将一个字符串含义为H:M:S转化成多少小时，若该字符串只有一个数字，默认为小时
    /// </summary>
    /// <param name="preTime">H:M:S或H</param>
    /// <returns></returns>
    private static double HMSorHoursToHours(string preTime, out string failureReason)
    {
        failureReason = "";
        if (double.TryParse(preTime, out var hour))
        {
            return hour;
        }
        var has负号 = false;
        var list = preTime.Split(':', '.').ToList();
        list.ForEach(x =>
        {
            while (x.StartsWith(' ') || x.EndsWith(' '))
            {
                x = x.Trim(' ');
            }
        });
        if (list.Count != 3)
        {
            failureReason = GetString("格式错误。应为[±num]或[±H:M:S]，num必须为整数或小数，H M S必须为整数或不填(不能为空格)，正号可以不加，冒号可用小数点代替但必须都要填");
            return double.MinValue;
        }
        else
        {
            int m = 0, s = 0;
            double sum;
            if (int.TryParse(list[0], out var h) || string.IsNullOrWhiteSpace(list[0]) || list[0] == "-" || list[0] == "+")
            {
                if (int.TryParse(list[1], out m) || string.IsNullOrWhiteSpace(list[1]))
                {
                    if (int.TryParse(list[2], out s) || string.IsNullOrWhiteSpace(list[2]))
                    {
                        if (h < 0)
                        {
                            has负号 = true;
                        }

                        if (list[0] == "-")
                        {
                            has负号 = true;
                        }
                        else if (list[0] == "+")
                        {
                            has负号 = false;
                        }

                        if (m < 0 || s < 0)
                        {
                            failureReason = GetString("格式错误。M 和 S 不需要加负号，负号只需要填入 H 的前面即可");
                            return double.MinValue;
                        }
                        if (m >= 60 || s >= 60)
                        {
                            failureReason = GetString("格式错误。M 和 S 不能大于59，请自行进位");
                            return double.MinValue;
                        }
                        sum = has负号 ? ((-1 * h) + (m / 60.0) + (s / 3600.0)) * -1 : h + (m / 60.0) + (s / 3600.0);

                        return sum;
                    }
                    else
                    {
                        failureReason = GetString("格式错误。S 必须为整数");
                        return double.MinValue;
                    }
                }
                else
                {
                    failureReason = GetString("格式错误。M 必须为整数");
                    return double.MinValue;
                }
            }
            else
            {
                failureReason = GetString("格式错误。H 必须为整数");
                return double.MinValue;
            }
        }
    }

    /*
    /// <summary>
    /// 只给在线游戏内玩家发送消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color">Color.LimeGreen, Color.Yellow, Color.OrangeRed, Color.Red</param>
    private static void SendMessageAll(string message, Color color)
    {
        foreach (var v in TShock.Players)
        {
            if (v != null && v.IsLoggedIn && v.Index != -1)
            {
                v.SendMessage(message, color);
            }
        }
    }
    */
}