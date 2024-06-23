using TShockAPI;

namespace WorldModify
{
    internal class WorldTool
    {
        public static void Manage(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count == 1)
            {
                player.SendErrorMessage("参数不够，用法如下");
                player.SendErrorMessage("/igen world <种子> [腐化] [大小] [彩蛋特性], 重建地图");
                player.SendErrorMessage("种子：输入任意种子名，0表示随机");
                player.SendErrorMessage("腐化：腐化/猩红 或 1/2, 0表示随机");
                player.SendErrorMessage("大小：小/中/大 或 1/2/3, 0表示忽略");
                player.SendErrorMessage("彩蛋特性：种子名中间输入英文逗号，例如 2020,ftw");
            }
            else if (!TileHelper.NeedWaitTask(player))
            {
                string seedStr = ((args.Parameters.Count > 1) ? args.Parameters[1] : "");
                string text = ((args.Parameters.Count > 2) ? args.Parameters[2] : "");
                string text2 = ((args.Parameters.Count > 3) ? args.Parameters[3] : "");
                string eggStr = "";
                if (args.Parameters.Count > 4)
                {
                    args.Parameters.RemoveAt(0);
                    args.Parameters.RemoveAt(0);
                    args.Parameters.RemoveAt(0);
                    args.Parameters.RemoveAt(0);
                    eggStr = string.Join(" ", args.Parameters);
                }
                int size = 0;
                if (text2 == "小" || text2 == "1")
                {
                    size = 1;
                }
                else if (text2 == "中" || text2 == "2")
                {
                    size = 2;
                }
                else if (text2 == "大" || text2 == "3")
                {
                    size = 3;
                }
                int evil = -1;
                if (text == "腐化" || text == "1")
                {
                    evil = 0;
                }
                else if (text == "猩红" || text == "2")
                {
                    evil = 1;
                }
                GenWorld(player, seedStr, size, evil, eggStr);
            }
        }

        private static async void GenWorld(TSPlayer op, string seedStr = "", int size = 0, int evil = -1, string eggStr = "")
        {
            BackupHelper.Backup(op, "GenWorld");
            if (!op.RealPlayer)
            {
                Console.WriteLine("seed:" + seedStr);
                op.SendErrorMessage("[i:556]世界正在解体~");
            }
            TSPlayer.All.SendErrorMessage("[i:556]世界正在解体~");
            int secondLast =  Utils.GetUnixTimestamp;
            ProcessSeeds(seedStr);
            ProcessEggSeeds(eggStr);
            seedStr = seedStr.ToLowerInvariant();
            if (string.IsNullOrEmpty(seedStr) || seedStr == "0")
            {
                seedStr = "random";
            }
            if (Terraria.Main.ActiveWorldFileData.Seed == 5162020)
            {
                seedStr = "5162020";
            }
            if (seedStr == "random")
            {
                Terraria.Main.ActiveWorldFileData.SetSeedToRandom();
            }
            else
            {
                Terraria.Main.ActiveWorldFileData.SetSeed(seedStr);
            }
            int tilesX = 0;
            int tilesY = 0;
            int rawSize = -1;
            if ((Terraria.Main.maxTilesX == 4200) & (Terraria.Main.maxTilesY == 1200))
            {
                rawSize = 1;
            }
            else if ((Terraria.Main.maxTilesX == 6400) & (Terraria.Main.maxTilesY == 1800))
            {
                rawSize = 2;
            }
            else if ((Terraria.Main.maxTilesX == 8400) & (Terraria.Main.maxTilesY == 2400))
            {
                rawSize = 3;
            }
            switch (size)
            {
                case 1:
                    tilesX = 4200;
                    tilesY = 1200;
                    break;
                case 2:
                    tilesX = 6400;
                    tilesY = 1800;
                    break;
                case 3:
                    tilesX = 8400;
                    tilesY = 2400;
                    break;
            }
            if (tilesX > 0)
            {
                Terraria.Main.maxTilesX = tilesX;
                Terraria.Main.maxTilesY = tilesY;
                Terraria.Main.ActiveWorldFileData.SetWorldSize(tilesX, tilesY);
            }
            Terraria.WorldGen.WorldGenParam_Evil = evil;
            if (!op.RealPlayer)
            {
                op.SendErrorMessage("[i:3061]世界正在重建（" + Terraria.WorldGen.currentWorldSeed + "）");
            }
            TSPlayer.All.SendErrorMessage("[i:3061]世界正在重建（" + Terraria.WorldGen.currentWorldSeed + "）");
            await AsyncGenerateWorld(Terraria.Main.ActiveWorldFileData.Seed);
            int second = Utils.GetUnixTimestamp - secondLast;
            string text = $"[i:3061]世界重建完成 （用时 {second}s, {Terraria.WorldGen.currentWorldSeed}）；-）";
            TSPlayer.All.SendSuccessMessage(text);
            if (!op.RealPlayer)
            {
                op.SendErrorMessage(text);
            }
            if (rawSize != -1 && size != 0 && rawSize != size)
            {
                if (Terraria.Main.ServerSideCharacter)
                {
                    TSPlayer[] players = TShock.Players;
                    foreach (TSPlayer player in players)
                    {
                        if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
                        {
                            player.SaveServerCharacter();
                        }
                    }
                }
                Utils.Log("服务器已关闭：重建后的地图大小和之前不一样，为了稳定起见，请重新开服");
                TShock.Utils.StopServer(save: true, "服务器已关闭：地图大小和创建前不一样");
            }
            TSPlayer[] players2 = TShock.Players;
            foreach (TSPlayer plr in players2)
            {
                if (plr?.Active ?? false)
                {
                    plr.Teleport(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16 - 48, 1);
                }
            }
        }

