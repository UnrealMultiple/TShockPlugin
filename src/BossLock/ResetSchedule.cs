using System.Globalization;

namespace BossLock;

internal static class ResetSchedule
{
    // 输入和返回值均为北京时间；写入数据库前转换为服务器本地时间。
    internal static DateTime Next(DateTime now, string[]? times)
    {
        if (times == null || times.Length == 0)
        {
            throw new FormatException("AutoResetTimes must not be empty.");
        }

        var next = DateTime.MaxValue;
        foreach (var text in times)
        {
            if (!TimeOnly.TryParseExact(text, "HH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var time))
            {
                throw new FormatException("AutoResetTimes requires HH:mm.");
            }

            var candidate = now.Date.Add(time.ToTimeSpan());
            if (candidate <= now)
            {
                candidate = candidate.AddDays(1);
            }

            if (candidate < next)
            {
                next = candidate;
            }
        }

        return next;
    }
}
