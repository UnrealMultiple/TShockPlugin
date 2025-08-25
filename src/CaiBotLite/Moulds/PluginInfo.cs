using System;

namespace CaiBotLite.Moulds;

internal class PluginInfo(string name, string description, string author, Version version)
{
    public string Author = author;
    public string Description = description;
    public string Name = name;
    public Version Version = version;
}