using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace EssentialsPlus.Db;

public class TpAllowManager
{
    public class TpAllow
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public TpAllow(string name, bool isEnabled)
        {
            this.Name = name;
            this.IsEnabled = isEnabled;
        }
    }

    private readonly IDbConnection db;
    private readonly List<TpAllow> TpAllows = new();

    public TpAllowManager(IDbConnection db)
    {
        this.db = db;
        var sqlCreator = new SqlTableCreator(db, db.GetSqlQueryBuilder());
        sqlCreator.EnsureTableStructure(new SqlTable("TpAllows",
            new SqlColumn("Name", MySqlDbType.Text),
            new SqlColumn("IsEnabled", MySqlDbType.Int32)));

        using var result = db.QueryReader("SELECT * FROM TpAllows");
        while (result.Read())
        {
            this.TpAllows.Add(new TpAllow(
                result.Get<string>("Name"),
                result.Get<bool>("IsEnabled")));
        }
    }

    public bool ToggleTpAllow(TSPlayer player)
    {
        try
        {
            var existing = this.TpAllows.Find(f => f.Name == player.Name);
            bool newState;

            if (existing == null)
            {
                newState = true;
                this.TpAllows.Add(new TpAllow(player.Name, newState));

                if (this.db.Query("INSERT OR REPLACE INTO TpAllows VALUES (@0, @1)",
                    player.Name, newState ? 1 : 0) > 0)
                {
                    player.TPAllow = newState;
                    return true;
                }
                return false;
            }
            else
            {
                newState = !existing.IsEnabled;
                existing.IsEnabled = newState;

                if (this.db.Query("UPDATE TpAllows SET IsEnabled = @0 WHERE Name = @1",
                    newState ? 1 : 0, player.Name) > 0)
                {
                    player.TPAllow = newState;
                    return true;
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"ToggleTpAllow error for {player.Name}: {ex}");
            return false;
        }
    }

    public bool SetTpAllow(TSPlayer player, bool isEnabled)
    {
        try
        {
            var existing = this.TpAllows.Find(f => f.Name == player.Name);

            if (existing == null)
            {
                this.TpAllows.Add(new TpAllow(player.Name, isEnabled));

                if (this.db.Query("INSERT OR REPLACE INTO TpAllows VALUES (@0, @1)",
                    player.Name, isEnabled ? 1 : 0) > 0)
                {
                    player.TPAllow = isEnabled;
                    return true;
                }
                return false;
            }
            else
            {
                existing.IsEnabled = isEnabled;
                if (this.db.Query("UPDATE TpAllows SET IsEnabled = @0 WHERE Name = @1",
                    isEnabled ? 1 : 0, player.Name) > 0)
                {
                    player.TPAllow = isEnabled;
                    return true;
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"SetTpAllow error for {player.Name}: {ex}");
            return false;
        }
    }

    public bool IsTpAllowed(TSPlayer player)
    {
        var record = this.TpAllows.Find(f => f.Name == player.Name);
        if (record != null)
        {
            player.TPAllow = record.IsEnabled;
            return record.IsEnabled;
        }

        try
        {
            using var result = this.db.QueryReader("SELECT IsEnabled FROM TpAllows WHERE Name = @0", player.Name);
            if (result.Read())
            {
                var isEnabled = result.Get<bool>("IsEnabled");
                this.TpAllows.Add(new TpAllow(player.Name, isEnabled));
                player.TPAllow = isEnabled;
                return isEnabled;
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"查询玩家 {player.Name} 的传送权限时发生错误: {ex}");
        }
        player.TPAllow = false;
        return false;
    }
}