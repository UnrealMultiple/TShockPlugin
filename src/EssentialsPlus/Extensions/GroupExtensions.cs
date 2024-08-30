using System.Text.RegularExpressions;
using Group = TShockAPI.Group;

namespace EssentialsPlus.Extensions;

public static class GroupExtensions
{
    public static int GetDynamicPermission(this Group group, string root)
    {
        if (group.HasPermission(root + ".*"))
        {
            return int.MaxValue;
        }

        var max = 0;
        var regex = new Regex("^" + root.Replace(".", @"\.") + @"\.(\d+)$");
        foreach (var permission in group.TotalPermissions)
        {
            var match = regex.Match(permission);
            if (match.Success && match.Value == permission)
            {
                max = Math.Max(max, Convert.ToInt32(match.Groups[1].Value));
            }
        }

        return max == 0 && group.HasPermission(root) ? 1 : max;
    }
}