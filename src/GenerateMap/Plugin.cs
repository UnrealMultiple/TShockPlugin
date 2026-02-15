using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace GenerateMap;

[ApiVersion(2, 1)]
public class Plugin(Main game) : TerrariaPlugin(game)
{
    public override string Author => "少司命, Cai";

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Description => GetString("生成地图图片");

    public override Version Version => new (2, 0, 0);

    public override void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
        MapGenerator.Init();
        TShock.RestApi.Register(new SecureRestCommand("/generatemap/img", RestGenerateMapImg, "generatemap"));
        TShock.RestApi.Register(new SecureRestCommand("/generatemap/file", RestGenerateMapFile, "generatemap"));
        TShock.RestApi.RegisterRedirect("/generatemap", "/generatemap/img");
        Commands.ChatCommands.Add(new Command("generatemap", Generate, "map", "生成地图", "generatemap"));
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            MapGenerator.Dispose();
            ((List<RestCommand>) typeof(Rest)
                .GetField("commands", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(TShock.RestApi)!)
                .RemoveAll(x => x.UriTemplate.Contains("generatemap"));
            
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Generate);
            AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
        }
        base.Dispose(disposing);
    }
    
    private static RestObject RestGenerateMapFile(RestRequestArgs args)
    {
        return new RestObject("200")
        {
            { "response", GetString("生成地图文件成功") },
            { "base64", Convert.ToBase64String(MapGenerator.CreatMapFileBytes()) }
        };
    }

    private static RestObject RestGenerateMapImg(RestRequestArgs args)
    {
        return new RestObject("200")
        {
            { "response", GetString("生成地图图片成功") },
            { "base64", Convert.ToBase64String(MapGenerator.CreatMapImgBytes()) }
        };
    }

    private static void Generate(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            ShowHelp();
            return;
        }
        
        switch (args.Parameters[0])
        {
            case "img":
                try
                {
                    var fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                    var path = MapGenerator.SaveMapImg(fileName);
                    args.Player.SendSuccessMessage(GetString($"[GenerateMap]地图图片已保存到: {path}"));
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError( GetString("[GenerateMap]生成地图出错: ") + ex);
                }
                break;
            case "file":
                try
                {
                    var path = MapGenerator.SaveMapFile();
                    args.Player.SendSuccessMessage(GetString($"[GenerateMap]地图文件已保存到: {path}"));
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError( GetString("[GenerateMap]生成地图出错: ") + ex);
                }
                break;
            default:
                ShowHelp();
                break;
        }

        return;


        void ShowHelp()
        {
            args.Player.SendSuccessMessage(GetString("GenerateMap帮助: "));
            args.Player.SendSuccessMessage(GetString("/map file --- 导出世界文件(.wld)"));
            args.Player.SendSuccessMessage(GetString("/map img --- 生成地图图片"));
        }
    }
    
    #region 加载前置

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var resourceName =
            $"embedded.{new AssemblyName(args.Name).Name}.dll";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            return null;
        }

        var assemblyData = new byte[stream.Length];
        _ = stream.Read(assemblyData, 0, assemblyData.Length);
        return Assembly.Load(assemblyData);
    }

    #endregion
}
