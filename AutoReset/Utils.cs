using System.Net;
using Terraria;
using TShockAPI;

namespace AutoReset.MainPlugin;

internal class Utils
{
    public static void CallApi()
    {
        if (AutoResetPlugin.Config.ResetCaution)
            try
            {
                HttpClient client = new();
                HttpResponseMessage? response;
                client.Timeout = TimeSpan.FromSeconds(5.0);
                response = client.GetAsync($"http://api.terraria.ink:22334/bot/send_reset?" +
                                           $"token={AutoResetPlugin.Config.CaiBotToken}" +
                                           $"&server_name={Main.worldName}" +
                                           $"&seed={(Main.ActiveWorldFileData.SeedText == "" ? Main.ActiveWorldFileData.Seed : Main.ActiveWorldFileData.SeedText)}")
                    .Result;
                if (response.StatusCode != HttpStatusCode.OK)
                    TShock.Log.ConsoleWarn($"[自动重置]调用API失败! (状态码: {(int)response.StatusCode})");
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
    }
}
