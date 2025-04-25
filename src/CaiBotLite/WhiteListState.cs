namespace CaiBotLite;

public enum WhiteListStatus
{
    /// <summary>
    /// 验证通过
    /// </summary>
    Success = 200,
    
    /// <summary>
    /// 用户未加入群组
    /// </summary>
    NotInGroup = 401,
    
    /// <summary>
    /// 用户被封禁
    /// </summary>
    Banned = 403,
    
    /// <summary>
    /// 不在白名单中
    /// </summary>
    NotInWhiteList = 404,
    
    /// <summary>
    /// 用户未登录
    /// </summary>
    Unauthorized = 405,
}