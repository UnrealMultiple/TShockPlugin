using Terraria;

namespace CustomMonster;

public class LPrj
{
    public int Index { get; set; }

    public int Type { get; set; }

    public int UseI { get; set; }

    public string Notes { get; set; }

    public LPrj(int index, int useIndex, int type, string notes)
    {
        this.Index = index;
        this.UseI = useIndex;
        this.Type = type;
        this.Notes = notes;
    }

    #region 在满足条件时销毁游戏世界中对应的弹幕。
    public void clear(string notes)
    {
        if ((!(notes != "") || !(this.Notes != notes)) && this.Index >= 0)
        {
            var index = this.Index;
            this.Index = -1;
            if (Main.projectile[index] != null && Main.projectile[index].active && Main.projectile[index].type == this.Type && Main.projectile[index].owner == Main.myPlayer)
            {
                Main.projectile[index].Kill();
            }
        }
    } 
    #endregion
}

