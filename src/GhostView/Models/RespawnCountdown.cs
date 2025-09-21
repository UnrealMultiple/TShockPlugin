using GhostView.Strategies;
using System.Collections;
using System.Collections.Concurrent;
using Terraria;

namespace GhostView.Models;

public class RespawnCountdown : IDisposable
{
    private readonly RespawnTimeSelector _selector = new ();

    private readonly ConcurrentDictionary<string, (int totalSeconds, DateTime startedAt, bool finished)>
        _timers = new ();

    private bool _disposed;

    public double StartCountdown(string playerName, bool isBossAlive, Action<string> onFinished)
    {
        if (this._disposed)
        {
            return 0;
        }

        if (this._timers.TryGetValue(playerName, out var existing) && !existing.finished)
        {
            return this.GetRemainingSeconds(playerName);
        }

        var totalSeconds = RespawnTimeSelector.GetRespawnSeconds(isBossAlive);
        this._timers[playerName] = (totalSeconds, DateTime.UtcNow, false);
        Main.DelayedProcesses.Add(this.GetDelayedTimer(totalSeconds, playerName,onFinished));
        return totalSeconds;
    }

    private IEnumerator GetDelayedTimer(int totalSeconds, string playerName, Action<string> onFinished)
    {
        for (var i = 0; i < 60 * totalSeconds; i++)
        {
            yield return null;
        }
        this._timers.Remove(playerName, out _);
        onFinished.Invoke(playerName);
    }  

    public double GetRemainingSeconds(string playerName)
    {
        if (this._disposed || !this._timers.TryGetValue(playerName, out var info))
        {
            return 0;
        }

        if (info.finished)
        {
            return 0;
        }

        var elapsed = (DateTime.UtcNow - info.startedAt).TotalSeconds;
        return Math.Max(0, info.totalSeconds - elapsed);
    }

    public bool IsFinished(string playerName)
    {
        if (this._disposed)
        {
            return true;
        }

        if (this._timers.TryGetValue(playerName, out var info))
        {
            return info.finished || this.GetRemainingSeconds(playerName) <= 0;
        }

        return true;
    }

    public void Dispose()
    {
        this._disposed = true;
        this._timers.Clear();
    }
}