        private static Task AsyncGenerateWorld(int seed)
        {
            TileHelper.isTaskRunning = true;
            Terraria.WorldGen.clearWorld();
            return Task.Run(delegate
            {
                Terraria.WorldGen.GenerateWorld(seed, null);
            }).ContinueWith(delegate
            {
                TileHelper.GenAfter();
            });
        }

        private static void ProcessSeeds(string seed)
        {
            Terraria.WorldGen.notTheBees = false;
            Terraria.WorldGen.getGoodWorldGen = false;
            Terraria.WorldGen.tenthAnniversaryWorldGen = false;
            Terraria.WorldGen.dontStarveWorldGen = false;
            ToggleSpecialWorld(seed.ToLowerInvariant());
        }

        private static void ProcessEggSeeds(string seedstr)
        {
            string[] array = seedstr.ToLowerInvariant().Split(',');
            string[] array2 = array;
            foreach (string seed in array2)
            {
                ToggleSpecialWorld(seed);
            }
        }

        private static void ToggleSpecialWorld(string seed)
        {
            switch (seed)
            {
                case "2020":
                case "516":
                case "5162020":
                case "05162020":
                    Terraria.Main.ActiveWorldFileData._seed = 5162020;
                    break;
                case "2021":
                case "5162011":
                case "5162021":
                case "05162011":
                case "05162021":
                case "celebrationmk10":
                    Terraria.WorldGen.tenthAnniversaryWorldGen = true;
                    break;
                case "ntb":
                case "not the bees":
                case "not the bees!":
                    Terraria.WorldGen.notTheBees = true;
                    break;
                case "ftw":
                case "for the worthy":
                    Terraria.WorldGen.getGoodWorldGen = true;
                    break;
                case "dst":
                case "constant":
                case "theconstant":
                case "the constant":
                case "eye4aneye":
                case "eyeforaneye":
                    Terraria.WorldGen.dontStarveWorldGen = true;
                    break;
                case "superegg":
                    Terraria.Main.ActiveWorldFileData._seed = 5162020;
                    Terraria.WorldGen.notTheBees = true;
                    Terraria.WorldGen.getGoodWorldGen = true;
                    Terraria.WorldGen.tenthAnniversaryWorldGen = true;
                    Terraria.WorldGen.dontStarveWorldGen = true;
                    break;
            }
        }
    }
}
