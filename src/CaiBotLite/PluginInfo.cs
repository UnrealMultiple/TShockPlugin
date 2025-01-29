namespace CaiBotLite;

internal class PluginInfo
{
    public string Author;
    public string Description;

    public string Name;
    public Version Version;

    public PluginInfo(string name, string description, string author, Version version)
    {
        this.Name = name;
        this.Description = description;
        this.Author = author;
        this.Version = version;
    }
}