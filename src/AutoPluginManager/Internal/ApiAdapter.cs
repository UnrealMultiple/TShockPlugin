using System.ComponentModel;
using System.Web;

namespace AutoPluginManager.Internal;
internal class ApiAdapter
{
    private static readonly HttpClient _client = new();

    private const string RequestApi = "http://api.terraria.ink:11434";

    /// <summary>
    /// 构建请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task<byte[]> HttpGetByte(string url, Dictionary<string, string>? args = null)
    {
        var uriBuilder = new UriBuilder(url);
        var param = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (args != null)
        {
            foreach (var (key, val) in args)
            {
                param[key] = val;
            }
        }
        uriBuilder.Query = param.ToString();
        return await _client.GetByteArrayAsync(uriBuilder.ToString());
    }

    public static async Task<byte[]> Request(PluginAPIType type, Dictionary<string, string>? args = null)
    {
        var url = RequestApi + type.GetAttribute<DescriptionAttribute>().Description;
        return await HttpGetByte(url, args);
    }
}
