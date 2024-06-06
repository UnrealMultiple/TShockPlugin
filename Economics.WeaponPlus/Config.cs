using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.WeaponPlus
{
    //[JsonConverter(typeof(ConfigConverter))]
    public class Config
    {
        [JsonIgnore]
        public static readonly string configPath = Path.Combine(TShock.SavePath, "WeaponPlus.json");

        [JsonProperty("启用英文")]
        public bool EnableEnglish = false;

        [Description("进服时是否开启自动重读武器")]
        public bool WhetherToTurnOnAutomaticLoadWeaponsWhenEnteringTheGame = true;

        [Description("最多升级次数")]
        public int MaximunofLevel = 50;

        [Description("花费参数")]
        public double CostParameters = 1.0;

        [Description("升级花费增加")]
        public double UpgradeCostsIncrease = 0.2;

        [Description("重置武器返还倍数")]
        public double ResetTheWeaponReturnMultiple = 0.5;

        [Description("武器升级攻速上限倍数")]
        public float MaximumUseTimeMultipleOfWeaponUpgrade = 60f;

        [Description("武器升级射弹飞速上限倍数")]
        public float MaximumProjectileSpeedMultipleOfWeaponUpgrade = 3f;

        [Description("武器升级击退上限倍数")]
        public float MaximumKnockBackMultipleOfWeaponUpgrade = 3f;

        [Description("武器升级尺寸上限倍数")]
        public float MaximumScaleMultipleOfWeaponUpgrade = 2.5f;

        [Description("近战武器伤害上限倍数")]
        public float MaximumDamageMultipleOfMeleeWeapons = 1.75f;

        [Description("近战武器升级攻速上限")]
        public int MaximumAttackSpeedOfMeleeWeaponUpgrade = 8;

        [Description("远程武器伤害上限倍数")]
        public float MaximumDamageMultipleOfRangeWeapons = 2f;

        [Description("远程武器升级攻速上限")]
        public int MaximumAttackSpeedOfRangeWeaponUpgrade = 4;

        [Description("魔法武器伤害上限倍数")]
        public float MaximumDamageMultipleOfMagicWeapons = 2.3f;

        [Description("魔法武器升级攻速上限")]
        public int MaximumAttackSpeedOfMagicWeaponUpgrade = 6;

        [Description("召唤武器伤害上限倍数")]
        public float MaximumDamageMultipleOfSummonWeapons = 2.5f;

        [Description("召唤武器升级攻速上限")]
        public int MaximumAttackSpeedOfSummonWeaponUpgrade = 8;

        [Description("其他武器伤害上限倍数")]
        public float MaximumDamageMultipleOfOtherWeapons = 2f;

        [Description("其他武器升级攻速上限")]
        public int MaximumAttackSpeedOfOtherWeaponUpgrade = 8;

        #region 读取与创建配置文件方法
        public void Write()
        {
            using (var fs = new FileStream(configPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        public static Config Read()
        {
            if (!File.Exists(configPath))
            {
                var c = new Config();
                c.Write();
                return c;
            }
            else
            {
                using (var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Config>(json);
                    return cf!;
                }
            }
        }
        #endregion
    }
}