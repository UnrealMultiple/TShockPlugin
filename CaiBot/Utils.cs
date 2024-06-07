using System.Data;

namespace CaiBot
{
    public static class Utils
    {
        public static List<int> GetActiveBuffs(IDbConnection connection, int userId, string name)
        {
            try
            {
                string queryString2 = $"SELECT buffid FROM Permabuff WHERE Name = '{name}'";

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = queryString2;

                    connection.Open();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string activeBuffsString = reader.GetString(0);
                            List<int> activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                            return activeBuffsList;
                        }
                    }
                }

            }
            catch { }
            try
            {
                string queryString = $"SELECT ActiveBuffs FROM Permabuffs WHERE UserID = '{userId}'";

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = queryString;

                    connection.Open();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string activeBuffsString = reader.GetString(0);
                            List<int> activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                            return activeBuffsList;
                        }
                    }
                }

            }
            catch { }
            return new List<int>();
        }
    }
}
