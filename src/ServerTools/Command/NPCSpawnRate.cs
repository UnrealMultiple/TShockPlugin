using LazyAPI.Attributes;
using Terraria;
using TShockAPI;

namespace ServerTools.Command;

[Command("birthrate", "br")]
[Permissions("server.tool.npcspawn")]
public class NPCSpawnRate
{
    [Alias("enable", "on")]
    [RealPlayer]
    public static void Enable(CommandArgs args)
    {
        var rate = GetPlayerSpawnRate(args.Player.TPlayer);
        rate.Enable = true;
        args.Player.SendInfoMessage(GetString($"开启怪物生成率，当前生成率{{spawnRate: {rate.spawnRate}, maxSpawn: {rate.maxSpawns}}}"));
    }

    [Alias("off")]
    [RealPlayer]
    public static void Off(CommandArgs args)
    {
        if(Plugin.PlayerSpawnRates.TryGetValue(args.Player.TPlayer, out var rate))
        {
            rate.Enable = false;
        }
        args.Player.SendInfoMessage(GetString("已关闭怪物生成率"));
    }

    [Alias("rate")]
    [RealPlayer]
    public static void ModifySpawnRate(CommandArgs args, int num)
    {
        var rate = GetPlayerSpawnRate(args.Player.TPlayer);
        rate.spawnRate = num;
        args.Player.SendInfoMessage(GetString($"设置生成速率{{spawnRate: {rate.spawnRate}, maxSpawn: {rate.maxSpawns}}}"));
    }

    [Alias("max")]
    [RealPlayer]
    public static void ModifySpawnMax(CommandArgs args, int max)
    {
        var rate = GetPlayerSpawnRate(args.Player.TPlayer);
        rate.maxSpawns = max;
        args.Player.SendInfoMessage(GetString($"设置最大生成数量{{spawnRate: {rate.spawnRate}, maxSpawn: {rate.maxSpawns}}}"));
    }

    [Alias("help")]
    public static void Help(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("/spawnrate on 启用生成率修改"));
        args.Player.SendInfoMessage(GetString("/spawnrate off 关闭生成率修改"));
        args.Player.SendInfoMessage(GetString("/spawnrate rate [生成率] 设置NPC生成速率（越小越快）"));
        args.Player.SendInfoMessage(GetString("/spawnrate max [数量] 设置每次生成最大可生成数量"));
    }

    private static Plugin.PlayerSpawnRate GetPlayerSpawnRate(Player player)
    {
        if(Plugin.PlayerSpawnRates.TryGetValue(player, out var item))
        {
            return item;
        }
        var rate = new Plugin.PlayerSpawnRate();
        Plugin.PlayerSpawnRates[player] = rate;
        return rate;
    }
}
