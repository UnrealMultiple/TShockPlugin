namespace Economics.Script;

/// <summary>
/// 脚本执行模式。
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// 顶层代码只执行一次（定义入口函数与全局环境），之后每次调用只 <c>Invoke</c> 入口函数。
    /// 同一运行时内顶层全局变量会在多次调用之间保留。
    /// </summary>
    DefineOnce,

    /// <summary>
    /// 每次调用前恢复干净的全局快照，再重新执行顶层代码并调用入口函数。
    /// 顶层全局状态每次调用都会归零（代价是每次都重跑一层顶层代码，但不会重新解析）。
    /// </summary>
    SnapshotRestore,
}
