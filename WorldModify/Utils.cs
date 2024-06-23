using Microsoft.Xna.Framework;
using System.Reflection;
using System.Security.Cryptography;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace WorldModify
{
    internal class Utils 
    {
        public static string? SaveDir;

        public static int GetUnixTimestamp => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static string CFlag(bool foo, string fstr)
        {
            return foo ? ("[c/96FF96:✔]" + fstr) : ("-" + fstr);
        }

        public static string CFlag(string fstr, bool foo)
        {
            return foo ? (fstr + "✓") : (fstr + "-");
        }

        public static string BFlag(bool _vaule)
        {
            return _vaule ? "已开启" : "已关闭";
        }

        public static bool NeedInGame(TSPlayer op)
        {
            if (!op.RealPlayer)
            {
                op.SendErrorMessage("请进入游戏后再操作！");
            }
            return !op.RealPlayer;
        }

        public static bool InArea(Rectangle rect, Rectangle point)
        {
            return InArea(rect, point.X, point.Y);
        }

        public static bool InArea(Rectangle rect, int x, int y)
        {
            return x >= rect.X && x <= rect.X + rect.Width && y >= rect.Y && y <= rect.Y + rect.Height;
        }

        public static bool IsProtected(int x, int y)
        {
            IEnumerable<Region> source = TShock.Regions.InAreaRegion(x, y);
            return source.Any();
        }

        public static bool IsBase(int x, int y)
        {
            int num = 122;
            int num2 = 68;
            Rectangle val = default(Rectangle);
            new Rectangle (Main.spawnTileX - num / 2, Main.spawnTileY - num2 / 2, num, num2);
            return ((Rectangle)( val)).Contains(x, y);
        }

        public static Rectangle GetScreen(TSPlayer op)
        {
            return GetScreen(op.TileX, op.TileY);
        }

        public static Rectangle GetScreen(int playerX, int playerY)
        {
            return new Rectangle(playerX - 59, playerY - 35 + 3, 120, 68);
        }

        public static Rectangle GetWorldArea()
        {
            return new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY);
        }

        public static Rectangle GetBaseArea()
        {
            return new Rectangle(Main.spawnTileX - 59, Main.spawnTileY - 35 + 3, 120, 68);
        }

        public static Rectangle CloneRect(Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static string GetLocationDesc(float x, float y, int width, int height)
        {
            int num = (int)((x + width / 2) * 2f / 16f - Main.maxTilesX);
            string text = ((num > 0) ? $"{num}以东" : ((num >= 0) ? "中心" : $"{-num}以西"));
            int num2 = (int)((double)((y + height) * 2f / 16f) - Main.worldSurface * 2.0);
            float num3 = Main.maxTilesX / 4200f;
            num3 *= num3;
            int num4 = 1200;
            float num5 = (float)((double)((y + (height / 2)) / 16f - (65f + 10f * num3)) / (Main.worldSurface / 5.0));
            string text2 = ((y > ((Main.maxTilesY - 204) * 16)) ? "地狱" : (((double)y > Main.rockLayer * 16.0 + num4 / 2 + 16.0) ? "洞穴" : ((num2 > 0) ? "地下" : ((!(num5 >= 1f)) ? "太空" : "地表"))));
            num2 = Math.Abs(num2);
            string text3 = ((num2 != 0) ? $"{num2}的" : "级别");
            string text4 = text3 + text2;
            return text + " " + text4;
        }

        public static string GetLocationDesc(int tileX, int tileY)
        {
            return GetLocationDesc(tileX * 16, tileY * 16, 0, 0);
        }

        public static string GetLocationDesc(NPC npc)
        {
            return GetLocationDesc(npc.position.X, npc.position.Y, npc.width, npc.height);
        }

        public static string GetLocationDesc(Player plr)
        {
            return GetLocationDesc(plr.position.X, plr.position.Y, plr.width, plr.height);
        }

        public static bool StringToGuid(string str)
        {
            Guid guid = default;
            try
            {
                guid = new Guid(str);
            }
            catch (Exception)
            {
            }
            return guid != Guid.Empty;
        }

        public static bool TryParseInt(List<string> args, int index, out int num)
        {
            if (index >= args.Count)
            {
                num = 0;
                return false;
            }
            return int.TryParse(args[index], out num);
        }

        public static string TryParseString(List<string> args, int index)
        {
            return (index >= args.Count) ? "" : args[index];
        }

        public static string GetMD5HashFromFile(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            MD5 mD = MD5.Create();
            byte[] inArray = mD.ComputeHash(fileStream);
            fileStream.Close();
            return Convert.ToHexString(inArray);
        }

        public static void Save(string path, string content)
        {
            CreateSaveDir();
            File.WriteAllText(path, content);
        }

        public static void SafeSave(string path, string content)
        {
            CreateSaveDir();
            if (File.Exists(path))
            {
                string arg = path.Substring(path.Length - 3);
                string arg2 = path.Substring(0, path.Length - 4);
                File.Move(path, $"{arg2}-{DateTime.Now:yyyyMMddHHmmss}.{arg}");
            }
            File.WriteAllText(path, content);
        }

        public static void CreateSaveDir()
        {
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
        }

        public static string CombinePath(string fileName)
        {
            return Path.Combine(SaveDir, fileName);
        }

        public static string FromCombinePath(string fileName)
        {
            string path = CombinePath(fileName);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return "";
        }

        public static string FromEmbeddedPath(string fileName)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldModify.res." + fileName);
            if (manifestResourceStream == null)
            {
                Log("内嵌资源 " + fileName + " 加载失败");
                return "";
            }
            return new StreamReader(manifestResourceStream).ReadToEnd();
        }

        public static void Log(string msg)
        {
            TShock.Log.ConsoleInfo("[wm]" + msg);
        }
    }
}