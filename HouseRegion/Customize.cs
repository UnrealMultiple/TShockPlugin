using Microsoft.Xna.Framework;

namespace HouseRegion
{
    public class LPlayer//本地玩家类
    {
        public int Who { get; set; }
        public int TileX { get; set; }//上次瓷砖X位置目的用来判定玩家是否步入房子，若上次瓷砖位置在房子内表示已在房子内
        public int TileY { get; set; }//上次瓷砖Y位置
        public bool Look { get; set; }

        public LPlayer(int who, int lasttileX, int lasttileY)//类初始化时
        {
            Who = who;
            TileX = lasttileX;
            TileY = lasttileY;
            Look = false;
        }
    }

    public class House//本地房子类
    {
        public Rectangle HouseArea { get; set; }
        public string Author { get; set; }
        public List<string> Owners { get; set; }
        public string Name { get; set; }
        public bool Locked { get; set; }
        public List<string> Users { get; set; }

        public House(Rectangle housearea, string author, List<string> owners, string name, bool locked, List<string> users)//类初始化时
        {
            HouseArea = housearea;
            Author = author;
            Owners = owners;
            Name = name;
            Locked = locked;
            Users = users;
        }
    }
}
