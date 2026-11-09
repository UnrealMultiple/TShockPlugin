using System;
using System.Collections.Generic;

namespace NurseHeal;

public sealed class DeductResult
{
    public bool Success { get; private init; }

    public string? Reason { get; private init; }

    /// <summary>被扣款/找零改动过的物品槽位,由调用方发 PlayerSlot 同步。</summary>
    public IReadOnlyList<int> ChangedSlots { get; private init; } = Array.Empty<int>();

    public static DeductResult Ok(List<int> changedSlots)
    {
        return new DeductResult { Success = true, ChangedSlots = changedSlots };
    }

    public static DeductResult Fail(string reason)
    {
        return new DeductResult { Success = false, Reason = reason };
    }
}
