using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using TerrariaApi.Server;

namespace CaiBotLite.Services;

public static class BossLockSupport
{
    public static bool Support { get; private set; }

    public static void Init()
    {
        var pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "BossLock");
        if (pluginContainer is not null)
        {
            Support = true;
        }
        
    }

    public static Dictionary<string, string> GetLockBosses()
    {
        return new Dictionary<string, string>();
        //     if (!Support)
        //     {
        //         throw new NotSupportedException("没有安装BossLock插件!");
        //     }        
        //     
        //     //var lockedBosses = BossLock.Database.GetAllLocked(); // 原始数据: Dictionary<int, string>
        //     var result = new Dictionary<string, string>();
        //     
        //     var bossIdToName = new Dictionary<int, string>
        //     {
        //         { NPCID.KingSlime, "King Slime" },
        //         { NPCID.EyeofCthulhu, "Eye of Cthulhu" },
        //         { NPCID.EaterofWorldsHead, "Eater of Worlds" },
        //         { NPCID.BrainofCthulhu, "Brain of Cthulhu" },
        //         { NPCID.QueenBee, "Queen Bee" },
        //         { NPCID.Deerclops, "Deerclops" },
        //         { NPCID.SkeletronHand, "Skeletron" },
        //         { NPCID.WallofFlesh, "Wall of Flesh" },
        //         { NPCID.QueenSlimeBoss, "Queen Slime" },
        //         { NPCID.Retinazer, "The Twins" },
        //         { NPCID.Spazmatism, "The Twins" },
        //         { NPCID.TheDestroyer, "The Destroyer" },
        //         { NPCID.SkeletronPrime, "Skeletron Prime" },
        //         { NPCID.Plantera, "Plantera" },
        //         { NPCID.Golem, "Golem" },
        //         { NPCID.DukeFishron, "Duke Fishron" },
        //         { NPCID.HallowBoss, "Empress of Light" },
        //         { NPCID.CultistBoss, "Lunatic Cultist" },
        //         { NPCID.MoonLordCore, "Moon Lord" }
        //     };
        //
        //     foreach (var lockedBoss in lockedBosses)
        //     {
        //         if (!bossIdToName.TryGetValue(lockedBoss.Key, out var bossName))
        //         {
        //             continue;
        //         }
        //         
        //         if (lockedBoss.Key == NPCID.Retinazer || lockedBoss.Key == NPCID.Spazmatism)
        //         {
        //             if (!result.ContainsKey(bossName))
        //             {
        //                 result[bossName] = lockedBoss.Value;
        //             }
        //         }
        //         else
        //         {
        //             result[bossName] = lockedBoss.Value;
        //         }
        //     }
        //
        //     return result;
        // }
    }
}