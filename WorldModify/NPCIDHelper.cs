using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace WorldModify
{
    internal class NPCIDHelper
    {
        private static Dictionary<int, string> _dics = new Dictionary<int, string>
    {
        { 17, "商人" },
        { 18, "护士" },
        { 19, "军火商" },
        { 20, "树妖" },
        { 22, "向导" },
        { 37, "老人" },
        { 38, "爆破专家" },
        { 54, "服装商" },
        { 105, "受缚哥布林" },
        { 106, "受缚巫师" },
        { 107, "哥布林工匠" },
        { 108, "巫师" },
        { 123, "受缚机械师" },
        { 124, "机械师" },
        { 142, "圣诞老人" },
        { 160, "松露人" },
        { 178, "蒸汽朋克人" },
        { 207, "染料商" },
        { 208, "派对女孩" },
        { 209, "机器侠" },
        { 227, "油漆工" },
        { 228, "巫医" },
        { 229, "海盗" },
        { 353, "发型师" },
        { 354, "受缚发型师" },
        { 368, "旅商" },
        { 369, "渔夫" },
        { 376, "沉睡渔夫" },
        { 441, "税收官" },
        { 453, "骷髅商人" },
        { 550, "酒馆老板" },
        { 588, "高尔夫球手" },
        { 589, "高尔夫球手待拯救" },
        { 633, "动物学家" },
        { 663, "公主" },
        { 637, "猫咪" },
        { 638, "狗狗" },
        { 656, "兔兔" },
        { 670, "呆瓜史莱姆" },
        { 678, "冷酷史莱姆" },
        { 679, "年长史莱姆" },
        { 680, "笨拙史莱姆" },
        { 681, "唱将史莱姆" },
        { 682, "粗暴史莱姆" },
        { 683, "神秘史莱姆" },
        { 684, "侍卫史莱姆" },
        { 685, "老旧摇摇箱" },
        { 686, "笨拙气球史莱姆" },
        { 687, "神秘青蛙" }
    };

        public static int[] smIDs = new int[49]
        {
        17, 18, 19, 20, 22, 37, 38, 54, 105, 106,
        107, 108, 123, 124, 142, 160, 178, 207, 208, 209,
        227, 228, 229, 353, 354, 368, 369, 369, 441, 453,
        550, 588, 589, 633, 663, 637, 638, 656, 670, 678,
        679, 680, 681, 682, 683, 684, 685, 686, 687
        };

        public static int GetIDByName(string _name)
        {
            if (_dics.ContainsValue(_name))
            {
                foreach (KeyValuePair<int, string> dic in _dics)
                {
                    if (dic.Value == _name)
                    {
                        return dic.Key;
                    }
                }
            }
            string text = _name.ToLowerInvariant();
            for (int i = 0; i < NPCID.Count; i++)
            {
                if (Lang.GetNPCNameValue(i).ToLowerInvariant() == text)
                {
                    return i;
                }
            }
            return 0;
        }

        public static string GetNameByID(int _id)
        {
            if (_dics.ContainsKey(_id))
            {
                return _dics[_id];
            }
            return Lang.GetNPCNameValue(_id);
        }
    }
}
