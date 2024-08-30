using TShockAPI;
using TShockAPI.DB;

namespace CGive;

public class Data
{
    public static void Init()
    {
        TShock.DB.Query("create table if not exists CGive(executer text,cmd text,who text,id int(32));" +
            "create table if not exists Given(name text,id int(32))");
    }

    public static void Command(string cmd, params object[] args)
    {
        TShock.DB.Query(cmd, args);
    }
}
