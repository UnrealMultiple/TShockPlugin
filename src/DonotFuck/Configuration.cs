using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using TShockAPI;

namespace DonotFuck;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    #region 实例变量
    [LocalizedPropertyName(CultureType.Chinese, "每页行数")]
    [LocalizedPropertyName(CultureType.English, "pageLine")]
    public int PageSize = 30;

    [LocalizedPropertyName(CultureType.Chinese, "启用日志")]
    [LocalizedPropertyName(CultureType.English, "enableLog")]
    public bool EnableLog = true;

    [LocalizedPropertyName(CultureType.Chinese, "脏话表")]
    [LocalizedPropertyName(CultureType.English, "dirtyWords")]
    public HashSet<string> DirtyWords { get; set; } = new HashSet<string>();
    #endregion

    public const string _Directory = "DonotFuck";

    private StreamWriter? writer;

    internal StreamWriter Logger => this.writer ??= new StreamWriter(this.logFilePath);

    internal string logFilePath => Path.Combine(TShock.SavePath, _Directory, $"DonotFuck-{DateTime.Now:yyyy-MM-dd}.log");

    protected override string Filename => Path.Combine(_Directory, "Config");

    #region 预设参数方法
    protected override void SetDefault()
    {
        this.DirtyWords = new HashSet<string> { "6", "六" };
    }
    #endregion

    #region 增删改方法
    internal bool Exempt(string text)
    {
        return this.DirtyWords.Contains(text);
    }

    public void Log(string text)
    {
        if (this.EnableLog)
        {
            this.Logger.WriteLine(text);
            this.Logger.Flush();
        }
    }

    public void DisposeLog()
    {
        this.writer?.Close();
        this.writer?.Dispose();
        this.writer = null;
    }

    public bool Add(string text)
    {
        if (this.Exempt(text))
        {
            return false;
        }
        this.DirtyWords.Add(text);
        this.SaveTo();
        return true;
    }

    public bool Del(string text)
    {
        if (this.Exempt(text))
        {
            this.DirtyWords.Remove(text);
            this.SaveTo();
            return true;
        }
        return false;
    }
    #endregion
}