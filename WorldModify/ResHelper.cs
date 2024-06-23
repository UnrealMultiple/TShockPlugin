using System.Text;
using System.Xml.Linq;
using Terraria.DataStructures;

namespace WorldModify
{
    public class ResHelper
    {
        public static Dictionary<int, WallProp> Walls = new Dictionary<int, WallProp>();

        public static void LoadWall()
        {
            if (!Walls.Any())
            {
                int num = 1;
                string[] array = Utils.FromEmbeddedPath("Wall.csv").Split('\n');
                foreach (string text in array)
                {
                    string[] array2 = text.Split(',');
                    Walls.Add(num, new WallProp
                    {
                        id = num,
                        name = array2[0],
                        color = array2[1]
                    });
                    num++;
                }
            }
        }

        public static WallProp GetWallByIDOrName(string idOrName)
        {
            LoadWall();
            if (!int.TryParse(idOrName, out var result))
            {
                foreach (WallProp value in Walls.Values)
                {
                    if (value.name == idOrName)
                    {
                        result = value.id;
                        break;
                    }
                }
            }
            return Walls.ContainsKey(result) ? Walls[result] : null;
        }

        public static WallProp GetWallByID(ushort id)
        {
            return GetWallByIDOrName(id.ToString());
        }

        public static string GetWallDescByIDOrName(string idOrName)
        {
            WallProp wallByIDOrName = GetWallByIDOrName(idOrName);
            return (wallByIDOrName != null) ? wallByIDOrName.Desc : "";
        }

        public static void DumpXML()
        {
            string text = Utils.FromCombinePath("settings.xml");
            if (string.IsNullOrEmpty(text))
            {
                Utils.Log(Utils.SaveDir + " 目录下没有 settings.xml 文件");
                return;
            }
            XElement xElement = XElement.Parse(text);
            List<TileProp> list = new List<TileProp>();
            List<int> list2 = new List<int>();
            List<int> list3 = new List<int>();
            foreach (XElement item in xElement.Elements("Tiles").Elements("Tile"))
            {
                TileProp tileProp = new TileProp
                {
                    id = ((int?)item.Attribute("Id")).GetValueOrDefault(),
                    isFrame = ((bool?)item.Attribute("Framed")).GetValueOrDefault(),
                    name = (string?)item.Attribute("Name"),
                    color = (string?)item.Attribute("Color")
                };
                if (((bool?)item.Attribute("Solid")).GetValueOrDefault())
                {
                    list2.Add(tileProp.id);
                }
                if (tileProp.isFrame)
                {
                    list3.Add(tileProp.id);
                }
                Point16 val = StringToPoint16((string?)item.Attribute("Size"));
                if (val.X != 0 && val.Y != 0)
                {
                    tileProp.w = val.X;
                    tileProp.h = val.Y;
                }
                foreach (XElement item2 in item.Elements("Frames").Elements("Frame"))
                {
                    val = StringToPoint16((string?)item2.Attribute("UV"));
                    string name = (string?)item2.Attribute("Name");
                    string variety = (string?)item2.Attribute("Variety");
                    tileProp.Add(val.X, val.Y, name, variety);
                }
                list.Add(tileProp);
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("id,style,name,w,h,frameX,frameY,color");
            foreach (TileProp item3 in list)
            {
                StringBuilder stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder3 = stringBuilder2;
                StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 5, stringBuilder2);
                handler.AppendFormatted(item3.id);
                handler.AppendLiteral(",,");
                handler.AppendFormatted(item3.name);
                handler.AppendLiteral(",");
                handler.AppendFormatted(item3.w);
                handler.AppendLiteral(",");
                handler.AppendFormatted(item3.h);
                handler.AppendLiteral(",,,");
                handler.AppendFormatted(item3.color);
                stringBuilder3.AppendLine(ref handler);
                foreach (FrameProp frame in item3.frames)
                {
                    stringBuilder2 = stringBuilder;
                    StringBuilder stringBuilder4 = stringBuilder2;
                    handler = new StringBuilder.AppendInterpolatedStringHandler(6, 5, stringBuilder2);
                    handler.AppendFormatted(item3.id);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(frame.style);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(frame.name);
                    handler.AppendLiteral(",,,");
                    handler.AppendFormatted(frame.frameX);
                    handler.AppendLiteral(",");
                    handler.AppendFormatted(frame.frameY);
                    stringBuilder4.AppendLine(ref handler);
                }
            }
            File.WriteAllText(Utils.CombinePath("Tile.csv"), stringBuilder.ToString());
            Utils.Log("已生成 " + Utils.CombinePath("Tile.csv"));
        }

        private static Point16 StringToPoint16(string text)
        {
            Point16 result = default;
            if (!string.IsNullOrWhiteSpace(text))
            {
                string[] array = text.Split(',');
                if (array.Length == 2 && short.TryParse(array[0], out var result2) && short.TryParse(array[1], out var result3))
                {
                    return new Point16(result2, result3);
                }
            }
            return result;
        }
    }
}
