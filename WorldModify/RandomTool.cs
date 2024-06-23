using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace WorldModify
{
    internal class RandomTool
    {
        public static readonly List<int> matchBlockID = new List<int>
    {
        0, 1, 2, 6, 7, 8, 9, 22, 23, 25,
        30, 32, 37, 38, 39, 40, 41, 43, 44, 45,
        46, 47, 48, 51, 52, 53, 54, 56, 57, 58,
        59, 60, 62, 63, 64, 65, 66, 67, 68, 69,
        70, 75, 76, 107, 108, 109, 111, 112, 115, 116,
        117, 118, 119, 120, 121, 122, 123, 130, 131, 140,
        145, 146, 147, 148, 151, 152, 153, 154, 155, 156,
        157, 158, 159, 160, 161, 162, 163, 164, 166, 167,
        168, 169, 170, 175, 176, 177, 179, 180, 181, 182,
        183, 188, 189, 190, 191, 192, 193, 194, 195, 196,
        197, 198, 199, 200, 202, 203, 204, 205, 206, 208,
        211, 221, 222, 223, 224, 225, 226, 229, 230, 232,
        234, 248, 249, 250, 251, 252, 253, 254, 255, 256,
        257, 258, 259, 260, 261, 262, 263, 264, 265, 266,
        267, 268, 273, 274, 284, 311, 312, 313, 321, 322,
        325, 326, 327, 328, 329, 330, 331, 332, 333, 345,
        346, 347, 348, 350, 351, 352, 357, 367, 368, 369,
        370, 371, 379, 381, 382, 383, 384, 385, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 407, 408, 409,
        415, 416, 417, 418, 426, 429, 430, 431, 432, 433,
        434, 445, 446, 447, 448, 449, 450, 451, 458, 459,
        460, 472, 473, 474, 477, 478, 479, 481, 482, 483,
        492, 495, 496, 498, 500, 501, 502, 503, 507, 508,
        512, 513, 514, 515, 516, 517, 534, 535, 536, 537,
        539, 540, 541, 546, 557, 561, 562, 563, 566, 576,
        577, 618, 666
    };

        public static readonly List<int> randomBlockID = new List<int>
    {
        0, 1, 2, 6, 7, 8, 9, 22, 23, 30,
        32, 37, 38, 39, 40, 41, 43, 44, 45, 46,
        47, 48, 51, 52, 53, 54, 56, 57, 58, 59,
        60, 62, 63, 64, 65, 66, 67, 68, 69, 70,
        75, 76, 119, 120, 121, 122, 123, 124, 130, 131,
        140, 145, 146, 147, 148, 150, 151, 152, 153, 154,
        155, 156, 157, 158, 159, 160, 161, 162, 166, 167,
        168, 169, 170, 175, 176, 177, 179, 180, 181, 182,
        183, 188, 189, 190, 191, 192, 193, 194, 195, 196,
        197, 198, 199, 202, 204, 206, 208, 224, 225, 226,
        229, 230, 232, 248, 249, 250, 251, 252, 254, 255,
        256, 257, 258, 259, 260, 261, 262, 263, 264, 265,
        266, 267, 268, 273, 274, 284, 311, 312, 313, 321,
        322, 325, 326, 327, 328, 329, 345, 346, 347, 348,
        350, 351, 357, 367, 368, 369, 370, 371, 379, 381,
        382, 383, 384, 385, 396, 397, 404, 407, 415, 416,
        417, 418, 426, 429, 430, 431, 432, 433, 434, 445,
        446, 447, 448, 449, 450, 451, 458, 459, 460, 472,
        473, 474, 477, 478, 479, 481, 482, 483, 492, 495,
        496, 498, 500, 501, 502, 503, 507, 508, 512, 513,
        514, 515, 516, 517, 534, 535, 536, 537, 539, 540,
        541, 546, 557, 561, 562, 563, 566, 574, 575, 576,
        577, 578, 618, 666
    };

        private static Dictionary<int, int> Mapping = new Dictionary<int, int>();

        private static Dictionary<int, int> WallMapping = new Dictionary<int, int>();

        public static async void RandomAll(CommandArgs args)
        {
            TSPlayer op = args.Player;
            await AsyncRandomArea(op, Utils.GetWorldArea());
        }

        private static Task AsyncRandomArea(TSPlayer op, Rectangle rect)
        {
            int secondLast = Utils.GetUnixTimestamp;
            op.SendSuccessMessage("全图随机开始……");
            return Task.Run(delegate
            {
                ResetTileMapping();
                for (int i = rect.X; i < rect.Right; i++)
                {
                    for (int j = rect.Y; j < rect.Bottom; j++)
                    {
                        RandomTile(i, j);
                    }
                }
            }).ContinueWith(delegate
            {
                TileHelper.FinishGen();
                int value = Utils.GetUnixTimestamp - secondLast;
                op.SendSuccessMessage($"随机完成（用时 {value}s）");
            });
        }

        public static int RandomTile(int x, int y)
        {
            ITile val = Main.tile[x, y];
            bool flag = false;
            if (val.active() && matchBlockID.Contains(val.type) && Mapping.ContainsKey(val.type))
            {
                val.type = (ushort)Mapping[val.type];
                NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                flag = true;
            }
            if (val.wall != 0 && WallMapping.ContainsKey(val.wall))
            {
                val.wall = (ushort)WallMapping[val.wall];
                NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
                flag = true;
            }
            return flag ? 1 : 0;
        }

        public static void ResetTileMapping()
        {
            Mapping = GetRandomBlockMapping();
            WallMapping = GetRandomWallMapping();
        }

        private static Dictionary<int, int> GetRandomBlockMapping()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Random random = new Random((int)DateTime.Now.Ticks);
            List<int> list = new List<int>(randomBlockID);
            List<int> list2 = new List<int>(randomBlockID);
            int count = list2.Count;
            while (count > 1)
            {
                int index = random.Next(count--);
                int value = list2[count];
                list2[count] = list2[index];
                list2[index] = value;
            }
            for (int i = 0; i < list.Count; i++)
            {
                dictionary.Add(list[i], list2[i]);
            }
            return dictionary;
        }

        private static Dictionary<int, int> GetRandomWallMapping()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            Random random = new Random((int)DateTime.Now.Ticks);
            List<int> list = new List<int>();
            for (int i = 1; i < WallID.Count; i++)
            {
                list.Add(i);
            }
            List<int> list2 = new List<int>(list);
            int count = list2.Count;
            while (count > 1)
            {
                int index = random.Next(count--);
                int value = list2[count];
                list2[count] = list2[index];
                list2[index] = value;
            }
            for (int j = 0; j < list.Count; j++)
            {
                dictionary.Add(list[j], list2[j]);
            }
            return dictionary;
        }
    }
}
