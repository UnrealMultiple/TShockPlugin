using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class CGive
{
    public string Executer { get; set; } = "";

    public string cmd { get; set; } = "";

    public string who { get; set; } = "";

    public int id { get; set; }

    public bool Execute()
    {
        if (this.who == "-1")
        {
            TSPlayer? executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? TSPlayer.Server
                : TShock.Players.FirstOrDefault(p => p != null && p.Active &&
                      p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase));
            if (executer == null) return false;

            this.Save();
            using (var re = Data.QueryReader(
                "SELECT id FROM CGive WHERE executer=@0 AND cmd=@1 AND who=@2 ORDER BY id DESC LIMIT 1",
                this.Executer, this.cmd, this.who))
            {
                if (re.Read())
                    this.id = re.Reader.GetInt32(0);
            }
            foreach (var tSPlayer in TShock.Players)
            {
                if (tSPlayer is { Active: true })
                {
                    Commands.HandleCommand(executer, this.cmd.Replace("{name}", tSPlayer.Name));
                    new Given { Name = tSPlayer.Name, Id = this.id }.Save();
                }
            }
            return true;
        }

        // personal 模式：精确匹配在线玩家
        var target = TShock.Players.FirstOrDefault(p =>
            p != null && p.Active && p.Name.Equals(this.who, StringComparison.OrdinalIgnoreCase));
        if (target != null)
        {
            TSPlayer? executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? TSPlayer.Server
                : TShock.Players.FirstOrDefault(p => p != null && p.Active &&
                      p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase));
            if (executer != null)
            {
                Commands.HandleCommand(executer, this.cmd.Replace("{name}", target.Name));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 登录时执行：直接对已登录的玩家执行命令，不检查 Active，不写库，不遍历其他玩家。
    /// </summary>
    public bool ExecuteOnLogin(TSPlayer target)
    {
        TSPlayer executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
            ? TSPlayer.Server
            : TShock.Players.FirstOrDefault(p => p != null && p.Active &&
                  p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase))!;
        if (executer == null) return false;
        Commands.HandleCommand(executer, this.cmd.Replace("{name}", target.Name));
        return true;
    }

    public static IEnumerable<CGive> GetCGive()
    {
        var list = new List<CGive>();
        using (var re = Data.QueryReader("SELECT executer,cmd,who,id FROM CGive"))
        {
            while (re.Read())
            {
                list.Add(new CGive
                {
                    Executer = re.Reader.GetString(0),
                    cmd = re.Reader.GetString(1),
                    who = re.Reader.GetString(2),
                    id = re.Reader.GetInt32(3)
                });
            }
        }
        return list;
    }

    /// <summary>
    /// 只查询与指定玩家名相关的记录（personal 模式）和全体模式（who='-1'），避免全表扫描
    /// </summary>
    public static IEnumerable<CGive> GetCGiveForPlayer(string playerName)
    {
        var list = new List<CGive>();
        using var re = Data.QueryReader(
            "SELECT executer,cmd,who,id FROM CGive WHERE who=@0 OR who='-1'", playerName);
        while (re.Read())
        {
            list.Add(new CGive
            {
                Executer = re.Reader.GetString(0),
                cmd = re.Reader.GetString(1),
                who = re.Reader.GetString(2),
                id = re.Reader.GetInt32(3)
            });
        }
        return list;
    }

    /// <summary>
    /// 只查询指定玩家名的 personal 模式记录，用于 REST 接口等精确查询场景。
    /// </summary>
    public static IEnumerable<CGive> GetCGiveByWho(string who)
    {
        var list = new List<CGive>();
        using var re = Data.QueryReader(
            "SELECT executer,cmd,who,id FROM CGive WHERE who=@0", who);
        while (re.Read())
        {
            list.Add(new CGive
            {
                Executer = re.Reader.GetString(0),
                cmd = re.Reader.GetString(1),
                who = re.Reader.GetString(2),
                id = re.Reader.GetInt32(3)
            });
        }
        return list;
    }

    public void Save()
    {
        Data.Command($"INSERT INTO CGive(executer,cmd,who) VALUES (@0,@1,@2)", this.Executer, this.cmd, this.who);
    }

    public void Del()
    {
        Data.Command($"DELETE FROM CGive WHERE id=@0", this.id);
    }
}