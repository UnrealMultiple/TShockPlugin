using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace TrCDK;

public class CDK
{
    public string Cdkname { get; set; } = "";
    public int Usetime { get; set; }
    public long Utiltime { get; set; }
    public string Grouplimit { get; set; } = "";
    public string Playerlimit { get; set; } = "";
    public string Used { get; set; } = "";
    public string Cmds { get; set; } = "";
}

internal class Data
{
    public static SqliteConnection? DB;
    const string path = "tshock/TrCDK.sqlite";

    public static void Init()
    {
        DB = new SqliteConnection($"Data Source={path};");
        DB.Open();

        // 使用参数化命令创建表
        using var cmd = new SqliteCommand(
            "CREATE TABLE IF NOT EXISTS Data(" +
            "CDKname TEXT PRIMARY KEY, " +
            "Usetime INTEGER, " +
            "Utiltime INTEGER, " +
            "Grouplimit TEXT, " +
            "Playerlimit TEXT, " +
            "Used TEXT, " +
            "Cmds TEXT)", DB);
        cmd.ExecuteNonQuery();
    }

    public static bool Insert(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Cmds)
    {
        if (DB == null)
        {
            throw new InvalidOperationException(GetString("数据库未初始化"));
        }

        // 使用参数化查询检查CDK是否已存在
        using (var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Data WHERE CDKname = @cdkname", DB))
        {
            checkCmd.Parameters.AddWithValue("@cdkname", CDKname);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                return false; // CDK已存在
            }
        }

        // 使用参数化查询插入新CDK
        using var insertCmd = new SqliteCommand(
            "INSERT INTO Data (CDKname, Usetime, Utiltime, Grouplimit, Playerlimit, Used, Cmds) " +
            "VALUES (@cdkname, @usetime, @utiltime, @grouplimit, @playerlimit, @used, @cmds)", DB);

        insertCmd.Parameters.AddWithValue("@cdkname", CDKname);
        insertCmd.Parameters.AddWithValue("@usetime", Usetime);
        insertCmd.Parameters.AddWithValue("@utiltime", Utiltime);
        insertCmd.Parameters.AddWithValue("@grouplimit", Grouplimit);
        insertCmd.Parameters.AddWithValue("@playerlimit", Playerlimit);
        insertCmd.Parameters.AddWithValue("@used", "");
        insertCmd.Parameters.AddWithValue("@cmds", Cmds);

        insertCmd.ExecuteNonQuery();
        return true;
    }

    public static void Update(string CDKname, int Usetime, long Utiltime, string Grouplimit, string Playerlimit, string Used, string Cmds)
    {
        if (DB == null)
        {
            throw new InvalidOperationException(GetString("数据库未初始化"));
        }

        using var updateCmd = new SqliteCommand(
            "UPDATE Data SET Usetime = @usetime, Utiltime = @utiltime, Grouplimit = @grouplimit, " +
            "Playerlimit = @playerlimit, Used = @used, Cmds = @cmds WHERE CDKname = @cdkname", DB);

        updateCmd.Parameters.AddWithValue("@usetime", Usetime);
        updateCmd.Parameters.AddWithValue("@utiltime", Utiltime);
        updateCmd.Parameters.AddWithValue("@grouplimit", Grouplimit);
        updateCmd.Parameters.AddWithValue("@playerlimit", Playerlimit);
        updateCmd.Parameters.AddWithValue("@used", Used);
        updateCmd.Parameters.AddWithValue("@cmds", Cmds);
        updateCmd.Parameters.AddWithValue("@cdkname", CDKname);

        updateCmd.ExecuteNonQuery();
    }

    public static CDK GetData(string name)
    {
        if (DB == null)
        {
            throw new InvalidOperationException(GetString("数据库未初始化"));
        }

        var result = new CDK();
        using var cmd = new SqliteCommand("SELECT * FROM Data WHERE CDKname = @name", DB);
        cmd.Parameters.AddWithValue("@name", name);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            result = new CDK()
            {
                Cdkname = reader.GetString(0),
                Usetime = reader.GetInt32(1),
                Utiltime = reader.GetInt64(2),
                Grouplimit = reader.GetString(3),
                Playerlimit = reader.GetString(4),
                Used = reader.GetString(5),
                Cmds = reader.GetString(6)
            };
        }
        return result;
    }

    public static CDK[] GetAllData()
    {
        if (DB == null)
        {
            throw new InvalidOperationException(GetString("数据库未初始化"));
        }

        var result = new List<CDK>();
        using var cmd = new SqliteCommand("SELECT * FROM Data", DB);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new CDK()
            {
                Cdkname = reader.GetString(0),
                Usetime = reader.GetInt32(1),
                Utiltime = reader.GetInt64(2),
                Grouplimit = reader.GetString(3),
                Playerlimit = reader.GetString(4),
                Used = reader.GetString(5),
                Cmds = reader.GetString(6)
            });
        }
        return result.ToArray();
    }

    public static bool DelCDK(string name)
    {
        if (DB == null)
        {
            throw new InvalidOperationException(GetString("数据库未初始化"));
        }

        using var cmd = new SqliteCommand("DELETE FROM Data WHERE CDKname = @name", DB);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.ExecuteNonQuery();
        return true;
    }

    // 关闭数据库连接的
    public static void Close()
    {
        DB?.Close();
        DB?.Dispose();
        DB = null;
    }
}