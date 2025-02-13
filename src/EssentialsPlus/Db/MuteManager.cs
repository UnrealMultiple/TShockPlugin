using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace EssentialsPlus.Db;

public class MuteManager
{
    public class Mute
    {
        public string Name { get; set; }

        public string UUID { get; set; }

        public int ID { get; set; }

        public string IP { get; set; }

        public DateTime Date { get; set; }

        public DateTime Expiration { get; set; }

        public Mute(string name, string uuid, int id, string ip, string time, string exp)
        {
            this.Name = name;
            this.UUID = uuid;
            this.ID = id;
            this.IP = ip;
            this.Date = DateTime.Parse(time);
            this.Expiration = DateTime.Parse(exp);
        }

        public Mute(string name, string uuid, int id, string ip, DateTime time, DateTime exp)
        {
            this.Name = name;
            this.UUID = uuid;
            this.ID = id;
            this.IP = ip;
            this.Date = time;
            this.Expiration = exp;
        }
    }
    private readonly IDbConnection db;

    private readonly List<Mute> Mutes = new();

    public MuteManager(IDbConnection db)
    {
        this.db = db;

        var sqlCreator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite
            ? new SqliteQueryCreator()
            : new MysqlQueryCreator());

        sqlCreator.EnsureTableStructure(new SqlTable("Mutes",
            new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
            new SqlColumn("Name", MySqlDbType.Text),
            new SqlColumn("UUID", MySqlDbType.Text),
            new SqlColumn("IP", MySqlDbType.Text),
            new SqlColumn("Date", MySqlDbType.Text),
            new SqlColumn("Expiration", MySqlDbType.Text)));

        using var result = db.QueryReader("SELECT * FROM Mutes");
        while (result.Read())
        {
            this.Mutes.Add(new Mute(
                result.Get<string>("Name"),
                result.Get<string>("UUID"),
                result.Get<int>("ID"),
                result.Get<string>("IP"),
                result.Get<string>("Date"),
                result.Get<string>("Expiration")));
        }
    }

    public bool AddMute(TSPlayer player, DateTime expiration)
    {
        try
        {
            this.Mutes.Add(new(
                player.Name,
                player.UUID,
                player.Account.ID,
                player.IP,
                DateTime.Now,
                expiration
                ));
            return this.db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
                null,
                player.Name,
                player.UUID,
                player.IP,
                DateTime.Now.ToString(),
                expiration.ToString("s")) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
    public bool AddMute(UserAccount user, DateTime expiration)
    {
        try
        {
            var ip = JsonConvert.DeserializeObject<List<string>>(user.KnownIps)?[0];
            this.Mutes.Add(new(
                user.Name,
                user.UUID,
                user.ID,
                ip!,
                DateTime.Now,
                expiration
                ));

            return this.db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
                null,
                user.Name,
                user.UUID,
                ip,
                DateTime.Now.ToString(),
                expiration.ToString("s")) > 0;

        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
    public bool DeleteMute(TSPlayer player)
    {
        var query = this.db.GetSqlType() == SqlType.Mysql ?
            "DELETE FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1" :
            "DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)";

        try
        {
            this.Mutes.RemoveAll(r => r.UUID == player.UUID);
            return this.db.Query(query, player.UUID, player.IP) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
    public bool DeleteMute(UserAccount user)
    {
        var query = this.db.GetSqlType() == SqlType.Mysql ?
            "DELETE FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1" :
            "DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)";

        try
        {
            this.Mutes.RemoveAll(r => r.UUID == user.UUID);
            return this.db.Query(query, user.UUID, JsonConvert.DeserializeObject<List<string>>(user.KnownIps)?[0]) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }

    public DateTime GetExpiration(TSPlayer player)
    {
        var muteRecord = this.Mutes.Find(f => f.UUID == player.UUID);
        if (muteRecord != null)
        {
            return muteRecord.Expiration;
        }
        if (Config.Instance.IsMuteCheckIP)
        {
            muteRecord = this.Mutes.Find(f => f.IP == player.IP);
            if (muteRecord != null)
            {
                return muteRecord.Expiration;
            }
        }
        muteRecord = this.Mutes.Find(f => f.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase));

        return muteRecord != null ? muteRecord.Expiration : DateTime.MinValue;
    }
}