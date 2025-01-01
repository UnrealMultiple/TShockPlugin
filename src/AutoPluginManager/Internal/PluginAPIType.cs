using System.ComponentModel;

namespace AutoPluginManager.Internal;
internal enum PluginAPIType
{
    [Description("/plugin/get_plugin_list")]
    AllManifests,

    [Description("/plugin/get_plugin_manifest/")]
    Manifest,

    [Description("/plugin/get_plugin_zip")]
    Alone,

    [Description("/plugin/get_all_plugins")]
    All
}

internal static class EnumExtensions
{
    public static T GetAttribute<T>(this PluginAPIType value) where T : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value)!;
        return type.GetField(name)
                   ?.GetCustomAttributes(false)
                   .OfType<T>()
                   .FirstOrDefault() ?? throw new NullReferenceException("This is a bad API interface on my part!");
    }
}
