namespace Economics.Script;

/// <summary>
/// 标记一个静态方法为可暴露给脚本的宿主函数，<see cref="Name"/> 是在 JavaScript 中调用的名字。
/// 配合 <see cref="ScriptEngineOptions.RegisterFunctions{THost}"/> 使用。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ScriptFunctionAttribute(string name) : Attribute
{

    /// <summary>JavaScript 中使用的函数名。</summary>
    public string Name { get; } = name;
}
