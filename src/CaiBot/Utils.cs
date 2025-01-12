using System.Data;
using System.IO.Compression;
using System.Text;
using Terraria;

namespace CaiBot;

internal static class Utils
{

    internal static List<string> GetOnlineProcessList()
    {
        List<string> onlineProcessList = new ();
        #region 进度查询

        if (!NPC.downedSlimeKing)
        {
            onlineProcessList.Add("史王");
        }

        if (!NPC.downedBoss1)
        {
            onlineProcessList.Add("克眼");
        }

        if (!NPC.downedBoss2)
        {
            if (Main.drunkWorld)
            {
                onlineProcessList.Add("世吞/克脑");
            }
            else
            {
                onlineProcessList.Add(WorldGen.crimson ? "克脑" : "世吞");
            }
        }

        if (!NPC.downedBoss3)
        {
            onlineProcessList.Add("骷髅王");
        }

        if (!Main.hardMode)
        {
            onlineProcessList.Add("血肉墙");
        }

        if (!NPC.downedMechBoss2 || !NPC.downedMechBoss1 || !NPC.downedMechBoss3)
        {
            onlineProcessList.Add(Main.zenithWorld ? "美杜莎" : "新三王");
        }

        if (!NPC.downedPlantBoss)
        {
            onlineProcessList.Add("世花");
        }

        if (!NPC.downedGolemBoss)
        {
            onlineProcessList.Add("石巨人");
        }

        if (!NPC.downedAncientCultist)
        {
            onlineProcessList.Add("拜月教徒");
        }

        if (!NPC.downedTowers)
        {
            onlineProcessList.Add("四柱");
        }

        if (!NPC.downedMoonlord)
        {
            onlineProcessList.Add("月总");
        }

        return onlineProcessList;


        #endregion
    }
    
    internal static string FileToBase64String(string path)
    {
        FileStream fsForRead = new(path, FileMode.Open); //文件路径
        var base64Str = "";
        try
        {
            fsForRead.Seek(0, SeekOrigin.Begin);
            var bs = new byte[fsForRead.Length];
            var log = Convert.ToInt32(fsForRead.Length);
            _ = fsForRead.Read(bs, 0, log);
            base64Str = Convert.ToBase64String(bs);
            return base64Str;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            return base64Str;
        }
        finally
        {
            fsForRead.Close();
        }
    }

    internal static string CompressBase64(string base64String)
    {
        var base64Bytes = Encoding.UTF8.GetBytes(base64String);
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(base64Bytes, 0, base64Bytes.Length);
            }

            return Convert.ToBase64String(outputStream.ToArray());
        }
    }
    internal static List<int> GetActiveBuffs(IDbConnection connection, int userId, string name)
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
            // ignored
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
            // ignored
        }

        return new List<int>();
    }
}