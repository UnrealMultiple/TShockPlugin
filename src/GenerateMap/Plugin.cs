using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace GenerateMap;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Description => GetString("生成地图图片");

    public override Version Version => new Version(1, 0, 5);

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        TShock.RestApi.Register(new SecureRestCommand("/generatemap", this.RestGenerateMap, "generate.map"));
        Commands.ChatCommands.Add(new("generate.map", this.CGenerate, "生成地图", "generatemap"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ((List<RestCommand>) typeof(Rest)
                .GetField("commands", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .GetValue(TShock.RestApi)!)
                .RemoveAll(x => x.Name == "/generatemap");
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CGenerate);
        }
        base.Dispose(disposing);
    }

    private object RestGenerateMap(RestRequestArgs args)
    {
        var bytes = CaiLib.CaiMap.CreateMapBytes();
        return new RestObject("200")
        {
            { "response", "生成地图成功" },
            { "base64", Convert.ToBase64String(bytes) }
        };
    }

    private void CGenerate(CommandArgs args)
    {
        Task.Run(() =>
        {
            var bytes = CaiLib.CaiMap.CreateMapBytes();
            var fileName = $"{Guid.NewGuid()}.png";
            File.WriteAllBytes(fileName, bytes);
            args.Player.SendSuccessMessage(GetString($"地图已生成保存在{fileName}文件"));
        });
    }
}