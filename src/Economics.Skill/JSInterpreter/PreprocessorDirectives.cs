using System.Text.RegularExpressions;

namespace Economics.Skill.JSInterpreter;

internal static partial class PreprocessorDirectives
{

    public static readonly Regex importRegex = ImportRegex();

    public static readonly Regex requiresRegex = RequireRegex();

    [GeneratedRegex("@import \"(.*?)\";")]
    public static partial Regex ImportRegex();

    [GeneratedRegex("@require(s?) (.*?);")]
    public static partial Regex RequireRegex();
}