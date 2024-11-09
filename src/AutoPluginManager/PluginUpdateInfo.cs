namespace AutoPluginManager;

public class PluginUpdateInfo
{
    public PluginVersionInfo? Current { get; }
    public PluginVersionInfo Latest { get; }

    public PluginUpdateInfo(PluginVersionInfo? current, PluginVersionInfo latest)
    {
        this.Current = current;
        this.Latest = latest;
    }
    
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
