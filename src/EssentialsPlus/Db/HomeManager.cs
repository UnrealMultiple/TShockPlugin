using MySql.Data.MySqlClient;
using System.Data;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace EssentialsPlus.Db;

public class HomeManager
{
    private readonly IDbConnection db;
    private readonly List<Home> homes = new List<Home>();
    private readonly object syncLock = new object();

    public HomeManager(IDbConnection db)
    {
        this.db = db;

        var sqlCreator = new SqlTableCreator(db,
            db.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());
        sqlCreator.EnsureTableStructure(new SqlTable("Homes",
            new SqlColumn("ID", MySqlDbType.Int32) { AutoIncrement = true, Primary = true },
            new SqlColumn("UserID", MySqlDbType.Int32),
            new SqlColumn("Name", MySqlDbType.Text),
            new SqlColumn("X", MySqlDbType.Double),
            new SqlColumn("Y", MySqlDbType.Double),
            new SqlColumn("WorldID", MySqlDbType.Int32)));

        using var result = db.QueryReader("SELECT * FROM Homes WHERE WorldID = @0", Main.worldID);
        while (result.Read())
        {
            this.homes.Add(new Home(
                result.Get<int>("UserID"),
                result.Get<string>("Name"),
                result.Get<float>("X"),
                result.Get<float>("Y")));
        }
    }

    public bool AddHome(TSPlayer player, string name, float x, float y)
    {
        try
        {
            this.homes.Add(new Home(player.Account.ID, name, x, y));
            return this.db.Query("INSERT INTO Homes (UserID, Name, X, Y, WorldID) VALUES (@0, @1, @2, @3, @4)",
                player.Account.ID,
                name,
                x,
                y,
                Main.worldID) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }

    public bool DeleteHome(TSPlayer player, string name)
    {
        var query = this.db.GetSqlType() == SqlType.Mysql
            ? "DELETE FROM Homes WHERE UserID = @0 AND Name = @1 AND WorldID = @2"
            : "DELETE FROM Homes WHERE UserID = @0 AND Name = @1 AND WorldID = @2 COLLATE NOCASE";
        try
        {

            this.homes.RemoveAll(h => h.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                && h.UserID == player.Account.ID);
            return this.db.Query(query, player.Account.ID, name, Main.worldID) > 0;

        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }

    }

    public Home? GetHome(TSPlayer player, string name)
    {
        return
            this.homes.Find(h =>
                h.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                && h.UserID == player.Account.ID);
    }

    public List<Home> GetAllAsync(TSPlayer player)
    {
        return this.homes.FindAll(h => h.UserID == player.Account.ID);
    }

    public bool Reload()
    {
        try
        {
            this.homes.Clear();
            using (var result = this.db.QueryReader("SELECT * FROM Homes WHERE WorldID = @0", Main.worldID))
            {
                while (result.Read())
                {
                    this.homes.Add(new Home(
                        result.Get<int>("UserID"),
                        result.Get<string>("Name"),
                        result.Get<float>("X"),
                        result.Get<float>("Y")));
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }

    public bool UpdateHome(TSPlayer player, string name, float x, float y)
    {
        var query = this.db.GetSqlType() == SqlType.Mysql
            ? "UPDATE Homes SET X = @0, Y = @1 WHERE UserID = @2 AND Name = @3 AND WorldID = @4"
            : "UPDATE Homes SET X = @0, Y = @1 WHERE UserID = @2 AND Name = @3 AND WorldID = @4 COLLATE NOCASE";
        try
        {
            this.homes.RemoveAll(h => h.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                && h.UserID == player.Account.ID);
            this.homes.Add(new Home(player.Account.ID, name, x, y));
            return this.db.Query(query, x, y, player.Account.ID, name, Main.worldID) > 0;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            return false;
        }
    }
}