using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
namespace TimerKeeper
{
    public static class DB
    {
        private static IDbConnection db => TShock.DB;
        public static void Connect()
        {
            SqlTableCreator sqlcreator = new SqlTableCreator(db, new MysqlQueryCreator());

            sqlcreator.EnsureTableStructure(new SqlTable("TimerKeeper",
                new SqlColumn("X", MySqlDbType.Int32) { Length = 4 },
                new SqlColumn("Y", MySqlDbType.Int32) { Length = 4 }));

        }


        public static  List<TimerPos> LoadAll()
        {
            List<TimerPos> dataInfos = new();
            using (QueryResult result = db.QueryReader("SELECT * FROM TimerKeeper;"))
            {
                
                while (result.Read())
                {
                    dataInfos.Add(new TimerPos
                    {
                        X = result.Get<int>("X"),
                        Y = result.Get<int>("Y")

                    });
                }
            }
            return dataInfos;
        }

        public static void AddTimer(int x,int y)
        {
            db.Query("INSERT INTO TimerKeeper (X, Y) VALUES (@0, @1);", x, y);
        }

        public static void RemoveTimer(int x,int y)
        {
            db.Query("DELETE FROM TimerKeeper WHERE X=@0 AND Y=@1;", x,y);
        }

        public static void ClearDB()
        {
            db.Query("DELETE FROM TimerKeeper;");
        }

        public class TimerPos
        {
           public int X { get; set; }
            public int Y { get; set; }
        }

    }
}
