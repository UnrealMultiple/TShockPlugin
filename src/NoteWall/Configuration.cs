using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace NoteWall;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "NoteWall";

    [LocalizedPropertyName(CultureType.Chinese, "个人最大留言数量")]
    [LocalizedPropertyName(CultureType.English, "Max Notes per Player")]
    public int MaxNotesPerPlayer { get; set; } = 5;

}