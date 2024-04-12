using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Terraria;
using TShockAPI;

namespace CheckBag
{
    public class Configuration
    {
        [JsonProperty("配置说明")]
        public string README = "重载配置请输入：/reload";
        [JsonProperty("物品查询")]
        public string Wiki_ID = "https://terraria.wiki.gg/zh/wiki/Item_IDs";
        [JsonProperty("检测间隔（秒）")]
        public int DetectionInterval = 5;  // 秒
        [JsonProperty("更新频率（越小越快）")]
        public int UpdateRate = 60;  // 次
        [JsonProperty("封禁时长（分钟）")]
        public int BanTime = 10; // 分钟
        [JsonProperty("警告次数（封禁）")]
        public int WarningCount = 15;  // 次

        public static readonly string FilePath = Path.Combine(TShock.SavePath, "检查背包", "检查背包.json");
        [JsonProperty("全时期")]
        public List<ItemData> Anytime = new();
        [JsonProperty("哥布林入侵")]
        public List<ItemData> Goblins = new();
        [JsonProperty("史王前")]
        public List<ItemData> SlimeKing = new();
        [JsonProperty("克眼前")]
        public List<ItemData> Boss1 = new();
        [JsonProperty("鹿角怪前")]
        public List<ItemData> Deerclops = new();
        [JsonProperty("世吞克脑前")]
        public List<ItemData> Boss2 = new();
        [JsonProperty("蜂王前")]
        public List<ItemData> QueenBee = new();
        [JsonProperty("骷髅王前")]
        public List<ItemData> Boss3 = new();
        [JsonProperty("肉前")]
        public List<ItemData> hardMode = new();
        [JsonProperty("皇后前")]
        public List<ItemData> QueenSlime = new();
        [JsonProperty("一王前")]
        public List<ItemData> MechBossAny = new();
        [JsonProperty("三王前")]
        public List<ItemData> MechBoss = new();
        [JsonProperty("猪鲨前")]
        public List<ItemData> Fishron = new();
        [JsonProperty("光女前")]
        public List<ItemData> EmpressOfLight = new();
        [JsonProperty("花前")]
        public List<ItemData> PlantBoss = new();
        [JsonProperty("石前")]
        public List<ItemData> GolemBoss = new();
        [JsonProperty("拜月前")]
        public List<ItemData> AncientCultist = new();
        [JsonProperty("月前")]
        public List<ItemData> Moonlord = new();


