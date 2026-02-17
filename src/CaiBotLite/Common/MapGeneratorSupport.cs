using TerrariaApi.Server;

namespace CaiBotLite.Common;

internal static class MapGeneratorSupport
{
    private static bool Support { get; set; }
    internal static void Init()
    {
        var pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "GenerateMap");
        if (pluginContainer is not null)
        {
            Support = true;
        }
    }
    
    private static void ThrowIfNotSupported()
    {
        if (!Support)
        {
            throw new NotSupportedException("需要安装GenerateMap插件");
        }
    }
    

    internal static byte[] CreatMapImgBytes()
    {
        ThrowIfNotSupported();
        return GenerateMap.MapGenerator.CreatMapImgBytes();
    }

    internal static (byte[], string) CreateMapFile()
    {
        ThrowIfNotSupported();
        var mapFile = GenerateMap.MapGenerator.CreatMapFile();
        return (mapFile.File, mapFile.Name);
    }
}