using System.Diagnostics;
using Terraria;
using TShockAPI;

namespace Economics.Core.Services;

public class PingData
{
    public TSPlayer? Player { get; set; }
    public int ItemIndex { get; set; }
    public Stopwatch Stopwatch { get; } = new Stopwatch();
    public double CurrentPing { get; set; } = -1;
}

public class PingService
{
    private static readonly Dictionary<TSPlayer, PingData> _records = new();

    public static void SendPing(TSPlayer player)
    {
        if (player == null || !player.Active)
        {
            return;
        }

        var slot = -1;
        for (var i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i] != null && (!Main.item[i].active || Main.item[i].playerIndexTheItemIsReservedFor == player.Index))
            {
                slot = i;
                Main.item[i].playerIndexTheItemIsReservedFor = player.Index;
                break;
            }
        }
        if (slot == -1)
        {
            return;
        }

        if (!_records.TryGetValue(player, out var record))
        {
            record = new PingData { Player = player };
            _records[player] = record;
        }

        record.ItemIndex = slot;
        record.Stopwatch.Restart();

        NetMessage.TrySendData(22, player.Index, -1, null, slot);
        NetMessage.TrySendData(39, player.Index, -1, null, slot);
    }

    public static void OnResponseReceived(TSPlayer player, int slot)
    {
        if (_records.TryGetValue(player, out var record) && record.ItemIndex == slot)
        {
            record.Stopwatch.Stop();
            record.CurrentPing = record.Stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    public static PingData? GetData(TSPlayer player)
    {
        _records.TryGetValue(player, out var data);
        return data;
    }

    public static void RemovePlayer(TSPlayer player)
    {
        _records.Remove(player);
    }
}
