using System.Data;
using System.IO.Compression;
using System.Text;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace CaiBot;

internal static class Utils
{

    
    internal static string GetWorldIconName()
    {
        var data = Main.ActiveWorldFileData;
        
        static string GetSeedIcon(string seed)
        {
            return "Icon" + (Main.ActiveWorldFileData.IsHardMode ? "Hallow" : "") + (Main.ActiveWorldFileData.HasCorruption ? "Corruption" : "Crimson") + seed;
        }
        
        if (data.ZenithWorld)
        {
            return "Icon" + (data.IsHardMode ? "Hallow" : "") + "Everything";
        }
        if (data.DrunkWorld)
        {
            return "Icon" + (data.IsHardMode ? "Hallow" : "") + "CorruptionCrimson";
        }
        if (data.ForTheWorthy)
        {
            return GetSeedIcon("FTW");
        }
        if (data.NotTheBees)
        {
            return GetSeedIcon("NotTheBees");
        }
        if (data.Anniversary)
        {
            return GetSeedIcon("Anniversary");
        }
        if (data.DontStarve)
        {
            return GetSeedIcon("DontStarve");
        }
        if (data.RemixWorld)
        {
            return GetSeedIcon("Remix");
        }
        if (data.NoTrapsWorld)
        {
            return GetSeedIcon("Traps");
        }
        return "Icon" + (data.IsHardMode ? "Hallow" : "") + (data.HasCorruption ? "Corruption" : "Crimson");
    }
    
