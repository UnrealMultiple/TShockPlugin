using StatusTextManager.Utils;
using System.Text;
using Terraria;
using TShockAPI;

namespace StatusTextManager;

public interface IStatusTextUpdateHandler
{
    bool Invoke(TSPlayer player, bool forceUpdate = false);
    string GetPlayerStatusText(TSPlayer player);
}

public delegate void StatusTextUpdateDelegate(StatusTextUpdateEventArgs args);

public class StatusTextUpdateEventArgs
{
    public required TSPlayer TSPlayer { get; set; }
    public required StringBuilder StatusTextBuilder { get; set; }
}

public class StatusTextUpdateHandlerItem : IStatusTextUpdateHandler
{
    public string AssemblyName;

    private readonly StringBuilder?[] _playerStringBuilders = new StringBuilder[Main.maxPlayers];
    public StatusTextUpdateDelegate UpdateDelegate;
    public ulong UpdateInterval = 60;

    public StatusTextUpdateHandlerItem(StatusTextUpdateDelegate updateDelegate, ulong updateInterval = 60)
    {
        this.UpdateDelegate = updateDelegate ?? throw new ArgumentNullException(nameof(updateDelegate));
        this.UpdateInterval = updateInterval > 0 ? updateInterval : throw new ArgumentException("cannot be 0", nameof(updateInterval));
        this.AssemblyName = updateDelegate.Method.DeclaringType?.Assembly.GetName().Name ?? "";
    }

    public bool Invoke(TSPlayer player, bool forceUpdate = false)
    {
        try
        {
            // 检查对应玩家是否需要更新 Status Text
            if (forceUpdate || (Common.TickCount + (ulong) player.Index) % this.UpdateInterval == 0)
            {
                var updateDelegate = this.UpdateDelegate;
                var args = new StatusTextUpdateEventArgs { TSPlayer = player, StatusTextBuilder = this._playerStringBuilders.AcquirePlayerStringBuilder(player) };
                updateDelegate(args);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"'{this.AssemblyName}' has thrown an exception in StatusTextUpdateHandler.Invoke:\n{ex}");
        }

        return false;
    }

    // 获取当前 Handler 对对应玩家的 Status Text
    public string GetPlayerStatusText(TSPlayer player)
    {
        return this._playerStringBuilders[player.Index]?.ToString() ?? "";
    }
}