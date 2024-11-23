namespace AutoPluginManager;

public record PluginUpdateInfo(PluginVersionInfo Current, PluginVersionInfo Latest)
{
    public class AssemblyNameEqualityComparer : IEqualityComparer<PluginUpdateInfo>
    {
        public bool Equals(PluginUpdateInfo? x, PluginUpdateInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Latest.AssemblyName == y.Latest.AssemblyName;
        }

        public int GetHashCode(PluginUpdateInfo obj)
        {
            return obj.Latest.AssemblyName.GetHashCode();
        }
    }
}