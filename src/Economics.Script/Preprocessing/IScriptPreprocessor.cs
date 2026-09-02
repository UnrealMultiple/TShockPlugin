namespace Economics.Script.Preprocessing;

/// <summary>
/// 编译前的脚本预处理器，例如把 <c>@require</c> / <c>@import</c> 这类 Jint 不认识的
/// 指令改写成注释，避免解析失败。可按需叠加多个预处理器。
/// </summary>
public interface IScriptPreprocessor
{
    /// <summary>把源码中需要处理的指令改写后返回。</summary>
    string Preprocess(string source);
}
