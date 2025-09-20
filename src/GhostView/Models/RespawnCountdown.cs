using GhostView.Strategies;
using System.Collections.Concurrent;

namespace GhostView.Models;

public class RespawnCountdown : IDisposable
{
    private readonly RespawnTimeSelector _selector = new ();

    private readonly ConcurrentDictionary<string, (double totalSeconds, DateTime startedAt, bool finished)>
        _timers = new ();

    private bool _disposed;

    public double StartCountdown(string playerName, bool isBossAlive, Action<string>? onFinished)
    {
        if (this._disposed)
        {
            return 0;
        }

        if (this._timers.TryGetValue(playerName, out var existing) && !existing.finished)
        {
            return this.GetRemainingSeconds(playerName);
        }

        var totalSeconds = this._selector.GetRespawnSeconds(isBossAlive);
        this._timers[playerName] = (totalSeconds, DateTime.UtcNow, false);

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(totalSeconds));
                if (!this._disposed && this._timers.TryGetValue(playerName, out var info) && !info.finished)
                {
                    this._timers[playerName] = (info.totalSeconds, info.startedAt, true);
                    onFinished?.Invoke(playerName);
                }
            }
            finally
            {
                this._timers.TryRemove(playerName, out _);
            }
        });

        return totalSeconds;
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