using LazyAPI.Attributes;
using Rests;
using ServerTools.DB;

namespace ServerTools;

[Rest("rank")]
public class Rest
{
    [RestPath("onlie")]
    public static object Online(RestRequestArgs args)
    {
        var data = PlayerOnline.GetOnlineRank().Select(x => new
        {
            name = x.Name,
            duration = x.Duration
        });
        return new RestObject("200")
        {
            ["response"] = "获取成功",
            ["data"] = data
        };
    }

    [RestPath("dead")]
    public static object Dead(RestRequestArgs args)
    {
        var data = DB.PlayerDeath.GetDeathRank()
            .Select(x => new { x.Name, x.Count })
            .ToList();
        return new RestObject("200")
        {
            ["response"] = "获取成功",
            ["data"] = data
        };
    }
}
