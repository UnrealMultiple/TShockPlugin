using System.Data;
using System.IO.Compression;
using System.Text;

namespace CaiBot;

internal static class Utils
{
    
    internal static string FileToBase64String(string path)
    {
        FileStream fsForRead = new (path, FileMode.Open); //文件路径
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