using System.Text.RegularExpressions;

namespace Economics.Skill.JSInterpreter;

internal static class PreprocessorDirectives
{

    public static readonly Regex importRegex = new("@import \"(.*?)\";");

    public static readonly Regex requiresRegex = new("@require(s?) (.*?);");
}