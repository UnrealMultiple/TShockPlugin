using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using Terraria;
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
            Name = name;
            UUID = uuid;
            ID = id;
            IP = ip;
            Date = DateTime.Parse(time);
            Expiration = DateTime.Parse(exp);
        }

        public Mute(string name, string uuid, int id, string ip, DateTime time, DateTime exp)
        {
            Name = name;
            UUID = uuid;
            ID = id;
            IP = ip;
            Date = time;
            Expiration = exp;
        }
    }
    private IDbConnection db;

    private List<Mute> Mutes = new();

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

        using QueryResult result = db.QueryReader("SELECT * FROM Homes WHERE WorldID = @0", Main.worldID);
        while (result.Read())
        {
            Mutes.Add(new Mute(
                result.Get<string>("Name"),
                result.Get<string>("UUID"),
                result.Get<int>("ID"),
                result.Get<string>("IP"),
                result.Get<string>("Date"),
                result.Get<string>("Expiration")));
        }
    }

    public bool AddAsync(TSPlayer player, DateTime expiration)
    {
        try
        {
            Mutes.Add(new(
                player.Name,
                player.UUID,
                player.Account.ID,
                player.IP,
                DateTime.Now,
                expiration
                ));
            return db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
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
    public bool AddAsync(UserAccount user, DateTime expiration)
    {
        try
        {
            var ip = JsonConvert.DeserializeObject<List<string>>(user.KnownIps)?[0];
            Mutes.Add(new(
                user.Name,
                user.UUID,
                user.ID,
                ip,
                DateTime.Now,
                expiration
                ));

            return db.Query("INSERT INTO Mutes VALUES (@0, @1, @2, @3, @4, @5)",
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
    public bool DeleteAsync(TSPlayer player)
    {
        string query = db.GetSqlType() == SqlType.Mysql ?
            "DELETE FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1" :
            "DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)";

        try
        {
            Mutes.RemoveAll(r => r.UUID == player.UUID);
            return db.Query(query, player.UUID, player.IP) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
    public bool DeleteAsync(UserAccount user)
    {
        string query = db.GetSqlType() == SqlType.Mysql ?
            "DELETE FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1" :
            "DELETE FROM Mutes WHERE ID IN (SELECT ID FROM Mutes WHERE UUID = @0 OR IP = @1 ORDER BY ID DESC LIMIT 1)";

        try
        {
            Mutes.RemoveAll(r => r.UUID == user.UUID);
            return db.Query(query, user.UUID, JsonConvert.DeserializeObject<List<string>>(user.KnownIps)?[0]) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
    public DateTime GetExpirationAsync(TSPlayer player)
    {
        return Mutes.Find(f => f.UUID == player.UUID)?.Expiration ?? DateTime.MinValue;
    }
}
