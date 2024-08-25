namespace AutoPluginManager;

public class PluginUpdateInfo
{
    public PluginUpdateInfo(string name, string author, Version newVersion, Version oldVersion, string localPath, string remotePath)
    {
        NewVersion = newVersion;
        OldVersion = oldVersion;
        Author = author;
        Name = name;
        LocalPath = localPath;
        RemotePath = remotePath;
    }
    public Version NewVersion { get; set; }
    public Version OldVersion { get; set; }
    public string Author { get; set; }
    public string Name { get; set; }
    public string LocalPath { get; set; }
    public string RemotePath { get; set; }

}
