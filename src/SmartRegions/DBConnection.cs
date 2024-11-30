using Microsoft.Data.Sqlite;
using TShockAPI;

namespace SmartRegions;

sealed class DBConnection
{
    SqliteConnection Connection = null!;

    public void Initialize()
    {
        var folderPath = Path.Combine(TShock.SavePath, "SmartRegions");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        var path = Path.Combine(folderPath, "SmartRegions.sqlite");
        this.Connection = new SqliteConnection($"Data Source='{path}'");
        this.Connection.Open();
        var command = new SqliteCommand(
            "CREATE TABLE IF NOT EXISTS Regions (" +
            "  Name TEXT PRIMARY KEY," +
            "  Command TEXT," +
            "  Cooldown REAL)",
            this.Connection);
        command.ExecuteNonQuery();
    }

    public List<SmartRegion> GetRegions()
    {
        var result = new List<SmartRegion>();
        var command = new SqliteCommand("SELECT Name, Command, Cooldown FROM Regions", this.Connection);
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var name = reader.GetString(0);
                var cmd = reader.GetString(1);
                var cooldown = reader.GetDouble(2);
                result.Add(new SmartRegion { name = name, command = cmd, cooldown = cooldown });
            }
        }
        return result;
    }

    public async Task SaveRegion(SmartRegion region)
    {
        var command = new SqliteCommand(
            "INSERT OR REPLACE INTO Regions (Name, Command, Cooldown) VALUES (@nm, @cmd, @cool)",
            this.Connection);
        command.Parameters.Add(new SqliteParameter("@nm", region.name));
        command.Parameters.Add(new SqliteParameter("@cmd", region.command));
        command.Parameters.Add(new SqliteParameter("@cool", region.cooldown));
        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveRegion(string region)
    {
        var command = new SqliteCommand("DELETE FROM Regions WHERE Name = @nm", this.Connection);
        command.Parameters.Add(new SqliteParameter("@nm", region));
        await command.ExecuteNonQueryAsync();
    }

    public void Close()
    {
        this.Connection?.Close();
    }
}