namespace Economics.Script.Preprocessing;

/// <summary>
/// 把 <c>@require "模块";</c> / <c>@requires "模块";</c> 指令改写成注释（Jint 不识别它们）。
/// 通过 <see cref="Instance"/> 使用。
/// </summary>
public sealed class RequireDirectivePreprocessor : IScriptPreprocessor
{
    public static readonly RequireDirectivePreprocessor Instance = new();

    private RequireDirectivePreprocessor()
    {
    }

    public string Preprocess(string source)
    {
        return string.IsNullOrEmpty(source) || !PreprocessorDirectives.RequireRegex.IsMatch(source)
            ? source
            : PreprocessorDirectives.RequireRegex.Replace(
            source,
            static match => $"// [Economics.Script] @require directive disabled: {match.Value}\r\n");
    }
}
