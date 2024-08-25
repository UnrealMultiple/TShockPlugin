namespace AutoPluginManager;

public class PluginVersionInfo
{
    public Version Version { get; set; } = new ();

    public string Author { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string AssemblyName { get; set; } = string.Empty;
}
