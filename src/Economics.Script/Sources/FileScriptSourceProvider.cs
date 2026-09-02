namespace Economics.Script.Sources;

/// <summary>
/// 以文件系统为来源的默认 <see cref="IScriptSourceProvider"/>。
/// 以根目录为基准解析脚本文件名，并阻止路径穿越（<c>..</c>）。
/// </summary>
public sealed class FileScriptSourceProvider : IScriptSourceProvider
{
    private readonly string _rootFull;

    public FileScriptSourceProvider(string rootDirectory)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException("根目录不能为空。", nameof(rootDirectory));
        }

        this.Root = rootDirectory;
        this._rootFull = Path.GetFullPath(rootDirectory);
    }

    public string Root { get; }

    public bool TryResolve(string key, out string location)
    {
        location = string.Empty;
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        var candidate = Path.GetFullPath(Path.Combine(this._rootFull, key));
        if (!candidate.StartsWith(this._rootFull, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!File.Exists(candidate))
        {
            return false;
        }

        location = candidate;
        return true;
    }

    public bool Exists(string location)
    {
        return File.Exists(location);
    }

    public string Read(string location)
    {
        return File.ReadAllText(location);
    }

    public string GetVersion(string location)
    {
        var info = new FileInfo(location);
        return info.Exists
            ? ((info.LastWriteTimeUtc.Ticks * 31) + info.Length).ToString()
            : string.Empty;
    }
}
