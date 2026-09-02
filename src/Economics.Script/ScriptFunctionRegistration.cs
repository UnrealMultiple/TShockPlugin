namespace Economics.Script;

/// <summary>
/// 一个已注册的宿主函数：脚本中的名字 + 对应的 .NET 委托。
/// </summary>
public readonly record struct ScriptFunctionRegistration(string Name, Delegate Handler);
