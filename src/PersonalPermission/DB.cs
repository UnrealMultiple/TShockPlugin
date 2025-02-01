using TShockAPI;
using TShockAPI.DB;

namespace PersonalPermission;

class DB
{
    public static void TryCreateTable()
    {
        try
        {
            var sqlTable = new SqlTable("PersonalPermission", new SqlColumn[]
            {
                new SqlColumn("UserID", MySql.Data.MySqlClient.MySqlDbType.Int32){ Primary = true },
                new SqlColumn("Permissions", MySql.Data.MySqlClient.MySqlDbType.Text)
            });
            var db = TShock.DB;
            IQueryBuilder queryBuilder2;
            if (DbExt.GetSqlType(TShock.DB) != SqlType.Sqlite)
            {
                IQueryBuilder queryBuilder = new MysqlQueryCreator();
                queryBuilder2 = queryBuilder;
            }
            else
            {
                IQueryBuilder queryBuilder = new SqliteQueryCreator();
                queryBuilder2 = queryBuilder;
            }
            new SqlTableCreator(db, queryBuilder2).EnsureTableStructure(sqlTable);
        }
        catch (Exception ex) { TShock.Log.Error(ex.Message); }
    }
    public static bool AddUser(int id)
    {
        try { return DbExt.Query(TShock.DB, $"INSERT INTO PersonalPermission (UserID) VALUES ({id});") != -1; }
        catch (Exception ex) { TShock.Log.ConsoleError(GetString($"添加玩家信息失败.\n{ex}")); return false; }
    }
    public static List<string> GetPermissions(int id)
    {
        using (var reader = DbExt.QueryReader(TShock.DB, $"SELECT * FROM PersonalPermission WHERE UserID='{id}';"))
        {
            var list = new List<string>();
            if (reader.Read())
            {
                try
                {
                    var text = reader.Get<string>("Permissions") ?? "";
                    if (text.Contains(","))
                    {
                        text.Split(',').ForEach(p => { if (!list.Contains(p)) { list.Add(p); } });
                    }
                    else if (!string.IsNullOrWhiteSpace(text))
                    {
                        list.Add(text);
                    }
                }
                catch (Exception ex) { TShock.Log.ConsoleError(GetString($"[PersonalPermission] 读取数据库时发生错误.\n{ex}")); }
            }
            else
            {
                AddUser(id);
            }

            return list;
        }
    }
    public static Dictionary<int, List<string>> GetAllPermissions()
    {
        using (var reader = DbExt.QueryReader(TShock.DB, $"SELECT * FROM PersonalPermission;"))
        {
            var list = new Dictionary<int, List<string>>();
            while (reader.Read())
            {
                try
                {
                    var permissions = new List<string>();
                    var text = reader.Get<string>("Permissions") ?? "";
                    if (text.Contains(","))
                    {
                        text.Split(',').ForEach(p => { if (!permissions.Contains(p)) { permissions.Add(p); } });
                    }
                    else if (!string.IsNullOrWhiteSpace(text))
                    {
                        permissions.Add(text);
                    }

                    list.Add(reader.Get<int>("UserID"), permissions);
                }
                catch (Exception ex) { TShock.Log.ConsoleError(GetString($"[PersonalPermission] 读取数据库时发生错误.\n{ex}")); }
            }
            return list;
        }
    }
    public static bool SetPermissions(int id, List<string> list)
    {
        if (DbExt.Query(TShock.DB, $"UPDATE PersonalPermission SET Permissions=@0 WHERE UserID={id};", new object[] { string.Join(",", list) }) != -1)
        {
            return true;
        }
        TShock.Log.ConsoleError(GetString("保存玩家权限失败."));
        return false;
    }
}