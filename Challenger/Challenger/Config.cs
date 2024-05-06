using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace Challenger
{
    public class Config
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "ChallengerConfig.json");

        [JsonProperty("是否启用挑战模式")]
        public bool enableChallenge = true;
        [JsonProperty("是否启用BOSS魔改")]
        public bool enableBossAI = false;

        [JsonProperty("是否启用怪物吸血")]
        public bool enableMonsterSucksBlood = true;
        [JsonProperty("怪物吸血比率")]
        public float BloodAbsorptionRatio = 0.25f;
        [JsonProperty("怪物回血上限：小怪>1.5倍则会消失")]
        public float BloodAbsorptionRatio_Max { get;  set; } = 1.5f;
        [JsonProperty("怪物吸血比率对于Boss")]
        public float BloodAbsorptionRatioForBoss = 0.5f;

        [JsonProperty("启用话痨模式")]
        public bool EnableConsumptionMode = false;
        [JsonProperty("启用广播话痨模式")]
        public bool EnableBroadcastConsumptionMode = false;

        [JsonProperty("所有怪物血量倍数(仅开启魔改BOSS时生效)")]
        public float lifeXnum = 1.00f;

        [JsonProperty("蜜蜂背包是否扔毒蜂罐")]
        public bool HivePack = true;
        [JsonProperty("蜜蜂背包首次弹幕ID")]
        public int HivePack_1 = 346;
        [JsonProperty("蜜蜂背包首次弹幕伤害")]
        public int HivePack_2 = 0;
        [JsonProperty("蜜蜂背包首次弹幕击退")]
        public float HivePack_3 = 0f;
        [JsonProperty("蜜蜂背包弹幕爆炸后的弹幕ID")]
        public int HivePack_4 = 566;
        [JsonProperty("蜜蜂背包弹幕爆炸后的弹幕伤害")]
        public int HivePack_5 = 30;
        [JsonProperty("蜜蜂背包弹幕爆炸后的弹幕击退")]
        public float HivePack_6 = 0f;

        [JsonProperty("化石套是否出琥珀光球")]
        public bool FossilArmorEffect = true;
        [JsonProperty("化石套的弹幕ID")]
        public int FossilArmorEffect_0 = 732;
        [JsonProperty("化石套的弹幕射程")]
        public float FossilArmorEffect_1 = 18f;
        [JsonProperty("化石套的弹幕伤害")]
        public int FossilArmorDamage = 10;
        [JsonProperty("化石套的弹幕击退")]
        public float FossilArmorEffect_2 = 8f;

        [JsonProperty("钓鱼套包含哪些永久BUFF")]
        public int[] AnglerArmorEffectList { get; set; } = new int[] { 106, 123, 121, 122 };

        [JsonProperty("丛林套是否环绕伤害孢子")]
        public bool JungleArmorEffect = true;
        [JsonProperty("丛林套弹幕射程/速率")]
        public float JungleArmorEffect_2 = 1.5f;
        [JsonProperty("丛林套弹幕ID1")]
        public int JungleArmorEffect_3 = 569;
        [JsonProperty("丛林套弹幕ID2")]
        public int JungleArmorEffect_4 = 572;
        [JsonProperty("丛林套弹幕伤害")]
        public int JungleArmorEffect_5 = 15;
        [JsonProperty("丛林套弹幕击退")]
        public float JungleArmorEffect_6 = 8f;

        [JsonProperty("忍者套是否会闪避")]
        public bool NinjaArmorEffect = true;
        [JsonProperty("忍者套闪避概率随机数/0则100%闪避")]
        public int NinjaArmorEffect_2 = 4;
        [JsonProperty("忍者套闪避释放的弹幕ID")]
        public int NinjaArmorEffect_3 = 196;
        [JsonProperty("忍者套闪避释放的弹幕伤害")]
        public int NinjaArmorEffect_4 = 0;
        [JsonProperty("忍者套闪避释放的弹幕击退")]
        public float NinjaArmorEffect_5 = 0f;

        [JsonProperty("暗影套的弹幕ID")]
        public int ShadowArmorEffect = 307;
        [JsonProperty("暗影套的弹幕伤害")]
        public int ShadowArmorEffect_2 = 20;
        [JsonProperty("暗影套的弹幕击退")]
        public float ShadowArmorEffect_3 = 2f;

        [JsonProperty("猩红套的弹幕ID")]
        public int CrimsonArmorEffect = 305;
        [JsonProperty("猩红套的弹幕伤害")]
        public int CrimsonArmorEffect_2 = 0;
        [JsonProperty("猩红套的弹幕击退")]
        public float CrimsonArmorEffect_3 = 0f;


        [JsonProperty("流星套是否下落星")]
        public bool MeteorArmorEffect = true;
        [JsonProperty("流星套的弹幕ID")]
        public int MeteorArmorEffect_2 = 725;
        [JsonProperty("流星套的弹幕射程")]
        public int MeteorArmorEffect_3 = 1000;
        [JsonProperty("流星套的弹幕速度")]
        public float MeteorArmorEffect_4 = 16f;

        [JsonProperty("蜜蜂套是否撒蜂糖罐")]
        public bool BeeArmorEffect = true;
        [JsonProperty("蜜蜂套给什么永久BUFF")]
        public int[] BeeArmorEffectList { get; set; } = new int[] { 48 };
        [JsonProperty("蜜蜂套的BUFF时长")]
        public int BeeArmorEffectTime = 150;

        [JsonProperty("死灵套是否产生额外弹幕")]
        public bool NecroArmor = true;
        [JsonProperty("死灵套受到攻击时的弹幕ID")]
        public int NecroArmor_2 = 532;
        [JsonProperty("死灵套受到攻击时的弹幕伤害")]
        public int NecroArmor_3 = 20;
        [JsonProperty("死灵套受到攻击时的弹幕击退")]
        public float NecroArmor_4 = 5f;
        [JsonProperty("死灵套攻击时的弹幕ID")]
        public int NecroArmor_5 = 117;
        [JsonProperty("死灵套攻击时的弹幕伤害")]
        public int NecroArmor_6 = 20;
        [JsonProperty("死灵套攻击时的弹幕击退")]
        public float NecroArmor_7 = 2f;

        [JsonProperty("黑曜石套是否盗窃双倍掉落物")]
        public bool ObsidianArmorEffect = true;
        [JsonProperty("黑曜石套盗窃的稀有等级")]
        public int ObsidianArmorEffect_1 = 1;

        [JsonProperty("水晶刺客套是否释放水晶碎片")]
        public bool CrystalAssassinArmorEffect = true;
        [JsonProperty("水晶刺客套遇怪自动释放的弹幕ID")]
        public int CrystalAssassinArmorEffect_2 = 90;
        [JsonProperty("水晶刺客套受伤释放的弹幕ID")]
        public int CrystalAssassinArmorEffect_3 = 94;

        [JsonProperty("蜘蛛套给NPC施加什么BUFF1")]
        public int SpiderArmorEffect = 70;
        [JsonProperty("蜘蛛套给NPC施加BUFF1时长")]
        public int SpiderArmorEffect_2 = 180;
        [JsonProperty("蜘蛛套给NPC施加什么BUFF2")]
        public int SpiderArmorEffect_3 = 20;
        [JsonProperty("蜘蛛套给NPC施加BUFF2时长")]
        public int SpiderArmorEffect_4 = 360;

        [JsonProperty("禁戒套是否释放灵焰魂火")]
        public bool ForbiddenArmorEffect = true;
        [JsonProperty("禁戒套释放什么弹幕ID")]
        public int ForbiddenArmorEffect_2 = 659;
        [JsonProperty("禁戒套的弹幕伤害")]
        public int ForbiddenArmorEffect_3 = 45;

        [JsonProperty("寒霜套是否下北极弹幕")]
        public bool FrostArmorEffect = true;
        [JsonProperty("寒霜套释放什么弹幕ID")]
        public int FrostArmorEffect_2 = 344;
        [JsonProperty("寒霜套的弹幕伤害")]
        public int FrostArmorEffect_3 = 50;
        [JsonProperty("寒霜套的弹幕击退")]
        public float FrostArmorEffect_4 = 0f;

        [JsonProperty("神圣套额外弹幕多少伤害/默认55%")]
        public double HallowedArmorEffect = 0.55;
        [JsonProperty("神圣套释放什么弹幕ID")]
        public int HallowedArmorEffect_2 = 156;
        [JsonProperty("神圣套释放什么弹幕ID2")]
        public int HallowedArmorEffect_3 = 157;

        [JsonProperty("叶绿套加多少生命上限")]
        public int ChlorophyteArmorEffect = 100;

        [JsonProperty("海龟套加多少生命上限")]
        public int TurtleArmorEffect = 60;
        [JsonProperty("海龟套的弹幕ID")]
        public int TurtleArmorEffect_2 = 249;
        [JsonProperty("海龟套的弹幕伤害")]
        public int TurtleArmorEffect_3 = 60;
        [JsonProperty("海龟套的弹幕间隔/默认3秒")]
        public int TurtleArmorEffect_4 = 3;

        [JsonProperty("提基套加多少生命上限")]
        public int TikiArmorEffect = 20;
        [JsonProperty("提基套的弹幕ID")]
        public int TikiArmorEffect_2 = 228;
        [JsonProperty("提基套的弹幕伤害")]
        public float TikiArmorEffect_3 = 0.4f;

        [JsonProperty("阴森套是否出弹幕")]
        public bool SpookyArmorEffect = true;
        [JsonProperty("阴森套白天出什么弹幕")]
        public int SpookyArmorEffect_2 = 316;
        [JsonProperty("阴森套晚上出什么弹幕")]
        public int SpookyArmorEffect_3 = 321;
        [JsonProperty("阴森套弹幕伤害/默认20%")]
        public double SpookyArmorEffect_4 = 0.2;

        [JsonProperty("蘑菇套是否产蘑菇")]
        public bool ShroomiteArmorEffect = true;
        [JsonProperty("蘑菇套的弹幕ID")]
        public int ShroomiteArmorEffect_1 = 131;
        [JsonProperty("蘑菇套的弹幕伤害倍数")]
        public float ShroomiteArmorEffect_2 = 0.32f;
        [JsonProperty("蘑菇套的弹幕击退")]
        public float ShroomiteArmorEffect_3 = 1.14514f;

        [JsonProperty("幽灵套加多少生命和魔力上限")]
        public int SpectreArmorEffect = 60;
        [JsonProperty("幽灵兜帽是否出幽灵弹幕")]
        public bool EnableSpectreArmorEffect_1 = true;
        [JsonProperty("幽灵面具是否出幽灵弹幕")]
        public bool EnableSpectreArmorEffect_2 = false;
        [JsonProperty("幽灵套的弹幕ID")]
        public int EnableSpectreArmorEffect_3 = 356;
        [JsonProperty("幽灵套的弹幕伤害倍数")]
        public float EnableSpectreArmorEffect_4 = 120f;
        [JsonProperty("幽灵套的弹幕击退")]
        public float EnableSpectreArmorEffect_5 = 0f;
        [JsonProperty("幽灵套环绕的弹幕ID")]
        public int EnableSpectreArmorEffect_6 = 299;
        [JsonProperty("幽灵套环绕的弹幕伤害")]
        public int EnableSpectreArmorEffect_7 = 0;
        [JsonProperty("幽灵套环绕的弹幕击退")]
        public float EnableSpectreArmorEffect_8 = 0f;

        [JsonProperty("甲虫套加多少生命上限")]
        public int BeetleArmorEffect_1 = 60;
        [JsonProperty("甲虫套受到伤害给其他玩家的回血转换比例/默认30%")]
        public double BeetleArmorEffect_3 = 0.3;
        [JsonProperty("甲虫套减多少回复量/默认为0")]
        public int OtherPlayerHealAmount = 10;
        [JsonProperty("甲虫套带骑士盾时给圣锤加多少伤害/默认90%")]
        public float BeetleArmorEffect_2 = 0.9f;

        [JsonProperty("冲刺饰品类的闪避冷却时间/默认12秒")]
        public int CthulhuShieldTime = 12;

        [JsonProperty("皇家凝胶是否下物品雨")]
        public bool RoyalGel = true;
        [JsonProperty("皇家凝胶物品雨表")]
        public int[] RoyalGelList { get; set; } = new int[] { 23 };

        [JsonProperty("挥发凝胶击中敌怪掉落物品表")]
        public int[] VolatileGelatin { get; set; } = new int[] { 23, 409, 502, 3111 }; 

        [JsonProperty("狱岩套给什么永久BUFF")]
        public int[] MoltenArmor { get; set; } = new int[] { 1, 116 };

        [JsonProperty("挖矿套给什么永久BUFF")]
        public int[] MiningArmor { get; set; } = new int[] { 104, 192  };
        [JsonProperty("挖矿套连锁图格ID表")]
        public int[] Tile { get; set; } = new int[] { 6, 7, 8, 9, 166, 167, 168, 169, 22, 221, 222, 223, 224, 232, 37, 404, 408, 48, 481, 482, 483, 56, 571, 58, 63, 64, 65, 66, 67, 68, 107, 108, 111, 123, 178, 204, 211, 229, 230 };

        [JsonProperty("蠕虫围巾免疫buff是否开启")]
        public bool EnableWormScarf = false;
        [JsonProperty("蠕虫围巾遍历前几个buff")]
        public int WormScarfImmuneList_2 = 22;
        [JsonProperty("蠕虫围巾免疫DeBuff列表/遇到会清空所有BUFF")]
        public int[] WormScarfImmuneList { get; set; } = new int[] { 39, 69, 44, 46 };


        #region 读取与创建配置文件方法

        //创建 写入你 👆 上面的参数
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        // 从文件读取配置
        public static Config Read(string path)
        {
            if (!File.Exists(path))
            {
                var c = new Config();
                c.Write(path);
                return c;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Config>(json);
                    return cf!;
                }
            }
        }
        #endregion

        #region 原配置文件方法
        /*
        public Config()
        {
        }

 
        public static Config LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                Config config = new Config(b1: true, b2: true, 0.25f, 0.5f, b3: false, b4: false, 1.25f,b6: false);
                File.WriteAllText(configPath, JsonConvert.SerializeObject((object)config, (Formatting)1));
                return config;
            }
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
        }


        public Config(bool b1, bool b2, float f1, float f2, bool b3, bool b4, float b5,bool b6)
        {
            enableChallenge = b1;
            enableMonsterSucksBlood = b2;
            BloodAbsorptionRatio = f1;
            BloodAbsorptionRatioForBoss = f2;
            EnableConsumptionMode = b3;
            EnableBroadcastConsumptionMode = b4;
            this.lifeXnum = b5;
            enableBossAI = b6;
        }
        */
        #endregion
    }
}