using Newtonsoft.Json;

namespace Lagrange.XocMat.Adapter.Setting.Configs;

public class SocketConfig
{
    [JsonProperty("套字节地址")]
    public string IP = "";

    [JsonProperty("服务器名称")]
    public string ServerName = "";

    [JsonProperty("端口")]
    public int Port = 6000;

    [JsonProperty("验证令牌")]
    public string Token { get; set; } = string.Empty;

    [JsonProperty("心跳包间隔")]
    public int HeartBeatTimer = 1 * 60 * 1000;

    [JsonProperty("重连间隔")]
    public int ReConnectTimer = 5 * 1000;

    [JsonProperty("空指令注册")]
    public HashSet<string> EmptyCommand = new() { "购买", "抽" };
}
