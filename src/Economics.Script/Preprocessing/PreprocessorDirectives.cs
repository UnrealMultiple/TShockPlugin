using System.Text.RegularExpressions;

namespace Economics.Script.Preprocessing;

internal static partial class PreprocessorDirectives
{
    public static readonly Regex RequireRegex = RequirePattern();

    [GeneratedRegex("@require(s?) (.*?);")]
    private static partial Regex RequirePattern();
}
