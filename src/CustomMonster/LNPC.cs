using Terraria;
using static CustomMonster.TestPlugin;

namespace CustomMonster;

public class LNPC
{
    public int Index { get; set; }

    public int Time { get; set; }

    public float TiemN { get; set; }

    public int PlayerCount { get; set; }

    public int MaxLife { get; set; }

    public int MaxTime { get; set; }

    public MonsterGroup? Config { get; set; }

    public int LifeP { get; set; }

    public int LLifeP { get; set; }

    public float LTime { get; set; }

    public long LKC { get; set; }

    public List<BuffGroup>? RBuff { get; set; }

    public int BuffR { get; set; }

    public List<RatioGroup>? PLife { get; set; }

    public List<TimeGroup>? CTime { get; set; }

    public int KillPlay { get; set; }

    public int RespawnSeconds { get; set; }

    public int DefaultMaxSpawns { get; set; }

    public int DefaultSpawnRate { get; set; }

    public int BlockTeleporter { get; set; }

    public int OSTime { get; set; }

    public int Struck { get; set; }

    public List<IndicatorGroup2> Markers { get; set; }

    public LNPC(int index, int playercount, int life, MonsterGroup config, int maxtime, int ostime, long kc)
    {
        this.Index = index;
        this.PlayerCount = playercount;
        this.KillPlay = 0;
        this.Time = 0;
        this.TiemN = 0f;
        this.MaxLife = life;
        this.MaxTime = maxtime;
        this.Config = config ?? throw new ArgumentNullException(nameof(config), "Config cannot be null.");
        this.LTime = 0f;
        this.LifeP = 100;
        this.LLifeP = 100;
        this.BuffR = 0;
        this.OSTime = ostime;
        this.LKC = kc;
        this.Markers = new List<IndicatorGroup2>();
        this.Struck = 0;

        if (this.Config != null)
        {
            this.RespawnSeconds = config!.RespawnSeconds;
            this.BlockTeleporter = config.BlockTeleporter;
            this.DefaultMaxSpawns = config.DefaultMaxSpawns;
            this.DefaultSpawnRate = config.DefaultSpawnRate;
            this.PLife = new List<RatioGroup>();
            this.Config.LifeEvent.ForEach(delegate (RatioGroup i)
            {
                this.PLife.Add((RatioGroup) i.Clone());
            });
            this.CTime = new List<TimeGroup>();
            this.Config.TimeEvent.ForEach(delegate (TimeGroup i)
            {
                this.CTime.Add((TimeGroup) i.Clone());
            });
        }
    }

    #region 设置或更新指定名称的标记（Marker）的
    public void setMarkers(string name, int num, bool reset)
    {
        if (!this.Markers!.Exists(t => t.IndName == name))
        {
            this.Markers.Add(new IndicatorGroup2(name, 0));
        }
        foreach (var marker in this.Markers)
        {
            if (marker.IndName == name)
            {
                if (reset)
                {
                    marker.IndStack += num;
                }
                else
                {
                    marker.IndStack += num;
                }
                break;
            }
        }
    }
    #endregion

    #region 设置或更新指定名称的标记（Marker）的堆叠数量，并应用额外的运算规则和随机数调整。
    public void setMarkers(string name, int num, bool reset, string inname, float infactor, string inop, int rmin, int rmax, ref Random rd, NPC npc)
    {
        var name2 = name;
        if (!this.Markers!.Exists((IndicatorGroup2 t) => t.IndName == name2))
        {
            this.Markers.Add(new IndicatorGroup2(name2, 0));
        }
        var num2 = 0;
        if (rmax > rmin)
        {
            num2 = rd.Next(rmin, rmax);
        }
        var num3 = this.addMarkersIn(inname, infactor, npc);
        foreach (var marker in this.Markers)
        {
            if (marker.IndName == name2)
            {
                marker.IndStack = reset ? Sundry.intoperation(inop, 0, num + num2 + num3) : Sundry.intoperation(inop, marker.IndStack, num + num2 + num3);
                break;
            }
        }
    }
    #endregion