    internal static Dictionary<string,bool> GetProcessList()
    {
        
        // ReSharper disable once UseObjectOrCollectionInitializer
        Dictionary<string, bool> processList = new();
        processList.Add("King Slime", NPC.downedSlimeKing);
        processList.Add("Eye of Cthulhu", NPC.downedBoss1);
        processList.Add("Eater of Worlds or Brain of Cthulhu", NPC.downedBoss2);
        processList.Add("Eater of Worlds", Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCID.EaterofWorldsHead])>0);
        processList.Add("Brain of Cthulhu", Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCID.BrainofCthulhu])>0);
        processList.Add("Queen Bee", NPC.downedQueenBee);
        processList.Add("Deerclops", NPC.downedDeerclops);
        processList.Add("Skeletron", NPC.downedBoss3);
        processList.Add("Wall of Flesh", Main.hardMode);
        processList.Add("Queen Slime", NPC.downedQueenSlime);
        processList.Add("The Destroyer", NPC.downedMechBoss1);
        processList.Add("The Twins", NPC.downedMechBoss2);
        processList.Add("Skeletron Prime", NPC.downedMechBoss3);
        processList.Add("Plantera", NPC.downedPlantBoss);
        processList.Add("Golem", NPC.downedGolemBoss);
        processList.Add("Duke Fishron", NPC.downedFishron);
        processList.Add("Empress of Light", NPC.downedEmpressOfLight);
        processList.Add("Lunatic Cultist", NPC.downedAncientCultist);
        
        processList.Add("Tower Solar", NPC.downedTowerSolar);
        processList.Add("Tower Nebula", NPC.downedTowerNebula);
        processList.Add("Tower Vortex", NPC.downedTowerVortex);
        processList.Add("Tower Stardust", NPC.downedTowerStardust);
        
        processList.Add("Moon Lord", NPC.downedMoonlord);
        processList.Add("Pillars", NPC.downedTowerSolar && NPC.downedTowerNebula && NPC.downedTowerVortex &&  NPC.downedTowerStardust);
        processList.Add("Goblins", NPC.downedGoblins);
        processList.Add("Pirates", NPC.downedPirates);
        processList.Add("Frost", NPC.downedFrost);
        processList.Add("Frost Moon", NPC.downedChristmasTree && NPC.downedChristmasSantank && NPC.downedChristmasIceQueen);
        processList.Add("Pumpkin Moon", NPC.downedHalloweenTree && NPC.downedHalloweenKing);
        processList.Add("Martians", NPC.downedMartians);
        processList.Add("DD2InvasionT1", DD2Event.DownedInvasionT1);
        processList.Add("DD2InvasionT2", DD2Event.DownedInvasionT2);
        processList.Add("DD2InvasionT3", DD2Event.DownedInvasionT3);
        return processList;
    }
    
    internal static Dictionary<string,int> GetKillCountList()
    {
        int GetKillCount(int id)
        {
           return Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[id]);
        }
        // ReSharper disable once UseObjectOrCollectionInitializer
        Dictionary<string, int> killCountList = new();
        killCountList.Add("King Slime", GetKillCount(NPCID.KingSlime));
        killCountList.Add("Eye of Cthulhu", GetKillCount(NPCID.EyeofCthulhu));
        killCountList.Add("Eater of Worlds", GetKillCount(NPCID.EaterofWorldsHead));
        killCountList.Add("Brain of Cthulhu", GetKillCount(NPCID.BrainofCthulhu));
        killCountList.Add("Queen Bee", GetKillCount(NPCID.QueenBee));
        killCountList.Add("Deerclops", GetKillCount(NPCID.Deerclops));
        killCountList.Add("Skeletron", GetKillCount(NPCID.SkeletronHead));
        killCountList.Add("Wall of Flesh", GetKillCount(NPCID.WallofFlesh));
        killCountList.Add("Queen Slime", GetKillCount(NPCID.QueenSlimeBoss));
        killCountList.Add("The Twins", Math.Min(GetKillCount(NPCID.Retinazer),GetKillCount(NPCID.Spazmatism)));
        killCountList.Add("The Destroyer", GetKillCount(NPCID.TheDestroyer));
        killCountList.Add("Skeletron Prime", GetKillCount(NPCID.SkeletronPrime));
        killCountList.Add("Plantera", GetKillCount(NPCID.Plantera));
        killCountList.Add("Golem", GetKillCount(NPCID.Golem));
        killCountList.Add("Duke Fishron", GetKillCount(NPCID.DukeFishron));
        killCountList.Add("Empress of Light", GetKillCount(NPCID.HallowBoss));
        killCountList.Add("Lunatic Cultist", GetKillCount(NPCID.CultistBoss));
        killCountList.Add("Moon Lord", GetKillCount(NPCID.MoonLordCore));
        return killCountList;
    }
    
    
    internal static List<string> GetOnlineProcessList()
    {
        List<string> onlineProcessList = new ();

        #region 进度查询

        if (!NPC.downedSlimeKing)
        {
            onlineProcessList.Add("史王");
        }

        if (!NPC.downedBoss1)
        {
            onlineProcessList.Add("克眼");
        }

        if (!NPC.downedBoss2)
        {
            if (Main.drunkWorld)
            {
                onlineProcessList.Add("世吞/克脑");
            }
            else
            {
                onlineProcessList.Add(WorldGen.crimson ? "克脑" : "世吞");
            }
        }

        if (!NPC.downedBoss3)
        {
            onlineProcessList.Add("骷髅王");
        }

        if (!Main.hardMode)
        {
            onlineProcessList.Add("血肉墙");
        }

        if (!NPC.downedMechBoss2 || !NPC.downedMechBoss1 || !NPC.downedMechBoss3)
        {
            onlineProcessList.Add(Main.zenithWorld ? "美杜莎" : "新三王");
        }

        if (!NPC.downedPlantBoss)
        {
            onlineProcessList.Add("世花");
        }

        if (!NPC.downedGolemBoss)
        {
            onlineProcessList.Add("石巨人");
        }

        if (!NPC.downedAncientCultist)
        {
            onlineProcessList.Add("拜月教徒");
        }

        if (!NPC.downedTowers)
        {
            onlineProcessList.Add("四柱");
        }

        if (!NPC.downedMoonlord)
        {
            onlineProcessList.Add("月总");
        }

        return onlineProcessList;

        #endregion
    }

    internal static string FileToBase64String(string path)
    {
        FileStream fsForRead = new (path, FileMode.Open); //文件路径
        var base64Str = "";
        try
        {
            fsForRead.Seek(0, SeekOrigin.Begin);
            var bs = new byte[fsForRead.Length];
            var log = Convert.ToInt32(fsForRead.Length);
            _ = fsForRead.Read(bs, 0, log);
            base64Str = Convert.ToBase64String(bs);
            return base64Str;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            return base64Str;
        }
        finally
        {
            fsForRead.Close();
        }
    }

    internal static string CompressBase64(string base64String)
    {
        var base64Bytes = Encoding.UTF8.GetBytes(base64String);
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzipStream.Write(base64Bytes, 0, base64Bytes.Length);
            }

            return Convert.ToBase64String(outputStream.ToArray());
        }
    }

    internal static List<int> GetActiveBuffs(IDbConnection connection, int userId, string name)
    {
        try
        {
            var queryString2 = $"SELECT buffid FROM Permabuff WHERE Name = '{name}'";

            using var command = connection.CreateCommand();
            command.CommandText = queryString2;

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var activeBuffsString = reader.GetString(0);
                var activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                return activeBuffsList;
            }
        }
        catch
        {
            // ignored
        }

        try
        {
            var queryString = $"SELECT ActiveBuffs FROM Permabuffs WHERE UserID = '{userId}'";

            using var command = connection.CreateCommand();
            command.CommandText = queryString;

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var activeBuffsString = reader.GetString(0);
                var activeBuffsList = activeBuffsString.Split(',').Select(int.Parse).ToList();
                return activeBuffsList;
            }
        }
        catch
        {
            // ignored
        }

        return new List<int>();
    }
}