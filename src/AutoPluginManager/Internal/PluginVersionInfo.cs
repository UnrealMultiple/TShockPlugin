using Newtonsoft.Json;

namespace AutoPluginManager.Internal;

internal class PluginVersionInfo
{
    public Version Version { get; set; } = new();

    public string Author { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Dictionary<string, string> Description { get; set; } = new();

    [JsonProperty("Path")]
    public string FileName { get; set; } = string.Empty;

    public string[] Dependencies { get; set; } = Array.Empty<string>();

    public bool HotReload { get; set; } = true;

    public string AssemblyName { get; set; } = string.Empty;

    public class AssemblyNameEqualityComparer : IEqualityComparer<PluginVersionInfo>
    {
        public bool Equals(PluginVersionInfo? x, PluginVersionInfo? y)
        {
            return ReferenceEquals(x, y) || (x is not null && y is not null && x.AssemblyName == y.AssemblyName);
        }

        public int GetHashCode(PluginVersionInfo obj)
        {
            return obj.AssemblyName.GetHashCode();
        }
    }
}