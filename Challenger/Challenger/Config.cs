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

        [JsonProperty("吸血比率")]
        public float BloodAbsorptionRatio = 0.25f;

        [JsonProperty("吸血比率对于Boss")]
        public float BloodAbsorptionRatioForBoss = 0.5f;

        [JsonProperty("启用话痨模式")]
        public bool EnableConsumptionMode = false;

        [JsonProperty("启用广播话痨模式")]
        public bool EnableBroadcastConsumptionMode = false;

        [JsonProperty("所有怪物血量倍数")]
        public float lifeXnum = 1.00f;

        [JsonProperty("皇家凝胶是否下凝胶雨")]
        public bool RoyalGel = true;

        [JsonProperty("蠕虫围巾还能免疫什么DeBuff")]
        public int WormScarf = 0;

        [JsonProperty("蜜蜂背包是否扔毒蜂罐")]
        public bool HivePack = true;

        [JsonProperty("化石套是否出琥珀光球")]
        public bool FossilArmorEffect = true;

        [JsonProperty("丛林套是否环绕伤害孢子")]
        public bool JungleArmorEffect = true;

        [JsonProperty("忍者套是否会闪避")]
        public bool NinjaArmorEffect = true;

        [JsonProperty("流星套是否下落星")]
        public bool MeteorArmorEffect = true;

        [JsonProperty("蜜蜂套是否撒蜂糖罐")]
        public bool BeeArmorEffect = true;

        [JsonProperty("死灵套是否产生额外弹幕")]
        public bool NecroArmor = true;

        [JsonProperty("黑曜石套是否盗窃双倍掉落物")]
        public bool ObsidianArmorEffect = true;

        [JsonProperty("狱岩套给什么永久BUFF")]
        public int MoltenArmor = 116;

        [JsonProperty("水晶刺客套是否释放水晶碎片")]
        public bool CrystalAssassinArmorEffect = true;

        [JsonProperty("禁戒套是否释放灵焰魂火")]
        public bool ForbiddenArmorEffect = true;

        [JsonProperty("寒霜套是否下北极弹幕")]
        public bool FrostArmorEffect = true;

        [JsonProperty("神圣套额外弹幕多少伤害/默认55%")]
        public double HallowedArmorEffect = 0.55;

        [JsonProperty("叶绿套加多少血")]
        public int ChlorophyteArmorEffect = 100;

        [JsonProperty("海龟套加多少血")]
        public int TurtleArmorEffect = 60;

        [JsonProperty("提基套加多少血")]
        public int TikiArmorEffect = 20;

        [JsonProperty("阴森套是否出南瓜弹幕")]
        public bool SpookyArmorEffect = true;

        [JsonProperty("蘑菇套是否产蘑菇")]
        public bool ShroomiteArmorEffect = true;

        [JsonProperty("幽灵套加多少血和魔力")]
        public int SpectreArmorEffect = 60;


        [JsonProperty("甲虫套加多少血")]
        public int BeetleArmorEffect_1 = 60;

        [JsonProperty("甲虫套带骑士盾时给圣锤加多少伤害/默认90%")]
        public float BeetleArmorEffect_2 = 0.9f;




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