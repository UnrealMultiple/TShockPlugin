using Newtonsoft.Json;
using System.Text;

namespace TerrariaMap;

[JsonConverter(typeof(ConfigConverter))]
public class Config
{
    [JsonIgnore]
    public static readonly string configPath = Path.Combine(Economics.Core.Economics.SaveDirPath, "WeaponPlus.json");

    [JsonProperty("启用英文", Order = -6)]
    public bool EnableEnglish = false;
    [JsonProperty("进服时是否开启自动重读武器", Order = -6)]
    public bool WhetherToTurnOnAutomaticLoadWeaponsWhenEnteringTheGame = true;
    [JsonProperty("最多升级次数", Order = -6)]
    public int MaximunofLevel = 50;

    [JsonProperty("花费参数", Order = -5)]
    public double CostParameters = 1.0;
    [JsonProperty("升级花费增加", Order = -5)]
    public double UpgradeCostsIncrease = 0.2;
    [JsonProperty("重置武器返还倍数", Order = -5)]
    public double ResetTheWeaponReturnMultiple = 0.5;

    [JsonProperty("武器升级攻速上限倍数", Order = -4)]
    public float MaximumUseTimeMultipleOfWeaponUpgrade = 60f;
    [JsonProperty("武器升级射弹飞速上限倍数", Order = -4)]
    public float MaximumProjectileSpeedMultipleOfWeaponUpgrade = 3f;
    [JsonProperty("武器升级击退上限倍数", Order = -4)]
    public float MaximumKnockBackMultipleOfWeaponUpgrade = 3f;
    [JsonProperty("武器升级尺寸上限倍数", Order = -4)]
    public float MaximumScaleMultipleOfWeaponUpgrade = 2.5f;

    [JsonProperty("近战武器伤害上限倍数", Order = -3)]
    public float MaximumDamageMultipleOfMeleeWeapons = 1.75f;
    [JsonProperty("近战武器升级攻速上限", Order = -3)]
    public int MaximumAttackSpeedOfMeleeWeaponUpgrade = 8;

    [JsonProperty("远程武器伤害上限倍数", Order = -2)]
    public float MaximumDamageMultipleOfRangeWeapons = 2f;
    [JsonProperty("远程武器升级攻速上限", Order = -2)]
    public int MaximumAttackSpeedOfRangeWeaponUpgrade = 4;

    [JsonProperty("魔法武器伤害上限倍数", Order = -1)]
    public float MaximumDamageMultipleOfMagicWeapons = 2.3f;
    [JsonProperty("魔法武器升级攻速上限", Order = -1)]
    public int MaximumAttackSpeedOfMagicWeaponUpgrade = 6;

    [JsonProperty("召唤武器伤害上限倍数", Order = 1)]
    public float MaximumDamageMultipleOfSummonWeapons = 2.5f;
    [JsonProperty("召唤武器升级攻速上限", Order = 1)]
    public int MaximumAttackSpeedOfSummonWeaponUpgrade = 8;

    [JsonProperty("其他武器伤害上限倍数", Order = 2)]
    public float MaximumDamageMultipleOfOtherWeapons = 2f;
    [JsonProperty("其他武器升级攻速上限", Order = 2)]
    public int MaximumAttackSpeedOfOtherWeaponUpgrade = 8;

    [JsonProperty("花费货币")]
    public string Currency = "魂力";

    #region 读取与创建配置文件方法
    public void Write(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            sw.Write(str);
        }
    }

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
}