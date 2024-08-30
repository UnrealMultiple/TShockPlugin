using System.Data;

namespace CaiBot;

public static class Utils
{
    public static List<int> GetActiveBuffs(IDbConnection connection, int userId, string name)
    {
        try
        {
            var queryString2 = $"SELECT buffid FROM Permabuff WHERE Name = '{name}'";

            using var command = connection.CreateCommand();
            command.CommandText = queryString2;

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var activeBuffsString = reader.GetString(0);
                    var activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                    return activeBuffsList;
                }
            }
        }
        catch
        {
        }

        try
        {
            var queryString = $"SELECT ActiveBuffs FROM Permabuffs WHERE UserID = '{userId}'";

            using var command = connection.CreateCommand();
            command.CommandText = queryString;

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var activeBuffsString = reader.GetString(0);
                    var activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                    return activeBuffsList;
                }
            }
        }
        catch
        {
        }

        return new List<int>();
    }
}