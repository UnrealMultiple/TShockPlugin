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
    public static async Task<byte[]> Request(PluginAPIType type, Dictionary<string, string>? args = null)
    {
        var defaultArgs = new Dictionary<string, string> { { "tshock_version", TShockAPI.TShock.VersionNum.ToString() } };
        var uriBuilder = new UriBuilder(RequestApi)
        {
            Path = type.GetAttribute<DescriptionAttribute>().Description
        };
        var param = HttpUtility.ParseQueryString(uriBuilder.Query);
        var requestArgs = args is null ? defaultArgs : args.Concat(defaultArgs);
        foreach (var (key, val) in requestArgs)
        {
            param[key] = val;
        }
        uriBuilder.Query = param.ToString();
        return await _client.GetByteArrayAsync(uriBuilder.ToString());
    }
}
