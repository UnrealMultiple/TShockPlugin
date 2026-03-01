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
        var list = TSPlayer.FindByNameOrID(this.Executer);
        if (this.who == "-1")
        {
            if (list.Count > 0 || this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase))
            {
                this.Save();
                using (var re = Data.QueryReader(
                    "SELECT id FROM CGive WHERE executer=@0 AND cmd=@1 AND who=@2 ORDER BY id DESC LIMIT 1",
                    this.Executer, this.cmd, this.who))
                {
                    if (re.Read())
                    {
                        this.id = re.Reader.GetInt32(0);
                    }
                }
                foreach (var tSPlayer in TShock.Players)
                {
                    if (tSPlayer is { Active: true })
                    {
                        Commands.HandleCommand(
                            this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                                ? TSPlayer.Server
                                : list[0],
                            this.cmd.Replace("{name}", tSPlayer.Name));
                        var given = new Given { Name = tSPlayer.Name, Id = this.id };
                        given.Save();
                    }
                }
                return true;
            }
            return false;
        }

        // personal 模式：精确匹配在线玩家
        var target = TShock.Players.FirstOrDefault(p =>
            p != null && p.Active && p.Name == this.who);
        if (target != null)
        {
            var executer = this.Executer.Equals("server", StringComparison.OrdinalIgnoreCase)
                ? TSPlayer.Server
                : TShock.Players.FirstOrDefault(p => p != null && p.Active && p.Name == this.Executer);
            if (executer != null)
            {
                Commands.HandleCommand(executer, this.cmd.Replace("{name}", this.who));
                return true;
            }
        }
        return false;
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

    public void Save()
    {
        Data.Command($"INSERT INTO CGive(executer,cmd,who) VALUES (@0,@1,@2)", this.Executer, this.cmd, this.who);
    }

    public void Del()
    {
        Data.Command($"DELETE FROM CGive WHERE id=@0", this.id);
    }
}