        #region 默认配置
        public void Init()
        {
            #region 全时期
            Anytime = new List<ItemData> {
                new ItemData(74, 500, "铂金币"),
                new ItemData(75, 999, "坠落之星"),
                new ItemData(3617, 1, "广播盒")
            };
            #endregion

            #region 哥布林前
            Goblins = new List<ItemData> {
                new ItemData(128, 1,"火箭靴"),
                new ItemData(405, 1,"幽灵靴"),
                new ItemData(3993, 1,"仙灵靴"),
                new ItemData(908, 1,"熔岩靴"),
                new ItemData(898, 1,"闪电靴"),
                new ItemData(1862, 1,"霜花靴"),
                new ItemData(5000, 1,"泰拉闪耀靴"),
                new ItemData(1163, 1,"暴雪气球"),
                new ItemData(983, 1,"沙暴气球"),
                new ItemData(399, 1,"云朵气球"),
                new ItemData(1863, 1,"臭屁气球"),
                new ItemData(1252, 1,"黄马掌气球"),
                new ItemData(1251, 1,"白马掌气球"),
                new ItemData(1250, 1,"蓝马掌气球"),
                new ItemData(3250, 1,"绿马掌气球"),
                new ItemData(3251, 1,"琥珀马掌气球"),
                new ItemData(3241, 1,"鲨鱼龙气球"),
                new ItemData(3252, 1,"粉马掌气球"),
                new ItemData(1164, 1,"气球束"),
                new ItemData(3990, 1,"马掌气球束"),
                new ItemData(3990, 1,"水陆两用靴"),
                new ItemData(1860, 1,"水母潜水装备"),
                new ItemData(1861, 1,"北极潜水装备"),
                new ItemData(3995, 1,"青蛙装备"),
                new ItemData(407, 1,"工具腰带"),
                new ItemData(395, 1,"全球定位系统"),
                new ItemData(3122, 1,"R.E.K.3000"),
                new ItemData(3121, 1,"哥布林数据仪"),
                new ItemData(3036, 1,"探鱼器"),
                new ItemData(3123, 1,"个人数字助手"),
                new ItemData(555, 1,"魔力花"),
                new ItemData(4000, 1,"磁花"),
                new ItemData(2221, 1,"天界手铐"),
                new ItemData(1595, 1,"魔法手铐"),
                new ItemData(3061, 1,"建筑师发明背包"),
                new ItemData(5126, 1,"创造之手"),
                new ItemData(5358, 1, "贝壳电话（家）"),
                new ItemData(5359, 1, "贝壳电话（出生点）"),
                new ItemData(5360, 1, "贝壳电话（海洋）"),
                new ItemData(5361, 1, "贝壳电话（地狱）")
            };
            #endregion

            #region 史莱姆王前
            SlimeKing = new List<ItemData>{
                new ItemData(3318, 1, "宝藏袋（史莱姆王）"),
                new ItemData(2430, 1, "粘鞍"),
                new ItemData(256, 1, "忍者兜帽"),
                new ItemData(257, 1, "忍者衣"),
                new ItemData(258, 1, "忍者裤"),
                new ItemData(3090, 1, "皇家凝胶"),
            };
            #endregion

            #region 克眼前
            Boss1 = new List<ItemData> {
                new ItemData(3319, 1, "宝藏袋（克苏鲁之眼）"),
                new ItemData(3262, 1, "代码1球"),
                new ItemData(3097, 1, "克苏鲁护盾"),
            };
            #endregion

            #region 鹿角怪前
            Deerclops = new List<ItemData> {
                new ItemData(5111, 1, "宝藏袋（独眼巨鹿）"),
                new ItemData(5098, 1, "眼骨"),
                new ItemData(5095, 1, "露西斧"),
                new ItemData(5117, 1, "气喇叭"),
                new ItemData(5118, 1, "天候棒"),
                new ItemData(5119, 1, "眼球激光塔"),
            };
            #endregion

            #region 世吞克脑前
            Boss2 = new List<ItemData> {
                new ItemData(3320, 1, "宝藏袋（世界吞噬怪）"),
                new ItemData(3321, 1, "宝藏袋（克苏鲁之脑）"),
                new ItemData(174, 1, "狱石"),
                new ItemData(175, 1, "狱石锭"),
                new ItemData(122, 1, "熔岩镐"),
                new ItemData(120, 1, "熔火之怒"),
                new ItemData(119, 1, "烈焰回旋镖"),
                new ItemData(231, 1, "熔岩头盔"),
                new ItemData(232, 1, "熔岩胸甲"),
                new ItemData(233, 1, "熔岩护胫"),
                new ItemData(2365, 1, "小鬼法杖"),
                new ItemData(4821, 1, "防熔岩虫网"),
                new ItemData(121, 1, "火山"),
                new ItemData(3223, 1, "混乱之脑"),
                new ItemData(3224, 1, "蠕虫围巾"),
                new ItemData(3266, 1, "黑曜石逃犯帽"),
                new ItemData(3267, 1, "黑曜石风衣"),
                new ItemData(3268, 1, "黑曜石裤"),
                new ItemData(102, 1, "暗影头盔"),
                new ItemData(101, 1, "暗影鳞甲"),
                new ItemData(100, 1, "暗影护颈"),
                new ItemData(103, 1, "梦魇镐"),
                new ItemData(792, 1, "猩红头盔"),
                new ItemData(793, 1, "猩红鳞甲"),
                new ItemData(794, 1, "猩红护颈"),
                new ItemData(798, 1, "死亡使者镐"),
                new ItemData(3817, 1, "护卫奖章"),
                new ItemData(3813, 1, "护卫熔炉"),
                new ItemData(3809, 1, "学徒围巾"),
                new ItemData(3810, 1, "侍卫护盾"),
                new ItemData(197, 1, "星星炮"),
                new ItemData(123, 1, "流星头盔"),
                new ItemData(124, 1, "流星护甲"),
                new ItemData(125, 1, "流星护腿"),
                new ItemData(127, 1, "太空枪"),
                new ItemData(116, 1, "陨石"),
                new ItemData(117, 1, "陨石锭"),
                new ItemData(4076, 1, "虚空保险库"),
                new ItemData(4131, 1, "虚空袋"),
                new ItemData(5325, 1, "闭合的虚空袋")
            };
            #endregion

            #region 蜂王前
            QueenBee = new List<ItemData> {
                new ItemData(3322, 1, "宝藏袋（蜂王）"),
                new ItemData(1123, 1, "养蜂人"),
                new ItemData(2888, 1, "蜂膝弓"),
                new ItemData(1121, 1, "蜜蜂枪"),
                new ItemData(1132, 1, "蜂窝"),
                new ItemData(1130, 1, "蜜蜂手榴弹"),
                new ItemData(2431, 1, "蜂蜡"),
                new ItemData(2502, 1, "涂蜜护目镜"),
                new ItemData(1249, 1, "蜂蜜气球"),
                new ItemData(4007, 1, "毒刺项链"),
                new ItemData(5294, 1, "蜂巢球"),
                new ItemData(1158, 1, "矮人项链"),
                new ItemData(1430, 1, "灌注站"),
            };
            #endregion

            #region 骷髅王前
            Boss3 = new List<ItemData> {
                new ItemData(3323, 1, "宝藏袋（骷髅王）"),
                new ItemData(346, 1, "保险箱"),
                new ItemData(273, 1, "永夜刃"),
                new ItemData(329, 1, "暗影钥匙"),
                new ItemData(113, 1, "魔法导弹"),
                new ItemData(683, 1, "邪恶三叉戟"),
                new ItemData(157, 1, "海蓝权杖"),
                new ItemData(3019, 1, "地狱之翼弓"),
                new ItemData(219, 1, "凤凰爆破枪"),
                new ItemData(218, 1, "烈焰火鞭"),
                new ItemData(220, 1, "阳炎之怒"),
                new ItemData(3317, 1, "英勇球"),
                new ItemData(3282, 1, "喷流球"),
                new ItemData(155, 1, "村正"),
                new ItemData(156, 1, "钴护盾"),
                new ItemData(397, 1, "黑曜石护盾"),
                new ItemData(163, 1, "蓝月"),
                new ItemData(164, 1, "手枪"),
                new ItemData(151, 1, "死灵头盔"),
                new ItemData(152, 1, "死灵胸甲"),
                new ItemData(153, 1, "死灵护颈"),
                new ItemData(5074, 1, "脊柱骨鞭"),
                new ItemData(1313, 1, "骷髅头法书"),
                new ItemData(2999, 1, "施法桌"),
                new ItemData(3000, 1, "炼药桌"),
                new ItemData(890, 1, "扩音器"),
                new ItemData(891, 1, "邪眼"),
                new ItemData(904, 1, "反诅咒咒语"),
                new ItemData(2623, 1, "泡泡枪"),
                new ItemData(327, 1, "金钥匙"),
            };
            #endregion

            #region 肉山前
            hardMode = new List<ItemData> {
                new ItemData(3324, 1, "宝藏袋（血肉墙）"),
                new ItemData(2673, 1, "松露虫"),
                new ItemData(3991, 1, "奥术花"),
                new ItemData(3366, 1, "悠悠球袋"),
                new ItemData(400, 1, "精金头饰"),
                new ItemData(401, 1, "精金头盔"),
                new ItemData(402, 1, "精金面具"),
                new ItemData(403, 1, "精金胸甲"),
                new ItemData(404, 1, "精金护腿"),
                new ItemData(391, 1, "精金锭"),
                new ItemData(778, 1, "精金镐"),
                new ItemData(481, 1, "精金连弩"),
                new ItemData(524, 1, "精金熔炉"),
                new ItemData(376, 1, "秘银兜帽"),
                new ItemData(377, 1, "秘银头盔"),
                new ItemData(378, 1, "秘银帽"),
                new ItemData(379, 1, "秘银链甲"),
                new ItemData(380, 1, "秘银护胫"),
                new ItemData(382, 1, "秘银锭"),
                new ItemData(777, 1, "秘银镐"),
                new ItemData(436, 1, "秘银连弩"),
                new ItemData(525, 1, "秘银砧"),
                new ItemData(371, 1, "钴帽"),
                new ItemData(372, 1, "钴头盔"),
                new ItemData(373, 1, "钴面具"),
                new ItemData(374, 1, "钴胸甲"),
                new ItemData(375, 1, "钴护腿"),
                new ItemData(381, 1, "钴锭"),
                new ItemData(776, 1, "钴镐"),
                new ItemData(435, 1, "钴连弩"),
                new ItemData(1205, 1, "钯金面具"),
                new ItemData(1206, 1, "钯金头盔"),
                new ItemData(1207, 1, "钯金头饰"),
                new ItemData(1208, 1, "钯金胸甲"),
                new ItemData(1209, 1, "钯金护腿"),
                new ItemData(1184, 1, "钯金锭"),
                new ItemData(1187, 1, "钯金连弩"),
                new ItemData(1188, 1, "钯金镐"),
                new ItemData(1189, 1, "钯金钻头"),
                new ItemData(1210, 1, "山铜面具"),
                new ItemData(1211, 1, "山铜头盔"),
                new ItemData(1212, 1, "山铜头饰"),
                new ItemData(1213, 1, "山铜胸甲"),
                new ItemData(1214, 1, "山铜护腿"),
                new ItemData(1191, 1, "山铜锭"),
                new ItemData(1194, 1, "山铜连弩"),
                new ItemData(1195, 1, "山铜镐"),
                new ItemData(1196, 1, "山铜钻头"),
                new ItemData(1220, 1, "山铜砧"),
                new ItemData(1215, 1, "钛金面具"),
                new ItemData(1216, 1, "钛金头盔"),
                new ItemData(1217, 1, "钛金头饰"),
                new ItemData(1218, 1, "钛金胸甲"),
                new ItemData(1219, 1, "钛金护腿"),
                new ItemData(1198, 1, "钛金锭"),
                new ItemData(1201, 1, "钛金连弩"),
                new ItemData(1202, 1, "钛金镐"),
                new ItemData(1203, 1, "钛金钻头"),
                new ItemData(1221, 1, "钛金熔炉"),
                new ItemData(1328, 1, "海龟壳"),
                new ItemData(2161, 1, "寒霜核"),
                new ItemData(684, 1, "寒霜头盔"),
                new ItemData(685, 1, "寒霜胸甲"),
                new ItemData(686, 1, "寒霜护腿"),
                new ItemData(726, 1, "寒霜法杖"),
                new ItemData(1264, 1, "寒霜之花"),
                new ItemData(676, 1, "寒霜剑"),
                new ItemData(4911, 1, "冷鞭"),
                new ItemData(1306, 1, "冰雪镰刀"),
                new ItemData(3783, 1, "禁戒碎片"),
                new ItemData(3776, 1, "禁戒面具"),
                new ItemData(3777, 1, "禁戒长袍"),
                new ItemData(3778, 1, "禁戒裤"),
                new ItemData(2607, 1, "蜘蛛牙"),
                new ItemData(2370, 1, "蜘蛛面具"),
                new ItemData(2371, 1, "蜘蛛胸甲"),
                new ItemData(2372, 1, "蜘蛛护胫"),
                new ItemData(2551, 1, "蜘蛛法杖"),
                new ItemData(2366, 1, "蜘蛛女王法杖"),
                new ItemData(1308, 1, "剧毒法杖"),
                new ItemData(389, 1, "太极连枷"),
                new ItemData(426, 1, "毁灭刃"),
                new ItemData(3051, 1, "魔晶碎块"),
                new ItemData(422, 1, "圣水"),
                new ItemData(2998, 1, "召唤师徽章"),
                new ItemData(489, 1, "巫士徽章"),
                new ItemData(490, 1, "战士徽章"),
                new ItemData(491, 1, "游侠徽章"),
                new ItemData(492, 1, "恶魔之翼"),
                new ItemData(493, 1, "天使之翼"),
                new ItemData(785, 1, "鸟妖之翼"),
                new ItemData(1165, 1, "蝙蝠之翼"),
                new ItemData(761, 1, "仙灵之翼"),
                new ItemData(822, 1, "冰冻之翼"),
                new ItemData(485, 1, "月光护身符"),
                new ItemData(900, 1, "月亮石"),
                new ItemData(497, 1, "海神贝壳"),
                new ItemData(861, 1, "月亮贝壳"),
                new ItemData(3013, 1, "臭虎爪"),
                new ItemData(3014, 1, "爬藤怪法杖"),
                new ItemData(3015, 1, "腐香囊"),
                new ItemData(3016, 1, "血肉指虎"),
                new ItemData(3992, 1, "狂战士手套"),
                new ItemData(536, 1, "泰坦手套"),
                new ItemData(897, 1, "强力手套"),
                new ItemData(527, 1, "暗黑碎块"),
                new ItemData(528, 1, "光明碎块"),
                new ItemData(520, 1, "光明之魂"),
                new ItemData(521, 1, "暗影之魂"),
                new ItemData(575, 1, "飞翔之魂"),
                new ItemData(535, 1, "点金石"),
                new ItemData(860, 1, "神话护身符"),
                new ItemData(554, 1, "十字项链"),
                new ItemData(862, 1, "星星面纱"),
                new ItemData(1613, 1, "十字章护盾"),
                new ItemData(1612, 1, "十字章护符"),
                new ItemData(892, 1, "维生素"),
                new ItemData(886, 1, "盔甲抛光剂"),
                new ItemData(901, 1, "盔甲背带"),
                new ItemData(893, 1, "三折地图"),
                new ItemData(889, 1, "快走时钟"),
                new ItemData(903, 1, "计划书"),
                new ItemData(888, 1, "蒙眼布"),
                new ItemData(3781, 1, "袖珍镜"),
                new ItemData(5354, 1, "反光墨镜"),
                new ItemData(1253, 1, "冰冻海龟壳"),
                new ItemData(3290, 1, "狱火球"),
                new ItemData(3289, 1, "冰雪悠悠球"),
                new ItemData(3316, 1, "渐变球"),
                new ItemData(3315, 1, "好胜球"),
                new ItemData(3283, 1, "吉克球"),
                new ItemData(3054, 1, "暗影焰刀"),
                new ItemData(532, 1, "星星斗篷"),
                new ItemData(1247, 1, "蜜蜂斗篷"),
                new ItemData(1244, 1, "雨云魔杖"),
                new ItemData(1326, 1, "混沌传送杖"),
                new ItemData(522, 1, "诅咒焰"),
                new ItemData(519, 1, "诅咒焰"),
                new ItemData(3010, 1, "诅咒镖"),
                new ItemData(545, 1, "诅咒箭"),
                new ItemData(546, 1, "诅咒弹"),
                new ItemData(1332, 1, "灵液"),
                new ItemData(1334, 1, "灵液箭"),
                new ItemData(1335, 1, "灵液弹"),
                new ItemData(3011, 1, "灵液镖"),
                new ItemData(1356, 1, "灵液瓶"),
                new ItemData(1336, 1, "黄金雨"),
                new ItemData(1346, 1, "纳米机器人"),
                new ItemData(1350, 1, "纳米弹"),
                new ItemData(1357, 1, "纳米机器人瓶"),
                new ItemData(1347, 1, "爆炸粉"),
                new ItemData(1351, 1, "爆破弹"),
                new ItemData(526, 1, "独角兽角"),
                new ItemData(501, 1, "妖精尘"),
                new ItemData(516, 1, "圣箭"),
                new ItemData(502, 1, "水晶碎块"),
                new ItemData(518, 1, "水晶风暴"),
                new ItemData(515, 1, "水晶子弹"),
                new ItemData(3009, 1, "水晶镖"),
                new ItemData(534, 1, "霰弹枪"),
                new ItemData(3211, 1, "舌锋剑"),
                new ItemData(723, 1, "光束剑"),
                new ItemData(514, 1, "激光步枪"),
                new ItemData(1265, 1, "乌兹冲锋枪"),
                new ItemData(3788, 1, "玛瑙爆破枪"),
                new ItemData(3210, 1, "毒弹枪"),
                new ItemData(2270, 1, "鳄鱼机关枪"),
                new ItemData(434, 1, "发条式突击步枪"),
                new ItemData(496, 1, "冰雪魔杖"),
                new ItemData(3006, 1, "夺命杖"),
                new ItemData(3007, 1, "飞镖手枪"),
                new ItemData(3008, 1, "飞镖步枪"),
                new ItemData(3029, 1, "代达罗斯风暴弓"),
                new ItemData(3052, 1, "暗影焰弓"),
                new ItemData(5065, 1, "共鸣权杖"),
                new ItemData(4269, 1, "血红法杖"),
                new ItemData(4270, 1, "血荆棘"),
                new ItemData(4272, 1, "滴滴怪致残者"),
                new ItemData(3787, 1, "裂天剑"),
                new ItemData(1321, 1, "魔法箭袋"),
                new ItemData(4006, 1, "潜行者箭袋"),
                new ItemData(4002, 1, "熔火箭袋"),
                new ItemData(3103, 1, "无尽箭袋"),
                new ItemData(3104, 1, "无尽火枪袋"),
                new ItemData(2750, 1, "流星法杖"),
                new ItemData(905, 1, "钱币枪"),
                new ItemData(2584, 1, "海盗法杖"),
                new ItemData(854, 1, "优惠卡"),
                new ItemData(855, 1, "幸运币"),
                new ItemData(3034, 1, "钱币戒指"),
                new ItemData(3035, 1, "贪婪戒指"),
                new ItemData(1324, 1, "香蕉回旋镖"),
                new ItemData(3012, 1, "铁链血滴子"),
                new ItemData(4912, 1, "鞭炮"),
                new ItemData(544, 1, "机械魔眼"),
                new ItemData(556, 1, "机械蠕虫"),
                new ItemData(557, 1, "机械骷髅头"),
                new ItemData(3779, 1, "神灯烈焰")
            };
            #endregion

            #region 史莱姆女皇前
            QueenSlime = new List<ItemData> {
                new ItemData(4957, 1, "宝藏袋（史莱姆皇后）"),
                new ItemData(4987, 1, "挥发明胶"),
                new ItemData(4980, 1, "失谐钩爪"),
                new ItemData(4981, 1, "明胶女式鞍"),
                new ItemData(4982, 1, "水晶刺客兜帽"),
                new ItemData(4983, 1, "水晶刺客上衣"),
                new ItemData(4984, 1, "水晶刺客裤"),
                new ItemData(4758, 1, "刃杖"),
            };
            #endregion

            #region 一王前
            MechBossAny = new List<ItemData> {
                new ItemData(3325, 1, "宝藏袋（毁灭者"),
                new ItemData(3326, 1, "宝藏袋（双子魔眼）"),
                new ItemData(3327, 1, "宝藏袋（机械骷髅王）"),
                new ItemData(1291, 1, "生命果"),
                new ItemData(5338, 1, "神盾果"),
                new ItemData(533, 1, "巨兽鲨"),
                new ItemData(4060, 1, "超级星星炮"),
                new ItemData(561, 1, "光辉飞盘"),
                new ItemData(494, 1, "魔法竖琴"),
                new ItemData(495, 1, "彩虹魔杖"),
                new ItemData(4760, 1, "中士联盾"),
                new ItemData(506, 1, "火焰喷射器"),
                new ItemData(3284, 1, "代码2球"),
                new ItemData(3287, 1, "Red的抛球"),
                new ItemData(3288, 1, "女武神悠悠球"),
                new ItemData(3286, 1, "叶列茨球"),
                new ItemData(1515, 1, "蜜蜂之翼"),
                new ItemData(821, 1, "烈焰之翼"),
                new ItemData(748, 1, "喷气背包"),
                new ItemData(4896, 1, "远古神圣面具"),
                new ItemData(4897, 1, "远古神圣头盔"),
                new ItemData(4898, 1, "远古神圣头饰"),
                new ItemData(4899, 1, "远古神圣兜帽"),
                new ItemData(4900, 1, "远古神圣板甲"),
                new ItemData(4901, 1, "远古神圣护胫"),
                new ItemData(558, 1, "神圣头饰"),
                new ItemData(559, 1, "神圣面具"),
                new ItemData(553, 1, "神圣头盔"),
                new ItemData(4873, 1, "神圣兜帽"),
                new ItemData(551, 1, "神圣板甲"),
                new ItemData(552, 1, "神圣护胫"),
                new ItemData(1225, 1, "神圣锭"),
                new ItemData(578, 1, "神圣连弩"),
                new ItemData(4678, 1, "迪朗达尔"),
                new ItemData(550, 1, "永恒之枪"),
                new ItemData(2535, 1, "魔眼法杖"),
                new ItemData(3353, 1, "机械矿车"),
                new ItemData(547, 1, "恐惧之魂"),
                new ItemData(548, 1, "力量之魂"),
                new ItemData(549, 1, "视域之魂"),
                new ItemData(3856, 1, "飞眼怪蛋"),
                new ItemData(3835, 1, "瞌睡章鱼"),
                new ItemData(3836, 1, "恐怖关刀"),
                new ItemData(3854, 1, "幽灵凤凰"),
                new ItemData(3823, 1, "地狱烙印"),
                new ItemData(3852, 1, "无限智慧巨著"),
                new ItemData(3797, 1, "学徒帽"),
                new ItemData(3798, 1, "学徒长袍"),
                new ItemData(3799, 1, "学徒裤"),
                new ItemData(3800, 1, "侍卫大头盔"),
                new ItemData(3801, 1, "侍卫板甲"),
                new ItemData(3802, 1, "侍卫护胫"),
                new ItemData(3803, 1, "女猎人假发"),
                new ItemData(3804, 1, "女猎人上衣"),
                new ItemData(3805, 1, "女猎人裤"),
                new ItemData(3811, 1, "女猎人圆盾"),
                new ItemData(3806, 1, "武僧浓眉秃头帽"),
                new ItemData(3807, 1, "武僧衣"),
                new ItemData(3808, 1, "武僧裤"),
                new ItemData(3812, 1, "武僧腰带"),
            };
            #endregion

            #region 三王前
            MechBoss = new List<ItemData> {
                new ItemData(935, 1, "复仇者徽章"),
                new ItemData(2220, 1, "天界徽章"),
                new ItemData(936, 1, "机械手套"),
                new ItemData(1343, 1, "烈火手套"),
                new ItemData(674, 1, "原版断钢剑"),
                new ItemData(675, 1, "原版永夜刃"),
                new ItemData(990, 1, "镐斧"),
                new ItemData(579, 1, "斧钻"),
                new ItemData(947, 1, "叶绿矿"),
                new ItemData(1006, 1, "叶绿锭"),
                new ItemData(1001, 1, "叶绿面具"),
                new ItemData(1002, 1, "叶绿头盔"),
                new ItemData(1003, 1, "叶绿头饰"),
                new ItemData(1004, 1, "叶绿板甲"),
                new ItemData(1005, 1, "叶绿护胫"),
                new ItemData(1229, 1, "叶绿连弩"),
                new ItemData(1230, 1, "叶绿镐"),
                new ItemData(1231, 1, "叶绿钻头"),
                new ItemData(1235, 1, "叶绿箭"),
                new ItemData(1179, 1, "叶绿弹"),
                new ItemData(1316, 1, "海龟头盔"),
                new ItemData(1317, 1, "海龟铠甲"),
                new ItemData(1318, 1, "海龟护腿"),
                new ItemData(5382, 1, "华夫饼烘烤模"),
            };
            #endregion

            #region 世纪之花前
            PlantBoss = new List<ItemData> {
                new ItemData(3328, 1, "宝藏袋（世纪之花）"),
                new ItemData(1958, 1, "调皮礼物"),
                new ItemData(1844, 1, "南瓜月勋章"),
                new ItemData(4961, 1, "七彩草蛉"),
                new ItemData(3336, 1, "孢子囊"),
                new ItemData(963, 1, "黑腰带"),
                new ItemData(984, 1, "忍者大师装备"),
                new ItemData(977, 1, "分趾厚底袜"),
                new ItemData(3292, 1, "克苏鲁之眼"),
                new ItemData(3291, 1, "克拉肯球"),
                new ItemData(1178, 1, "吹叶机"),
                new ItemData(3105, 1, "毒气瓶"),
                new ItemData(3106, 1, "变态人的刀"),
                new ItemData(2770, 1, "蛾怪之翼"),
                new ItemData(1871, 1, "喜庆之翼"),
                new ItemData(1183, 1, "妖灵瓶"),
                new ItemData(4679, 1, "晨星"),
                new ItemData(4680, 1, "暗黑收割"),
                new ItemData(1444, 1, "暗影束法杖"),
                new ItemData(1445, 1, "狱火叉"),
                new ItemData(1446, 1, "幽灵法杖"),
                new ItemData(3249, 1, "致命球法杖"),
                new ItemData(1305, 1, "斧"),
                new ItemData(757, 1, "泰拉刃"),
                new ItemData(1569, 1, "吸血鬼刀"),
                new ItemData(1571, 1, "腐化者之戟"),
                new ItemData(1572, 1, "寒霜九头蛇法杖"),
                new ItemData(1156, 1, "食人鱼枪"),
                new ItemData(1260, 1, "彩虹枪"),
                new ItemData(4607, 1, "沙漠虎杖"),
                new ItemData(1508, 1, "灵气"),
                new ItemData(1946, 1, "雪人炮"),
                new ItemData(1947, 1, "北极"),
                new ItemData(1931, 1, "暴雪法杖"),
                new ItemData(1928, 1, "圣诞树剑"),
                new ItemData(1929, 1, "链式机枪"),
                new ItemData(1930, 1, "剃刀松"),
                new ItemData(3997, 1, "冰冻护盾"),
                new ItemData(1552, 1, "蘑菇矿锭"),
                new ItemData(1546, 1, "蘑菇矿头饰"),
                new ItemData(1547, 1, "蘑菇矿面具"),
                new ItemData(1548, 1, "蘑菇矿头盔"),
                new ItemData(1549, 1, "蘑菇矿胸甲"),
                new ItemData(1550, 1, "蘑菇矿护腿"),
                new ItemData(1866, 1, "悬浮板"),
                new ItemData(1570, 1, "断裂英雄剑"),
                new ItemData(823, 1, "幽灵之翼"),
                new ItemData(1503, 1, "幽灵兜帽"),
                new ItemData(1504, 1, "幽灵长袍"),
                new ItemData(1505, 1, "幽灵裤"),
                new ItemData(1506, 1, "幽灵镐"),
                new ItemData(3261, 1, "幽灵锭"),
                new ItemData(1729, 1, "阴森木"),
                new ItemData(1830, 1, "阴森之翼"),
                new ItemData(1832, 1, "阴森头盔"),
                new ItemData(1833, 1, "阴森胸甲"),
                new ItemData(1834, 1, "阴森护腿"),
                new ItemData(1802, 1, "乌鸦法杖"),
                new ItemData(1801, 1, "蝙蝠权杖"),
                new ItemData(4444, 1, "女巫扫帚"),
                new ItemData(1157, 1, "矮人法杖"),
                new ItemData(1159, 1, "提基面具"),
                new ItemData(1160, 1, "提基衣"),
                new ItemData(1161, 1, "提基裤"),
                new ItemData(1162, 1, "叶之翼"),
                new ItemData(1845, 1, "死灵卷轴"),
                new ItemData(1864, 1, "甲虫莎草纸"),
                new ItemData(1339, 1, "小毒液瓶"),
                new ItemData(1342, 1, "毒液弹"),
                new ItemData(1341, 1, "毒液箭"),
                new ItemData(1340, 1, "毒液瓶"),
                new ItemData(1255, 1, "维纳斯万能枪"),
                new ItemData(3107, 1, "钉枪"),
                new ItemData(3108, 1, "钉子"),
                new ItemData(1782, 1, "玉米糖步枪"),
                new ItemData(1783, 1, "玉米糖"),
                new ItemData(1910, 1, "精灵熔枪"),
                new ItemData(1300, 1, "步枪瞄准镜"),
                new ItemData(1254, 1, "狙击步枪"),
                new ItemData(760, 1, "感应雷发射器"),
                new ItemData(759, 1, "火箭发射器"),
                new ItemData(758, 1, "榴弹发射器"),
                new ItemData(1784, 1, "杰克南瓜灯发射器"),
                new ItemData(1785, 1, "爆炸杰克南瓜灯"),
                new ItemData(1835, 1, "尖桩发射器"),
                new ItemData(1836, 1, "尖桩"),
                new ItemData(4457, 1, "初级迷你核弹"),
                new ItemData(4458, 1, "二级迷你核弹"),
                new ItemData(771, 1, "初级火箭"),
                new ItemData(772, 1, "二级火箭"),
                new ItemData(773, 1, "三级火箭"),
                new ItemData(774, 1, "四级火箭"),
                new ItemData(4445, 1, "初级集束火箭"),
                new ItemData(4446, 1, "二级集束火箭"),
                new ItemData(4447, 1, "湿火箭"),
                new ItemData(4448, 1, "熔岩火箭"),
                new ItemData(4449, 1, "蜂蜜火箭"),
                new ItemData(4459, 1, "干火箭"),
                new ItemData(1259, 1, "花冠"),
                new ItemData(3018, 1, "种子弯刀"),
                new ItemData(1826, 1, "无头骑士剑"),
                new ItemData(1513, 1, "圣骑士锤"),
                new ItemData(938, 1, "圣骑士护盾"),
                new ItemData(3998, 1, "英雄护盾"),
                new ItemData(1327, 1, "死神镰刀"),
                new ItemData(4812, 1, "南瓜香薰蜡烛"),
                new ItemData(1301, 1, "毁灭者徽章"),
                new ItemData(4005, 1, "侦察镜"),
            };
            #endregion

            #region 猪鲨前
            Fishron = new List<ItemData> {
                new ItemData(3330, 1, "宝藏袋（猪龙鱼公爵）"),
                new ItemData(2609, 1, "猪龙鱼之翼"),
                new ItemData(2622, 1, "利刃台风"),
                new ItemData(2624, 1, "海啸"),
                new ItemData(2621, 1, "暴风雨法杖"),
                new ItemData(2611, 1, "猪鲨链球"),
                new ItemData(3367, 1, "虾松露")
            };
            #endregion

            #region 光女前
            EmpressOfLight = new List<ItemData> {
                new ItemData(4782, 1, "宝藏袋（光之女皇）"),
                new ItemData(4823, 1, "女皇之翼"),
                new ItemData(4715, 1, "星星吉他"),
                new ItemData(4914, 1, "万花筒"),
                new ItemData(5005, 1, "泰拉棱镜"),
                new ItemData(4989, 1, "翱翔徽章"),
                new ItemData(4923, 1, "星光"),
                new ItemData(4952, 1, "夜光"),
                new ItemData(4953, 1, "日暮"),
                new ItemData(4811, 1, "光之珠宝"),
            };
            #endregion

            #region 石巨人前
            GolemBoss = new List<ItemData> {
                new ItemData(3329, 1, "宝藏袋（石巨人）"),
                new ItemData(3860, 1, "宝藏袋（双足翼龙）"),
                new ItemData(4807, 1, "石巨人守卫"),
                new ItemData(1248, 1, "石巨人之眼"),
                new ItemData(3337, 1, "闪亮石"),
                new ItemData(1294, 1, "锯刃镐"),
                new ItemData(1122, 1, "疯狂飞斧"),
                new ItemData(1295, 1, "高温射线枪"),
                new ItemData(1296, 1, "大地法杖"),
                new ItemData(1297, 1, "石巨人之拳"),
                new ItemData(3110, 1, "天界壳"),
                new ItemData(1865, 1, "天界石"),
                new ItemData(899, 1, "太阳石"),
                new ItemData(2218, 1, "甲虫外壳"),
                new ItemData(2199, 1, "甲虫头盔"),
                new ItemData(2200, 1, "甲虫铠甲"),
                new ItemData(2201, 1, "甲虫壳"),
                new ItemData(2202, 1, "甲虫护腿"),
                new ItemData(2280, 1, "甲虫之翼"),
                new ItemData(948, 1, "蒸汽朋克之翼"),
                new ItemData(3871, 1, "英灵殿骑士头盔"),
                new ItemData(3872, 1, "英灵殿骑士胸甲"),
                new ItemData(3873, 1, "英灵殿骑士护胫"),
                new ItemData(3874, 1, "暗黑艺术家帽子"),
                new ItemData(3875, 1, "暗黑艺术家长袍"),
                new ItemData(3876, 1, "暗黑艺术家护腿"),
                new ItemData(3877, 1, "红色骑术兜帽"),
                new ItemData(3878, 1, "红色骑术服"),
                new ItemData(3879, 1, "红色骑术护腿"),
                new ItemData(3880, 1, "渗透忍者头盔"),
                new ItemData(3882, 1, "渗透忍者裤装"),
                new ItemData(1258, 1, "毒刺发射器"),
                new ItemData(2797, 1, "外星霰弹枪"),
                new ItemData(2882, 1, "带电爆破炮"),
                new ItemData(2769, 1, "宇宙车钥匙"),
                new ItemData(2880, 1, "波涌之刃"),
                new ItemData(2795, 1, "激光机枪"),
                new ItemData(2749, 1, "外星法杖"),
                new ItemData(2796, 1, "电圈发射器"),
                new ItemData(3883, 1, "双足翼龙之翼"),
                new ItemData(3870, 1, "双足翼龙怒气"),
                new ItemData(3859, 1, "空中祸害"),
                new ItemData(3858, 1, "天龙之怒"),
                new ItemData(3827, 1, "飞龙"),
                new ItemData(1261, 1, "毒刺矢"),
            };
            #endregion

            #region 拜月前
            AncientCultist = new List<ItemData> {
                new ItemData(3456, 1, "星璇碎片"),
                new ItemData(3457, 1, "星云碎片"),
                new ItemData(3458, 1, "日耀碎片"),
                new ItemData(3459, 1, "星尘碎片"),
                new ItemData(3473, 1, "日耀喷发剑"),
                new ItemData(3543, 1, "破晓之光"),
                new ItemData(3474, 1, "星尘细胞法杖"),
                new ItemData(3531, 1, "星尘之龙法杖"),
                new ItemData(3475, 1, "星旋机枪"),
                new ItemData(3540, 1, "幻影弓"),
                new ItemData(3542, 1, "星云烈焰"),
                new ItemData(3476, 1, "星云奥秘"),
                new ItemData(3549, 1, "远古操纵机"),
                new ItemData(3544, 1, "超级治疗药水"),
            };
            #endregion

            #region 月前
            Moonlord = new List<ItemData> {
                new ItemData(3332, 1, "宝藏袋（月亮领主）"),
                new ItemData(3601, 1, "天界符"),
                new ItemData(3460, 1, "夜明矿"),
                new ItemData(3467, 1, "夜明锭"),
                new ItemData(3567, 1, "夜明弹"),
                new ItemData(3568, 1, "夜明箭"),
                new ItemData(2757, 1, "星旋头盔"),
                new ItemData(2758, 1, "星旋胸甲"),
                new ItemData(2759, 1, "星旋护腿"),
                new ItemData(3469, 1, "星旋强化翼"),
                new ItemData(3381, 1, "星尘头盔"),
                new ItemData(3382, 1, "星尘板甲"),
                new ItemData(3383, 1, "星尘护腿"),
                new ItemData(3470, 1, "星云斗篷"),
                new ItemData(2760, 1, "星云头盔"),
                new ItemData(2761, 1, "星云胸甲"),
                new ItemData(2762, 1, "星云护腿"),
                new ItemData(3471, 1, "星尘之翼"),
                new ItemData(2763, 1, "耀斑头盔"),
                new ItemData(2764, 1, "耀斑胸甲"),
                new ItemData(2765, 1, "耀斑护腿"),
                new ItemData(3468, 1, "日耀之翼"),
                new ItemData(3466, 1, "星尘镐"),
                new ItemData(2776, 1, "星旋镐"),
                new ItemData(2781, 1, "星云镐"),
                new ItemData(2786, 1, "耀斑镐斧"),
                new ItemData(2774, 1, "星旋钻头"),
                new ItemData(2779, 1, "星云钻头"),
                new ItemData(2784, 1, "耀斑钻头"),
                new ItemData(3464, 1, "星尘钻头"),
                new ItemData(2768, 1, "钻头控制装置"),
                new ItemData(3384, 1, "传送枪"),
                new ItemData(1131, 1, "重力球"),
                new ItemData(4954, 1, "天界星盘"),
                new ItemData(4956, 1, "天顶剑"),
                new ItemData(3065, 1, "狂星之怒"),
                new ItemData(3063, 1, "彩虹猫之刃"),
                new ItemData(1553, 1, "太空海豚机枪"),
                new ItemData(3930, 1, "喜庆弹射器Mk2"),
                new ItemData(3570, 1, "月耀"),
                new ItemData(3389, 1, "泰拉悠悠球"),
                new ItemData(3541, 1, "终极棱镜"),
                new ItemData(3569, 1, "月亮传送门法杖"),
                new ItemData(3571, 1, "七彩水晶法杖"),
                new ItemData(5335, 1, "和谐传送杖"),
                new ItemData(5364, 1, "无底微光桶"),
            };
            #endregion
        }
        #endregion

