using System.Text;
using Terraria;
using Terraria.GameContent;
using TerrariaApi.Server;
using TShockAPI;

namespace DamageStatistic;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class DamageStatistic(Main game) : TerrariaPlugin(game)
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (1, 1, 0);
    public override string Author => "Megghy, Cai";
    public override string Description => GetString("在每次 Boss 战后显示每个玩家造成的伤害。");

    public override void Initialize()
    {
        On.Terraria.GameContent.BossDamageTracker.OnBossKilled += BossDamageTrackerOnOnBossKilled;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            On.Terraria.GameContent.BossDamageTracker.OnBossKilled -= BossDamageTrackerOnOnBossKilled;
        }

        base.Dispose(disposing);
    }

    private static void BossDamageTrackerOnOnBossKilled(On.Terraria.GameContent.BossDamageTracker.orig_OnBossKilled orig, BossDamageTracker self, NPC npc)
    {
        orig(self, npc);

        TShock.Players
            .Where(x => x is { Active: true })
            .ForEach(x =>
            {
                SendReport(x, self, npc);
            });
    }

    private static void SendReport(TSPlayer player, BossDamageTracker tracker, NPC npc)
    {
        var report = new StringBuilder();
        var playerCount = tracker._list.Count(x => x is NPCDamageTracker.PlayerCreditEntry);
        tracker._list.Sort();
        var damagePercentages = NPCDamageTracker.CalculatePercentages(tracker._list.Select(x => x.Damage).ToArray());
        var duration = TimeSpan.FromSeconds(tracker.Duration / 60.0);
        for (var i = 0; i < tracker._list.Count; i++)
        {
            var creditEntry = tracker._list[i];
            var isSelf = creditEntry is NPCDamageTracker.PlayerCreditEntry entry && entry.PlayerName == player.Name;
            var nameDisplay = isSelf ? $"[c/FFAF00:{creditEntry.Name}]" : creditEntry.Name.ToString();
            report.AppendLine($"{nameDisplay}: [c/74F3C9:{creditEntry.Damage}] <{damagePercentages[i]}%>");
        }

        player.SendInfoMessage(
            GetString($"[c/74F3C9:{playerCount}]位玩家耗时[c/74F3C9:{(int) duration.TotalMinutes}分{duration.Seconds:00}秒]击败了[c/74F3C9:{tracker.Name.ToString()}]\n") +
            $"{report.ToString().TrimEnd()}"
        );
    }
}