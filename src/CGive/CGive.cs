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
            if (this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                || TShock.Players.Any(p => p != null && p.Active && p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase)))
            {
                this.Save();
                this.id = Data.GetLastInsertId();

                var executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                    ? TSPlayer.Server
                    : (TSPlayer?) TShock.Players.FirstOrDefault(p => p != null && p.Active
                        && p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase));

                foreach (var tSPlayer in TShock.Players)
                {
                    if (tSPlayer is { Active: true })
                    {
                        Commands.HandleCommand(executer!, this.cmd.Replace("{name}", tSPlayer.Name));
                        new Given { Name = tSPlayer.Name, Id = this.id }.Save();
                    }
                }
                return true;
            }
            return false;
        }

        // personal 模式：大小写不敏感精确匹配
        var target = TShock.Players.FirstOrDefault(p =>
            p != null && p.Active && p.Name.Equals(this.who, StringComparison.OrdinalIgnoreCase));
        if (target != null)
        {
            var executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? TSPlayer.Server
                : (TSPlayer?) TShock.Players.FirstOrDefault(p => p != null && p.Active
                    && p.Name.Equals(this.Executer, StringComparison.OrdinalIgnoreCase));
            if (executer != null)
            {
                Commands.HandleCommand(executer, this.cmd.Replace("{name}", target.Name));
                return true;
            }
        }
        return false;
    }

    public static IEnumerable<CGive> GetCGive()
    {
        var list = new List<CGive>();
        using var re = Data.QueryReader("SELECT executer,cmd,who,id FROM CGive");
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
    /// 只查询与指定玩家名相关的记录（personal 模式）和 all 模式记录，减少全表扫描
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

    public void Save()
    {
        Data.Command($"INSERT INTO CGive(executer,cmd,who) VALUES (@0,@1,@2)", this.Executer, this.cmd, this.who);
    }

    public void Del()
    {
        Data.Command($"DELETE FROM CGive WHERE id=@0", this.id);
    }
}