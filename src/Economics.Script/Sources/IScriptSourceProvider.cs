namespace Economics.Script.Sources;

/// <summary>
/// 脚本源码的来源抽象。默认实现是文件系统（<see cref="FileScriptSourceProvider"/>），
/// 你也可以实现它来从资源、数据库或远程地址读取脚本。
/// </summary>
public interface IScriptSourceProvider
{
    /// <summary>
    /// 把一个逻辑 key（例如脚本文件名）解析为一个稳定的 location（例如绝对路径）。
    /// 解析失败（不存在、路径越界等）返回 <c>false</c>。
    /// </summary>
    bool TryResolve(string key, out string location);

    /// <summary>location 是否仍存在。</summary>
    bool Exists(string location);

    /// <summary>读取 location 的源码文本。</summary>
    string Read(string location);

    /// <summary>
    /// 返回一个用于变更检测的版本标记（例如文件时间戳+长度）。返回的字符串
    /// 变化说明脚本变了，需要进行重载/重新编译。无法检测时返回 <c>string.Empty</c>。
    /// </summary>
    string GetVersion(string location);
}
