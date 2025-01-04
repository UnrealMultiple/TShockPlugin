namespace AutoPluginManager.Internal;

internal record PluginUpdateInfo(PluginVersionInfo? Current, PluginVersionInfo Latest)
{
    public class AssemblyNameEqualityComparer : IEqualityComparer<PluginUpdateInfo>
    {
        public bool Equals(PluginUpdateInfo? x, PluginUpdateInfo? y)
        {
            return ReferenceEquals(x, y) || (x is not null && y is not null && x.Latest.AssemblyName == y.Latest.AssemblyName);
        }

        public int GetHashCode(PluginUpdateInfo obj)
        {
            return obj.Latest.AssemblyName.GetHashCode();
        }
    }
}