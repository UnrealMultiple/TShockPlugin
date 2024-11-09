namespace AutoPluginManager;

public class PluginVersionInfo
{
    public Version Version { get; set; } = new ();

    public string Author { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string[] Dependencies { get; set; } = Array.Empty<string>();

    public string AssemblyName { get; set; } = string.Empty;

    public class AssemblyNameEqualityComparer : IEqualityComparer<PluginVersionInfo>
    {
        public bool Equals(PluginVersionInfo? x, PluginVersionInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.AssemblyName == y.AssemblyName;
        }

        public int GetHashCode(PluginVersionInfo obj)
        {
            return obj.AssemblyName.GetHashCode();
        }
    }
}