        #region 超进度物品
        public List<ItemData> Current()
        {
            List<ItemData> list = new();
            if (!NPC.downedSlimeKing) list.AddRange(SlimeKing);
            if (!NPC.downedGoblins) list.AddRange(Goblins);
            if (!NPC.downedBoss1) list.AddRange(Boss1);
            if (!NPC.downedDeerclops) list.AddRange(Deerclops);
            if (!NPC.downedBoss2) list.AddRange(Boss2);
            if (!NPC.downedQueenBee) list.AddRange(QueenBee);
            if (!NPC.downedBoss3) list.AddRange(Boss3);
            if (!Main.hardMode) list.AddRange(hardMode);
            if (!NPC.downedQueenSlime) list.AddRange(QueenSlime);
            if (!NPC.downedMechBossAny) list.AddRange(MechBossAny);
            if (!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) list.AddRange(MechBoss);
            if (!NPC.downedFishron) list.AddRange(Fishron);
            if (!NPC.downedEmpressOfLight) list.AddRange(EmpressOfLight);
            if (!NPC.downedPlantBoss) list.AddRange(PlantBoss);
            if (!NPC.downedGolemBoss) list.AddRange(GolemBoss);
            if (!NPC.downedAncientCultist) list.AddRange(AncientCultist);
            if (!NPC.downedMoonlord) list.AddRange(Moonlord);
            return list;
        }
        #endregion

        #region 违规物品是否为空
        public bool IsEmpty()
        {
            if (
                Goblins.Count +
                SlimeKing.Count +
                Boss1.Count +
                Deerclops.Count +
                Boss2.Count +
                QueenBee.Count +
                Boss3.Count +
                hardMode.Count +
                QueenSlime.Count +
                MechBossAny.Count +
                MechBoss.Count +
                Fishron.Count +
                EmpressOfLight.Count +
                PlantBoss.Count +
                GolemBoss.Count +
                AncientCultist.Count +
                Moonlord.Count > 0)
            {
                return false;
            }
            return true;
        }
        #endregion



        #region 加载配置文件方法
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
            }
        }

        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
            {
                var defaultConfig = new Configuration();
                defaultConfig.Init(); // 在写入之前初始化新配置对象
                defaultConfig.Write(path);
                return defaultConfig;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf;
                }
            }
        }

        #endregion


    }

    #region 物品数据
    public class ItemData
    {
        public int id = 0;

        public int 数量 = 1;

        public string 名称 = "";

        public ItemData()
        {
        }

        public ItemData(int _id, int stack, string name = "")
        {
            id = _id;
            数量 = stack;
            名称 = name;
        }
    }
    #endregion
}