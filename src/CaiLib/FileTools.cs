namespace CaiLib;

public class CaiFileTools
{
    /// <summary>
    /// 文件转base64
    /// </summary>
    /// <returns>base64字符串</returns>
    public static string FileToBase64String(string path)
    {
        var fsForRead = new FileStream(path, FileMode.Open);//文件路径
        var base64Str = "";
        try
        {
            fsForRead.Seek(0, SeekOrigin.Begin);
            var bs = new byte[fsForRead.Length];
            var log = Convert.ToInt32(fsForRead.Length);
            fsForRead.ReadExactly(bs, 0, log);
            base64Str = Convert.ToBase64String(bs);
            return base64Str;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            Console.ReadLine();
            return base64Str;
        }
        finally
        {
            fsForRead.Close();
        }
    }

    /// <summary>
    /// base64字符串转文件
    /// </summary>
    /// <param name="base64String">Base64字符串</param>
    /// <param name="fileFullPath">文件路径</param>
    /// <returns></returns>

    public static bool Base64StringToFile(string base64String, string fileFullPath)
    {
        var opResult = false;
        var strbase64 = base64String.Trim()[(base64String.IndexOf(",") + 1)..];   //将‘，’以前的多余字符串删除
        var stream = new MemoryStream(Convert.FromBase64String(strbase64));
        var fs = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Write);
        var b = stream.ToArray();
        fs.Write(b, 0, b.Length);
        fs.Close();

        opResult = true;
        return opResult;
    }
}