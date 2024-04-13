using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Noagent;

[ApiVersion(2, 1)]
public class Noagent : TerrariaPlugin
{
    public override string Author => "[星迹]Jonesn，肝帝熙恩更新适配1449";
    public override string Description => "禁止代理登录";
    public override string Name => "[禁止代理登录]Noagent";
    public override Version Version => new Version(1, 0, 1);

    public Noagent(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
        }
        base.Dispose(disposing);
    }

    public class Message
    {
        public TSPlayer Player { get; set; }
        public TaskCompletionSource<string> Result { get; set; }

        public async void ThreadMain()
        {
            try
            {
                await Task.Delay(3000);
                string ipAddress = Player.IP;
                string apiUrl = $"https://blackbox.ipinfo.app/lookup/{ipAddress}";

                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                Result.SetResult(responseContent);
            }
            catch (Exception ex)
            {
                Result.SetException(ex);
            }
        }
    }

    private async void OnJoin(JoinEventArgs args)
    {
        TSPlayer player = TShock.Players[args.Who];

        // 忽略内网IP和回环地址
        if (IsPrivateOrLoopbackAddress(player.IP))
        {
            Console.WriteLine($"{player.Name} 使用内网或回环地址，跳过代理检测");
            return;
        }
        Message message = new Message
        {
            Player = player,
            Result = new TaskCompletionSource<string>()
        };

        Thread thread = new Thread(message.ThreadMain);
        thread.Start();

        try
        {
            string result = await message.Result.Task;
            if (result == "Y") // 假设新的IP查询服务返回 "Y" 表示代理IP
            {
                Console.WriteLine($"检测到{player.Name}为代理，已踢出");
                player.Disconnect("本服务禁止代理IP登录");
            }
            else
            {
                Console.WriteLine($"{player.Name} IP无异常");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"检测玩家{player.Name} IP时发生错误: {ex.Message}");
        }
    }

    private bool IsPrivateOrLoopbackAddress(string ipAddress)
    {
        IPAddress ip;
        if (!IPAddress.TryParse(ipAddress, out ip))
        {
            return false;
        }

        // 判断是否为内网IP
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            byte[] bytes = ip.GetAddressBytes();
            if ((bytes[0] == 10) || // 10.0.0.0/8
                (bytes[0] == 172 && (bytes[1] >= 16 && bytes[1] <= 31)) || // 172.16.0.0/12
                (bytes[0] == 192 && bytes[1] == 168)) // 192.168.0.0/16
            {
                return true;
            }
        }

        // 判断是否为回环地址
        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        return false;
    }

}