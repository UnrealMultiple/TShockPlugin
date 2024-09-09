namespace CaiBot;

public class PluginInfo
{
    public PluginInfo(string name,string description,string author, Version version)
    {
        this.Name = name;
        this.Description = description;
        this.Author = author;
        this.Version = version;
    }
    
    public string Name;
    public string Description;
    public string Author;
    public Version Version;
}