    #region 根据输入名称从NPC或相关联的数据中获取附加值。
    public int addMarkersIn(string inname, float infactor, NPC npc)
    {
        var num = 0f;
        if (LNpcs![npc.whoAmI] == null)
        {
            return (int) num;
        }
        if (inname != "")
        {
            if (inname == "[序号]" && npc != null)
            {
                num = npc.whoAmI;
            }
            else if (npc != null)
            {
                switch (inname)
                {
                    case "[Struck]":
                    case "[被击]":
                        num = LNpcs[npc.whoAmI].Struck;
                        break;
                    case "[KillPlay]":
                    case "[击杀]":
                        num = LNpcs[npc.whoAmI].KillPlay;
                        break;
                    case "[TiemN]":
                    case "[耗时]":
                        num = (int) LNpcs[npc.whoAmI].TiemN;
                        break;
                    case "[X]":
                    case "[X坐标]":
                        if (npc != null)
                        {
                            num = (int) npc.Center.X;
                            break;
                        }
                        goto default;
                    default:
                    {
                        if (npc != null && (inname == "[Y坐标]" || inname == "[Y]"))
                        {
                            num = (int) npc.Center.Y;
                            break;
                        }
                        if (npc != null && (inname == "[血量]" || inname == "[lift]"))
                        {
                            num = npc.life;
                            break;
                        }
                        if (!(inname == "[被杀]" || inname == "[Killed]") || npc == null)
                        {
                            num = (inname == "[AI0]" && npc != null) ? npc.ai[0] : ((inname == "[AI1]" && npc != null) ? npc.ai[1] : ((inname == "[AI2]" && npc != null) ? npc.ai[2] : ((!(inname == "[AI3]") || npc == null) ? LNpcs[npc!.whoAmI].getMarkers(inname) : npc.ai[3])));
                            break;
                        }
                        var num2 = getLNKC(npc.netID);
                        if (num2 > int.MaxValue)
                        {
                            num2 = 2147483647L;
                        }
                        num = (int) num2;
                        break;
                    }
                }
            }
        }
        if (num != 0f)
        {
            num *= infactor;
        }
        return (int) num;
    }
    #endregion

    #region 获取指定名称的标记（Marker）的当前堆叠数量。
    public int getMarkers(string name)
    {
        if (name == "")
        {
            return 0;
        }
        foreach (var marker in this.Markers!)
        {
            if (marker.IndName == name)
            {
                return marker.IndStack;
            }
        }
        return 0;
    }
    #endregion

    #region 替换文本中所有标记（Marker）占位符为对应的堆叠数量。
    public string ReplaceMarkers(string text)
    {
        var text2 = text;
        foreach (var marker in this.Markers!)
        {
            text2 = text2.Replace("[" + marker.IndName + "]", marker.IndStack.ToString());
        }
        return text2;
    }
    #endregion

    #region 检查给定的标记列表是否满足指定条件。
    public bool haveMarkers(List<IndicatorGroup2> list, NPC npc)
    {
        var flag = false;
        foreach (var item in list)
        {
            var markers = this.getMarkers(item.IndName);
            var num = item.IndStack + this.addMarkersIn(item.InjectionStackName, item.InjectionStackRatio, npc);
            if (item.IFMark != "")
            {
                if (item.IFMark == "=")
                {
                    if (markers != num)
                    {
                        flag = true;
                        break;
                    }
                }
                else if (item.IFMark == "!")
                {
                    if (markers == num)
                    {
                        flag = true;
                        break;
                    }
                }
                else if (item.IFMark == ">")
                {
                    if (markers <= num)
                    {
                        flag = true;
                        break;
                    }
                }
                else if (item.IFMark == "<" && markers >= num)
                {
                    flag = true;
                    break;
                }
            }
            else
            {
                if (num == 0)
                {
                    continue;
                }
                if (num > 0)
                {
                    if (markers < num)
                    {
                        flag = true;
                        break;
                    }
                }
                else if (markers >= Math.Abs(num))
                {
                    flag = true;
                    break;
                }
            }
        }
        return !flag;
    }
    #endregion
}
