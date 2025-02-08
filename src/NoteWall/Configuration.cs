using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace NoteWall;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "NoteWall";

    [LocalizedPropertyName(CultureType.Chinese, "个人最大留言数量")]
    [LocalizedPropertyName(CultureType.English, "MaxNotesperPlayer")]
    public int MaxNotesPerPlayer { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "留言字数限制")]
    [LocalizedPropertyName(CultureType.English, "MaxNoteLength")]
    public int MaxNoteLength { get; set; } = 50;

    [LocalizedPropertyName(CultureType.Chinese, "屏蔽词列表")]
    [LocalizedPropertyName(CultureType.English, "BannedWordsList")]
    public string BannedWords { get; set; } = "";

}