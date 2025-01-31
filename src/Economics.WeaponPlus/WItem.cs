using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;

namespace Economics.WeaponPlus;

public class WItem
{
    #region 变量
    public int id;

    public string owner;

    public int lable;

    public int damage_level;

    public int scale_level;

    public int knockBack_level;

    public int useSpeed_level;

    public int shootSpeed_level;

    public long allCost;

    public readonly int orig_damage;

    public readonly float orig_scale;

    public readonly float orig_knockBack;

    public readonly int orig_useAnimation;

    public readonly int orig_useTime;

    public readonly float orig_shootSpeed;

    public readonly Color orig_color;

    public readonly int orig_shoot;

    public int Level => this.damage_level + this.scale_level + this.knockBack_level + this.useSpeed_level + this.shootSpeed_level;
    #endregion

    #region 强化物品的结构
    public WItem(int ID, string owner = "")
    {
        var itemById = TShock.Utils.GetItemById(ID);
        this.id = ID;
        this.owner = owner;
        this.damage_level = 0;
        this.scale_level = 0;
        this.knockBack_level = 0;
        this.useSpeed_level = 0;
        this.shootSpeed_level = 0;
        this.allCost = 0L;
        this.orig_damage = itemById.damage;
        this.orig_scale = itemById.scale;
        this.orig_knockBack = itemById.knockBack;
        this.orig_useAnimation = itemById.useAnimation;
        this.orig_useTime = itemById.useTime;
        this.orig_shootSpeed = itemById.shootSpeed;
        this.orig_color = itemById.color;
        this.orig_shoot = itemById.shoot;
    }
    #endregion

    #region 检查数据库
    public void CheckDB()
    {
        if (string.IsNullOrWhiteSpace(this.owner) || this.Level > 0)
        {
            return;
        }
        try
        {
            WeaponPlus.DB.DeleteDB(this.owner, this.id);
            var list = TSPlayer.FindByNameOrID(this.owner);
            if (list.Count != 1)
            {
                return;
            }
            var wPlayer = WeaponPlus.wPlayers[list[0].Index];
            if (wPlayer != null && wPlayer.isActive)
            {
                wPlayer.hasItems.RemoveAll((x) => x.id == this.id);
            }
        }
        catch
        {
        }
    }
    #endregion

    #region 武器信息
    public string ItemMess()
    {
        var stringBuilder = new StringBuilder();
        if (this.Level > 0)
        {
            stringBuilder.AppendLine(GetString($"当前总等级：{this.Level}   剩余强化次数：{WeaponPlus.config.MaximunofLevel - this.Level} 次    伤害等级：{this.damage_level}, 大小等级：{this.scale_level}, 击退等级：{this.knockBack_level}, 攻速等级：{this.useSpeed_level}, 射弹飞行速度等级：{this.shootSpeed_level}"));
        }
        else
        {
            stringBuilder.AppendLine(GetString("未升级过，无任何加成"));
        }

        var damageLevel = Math.Max((int) (this.orig_damage * 0.05f * this.damage_level), this.damage_level);
        var damageBonus = damageLevel * 1f / this.orig_damage;
        var scaleBonus = 0.05f * this.scale_level;
        var knockBackBonus = 0.05f * this.knockBack_level;
        var speedBonus = (this.orig_useAnimation * 1f / (this.orig_useAnimation - this.useSpeed_level)) - 1f;
        var projectileBonus = 0.05f * this.shootSpeed_level;
        stringBuilder.AppendLine(GetString($"当前状态：伤害 +{damageBonus:0.00%}，大小 +{scaleBonus:0.00%}，击退 +{knockBackBonus:0.00%}，攻速 +{speedBonus:0.00%}，射弹飞速 +{projectileBonus:0.00%}"));
        if (this.Level < WeaponPlus.config.MaximunofLevel)
        {
            stringBuilder.AppendLine(GetString("伤害升至下一级需：{0}", this.plusPrice(PlusType.damage, out var price) ? price : GetString("当前已满级")));
            stringBuilder.AppendLine(GetString("大小升至下一级需：{0}", this.plusPrice(PlusType.scale, out price) ? price : GetString("当前已满级")));
            stringBuilder.AppendLine(GetString("击退升至下一级需：{0}", this.plusPrice(PlusType.knockBack, out price) ? price : GetString("当前已满级")));
            stringBuilder.AppendLine(GetString("攻速升至下一级需：{0}", this.plusPrice(PlusType.useSpeed, out price) ? price : GetString("当前已满级")));
            stringBuilder.Append(GetString("射弹飞速升至下一级需：{0}", this.plusPrice(PlusType.shootSpeed, out price) ? price : GetString("当前已满级")));
        }
        else
        {
            stringBuilder.Append(GetString("已达到最大武器总等级"));
        }

        return stringBuilder.ToString();
    }
    #endregion

    #region 强化价格
    public bool plusPrice(PlusType plus, out long price, int gap = 1)
    {
        var itemById = TShock.Utils.GetItemById(this.id);
        price = 0L;
        if (this.Level + gap > WeaponPlus.config.MaximunofLevel)
        {
            return false;
        }
        for (var i = 1; i <= gap; i++)
        {
            double num = itemById.value;
            if (itemById.maxStack == 1 && itemById.value < 5000)
            {
                num = 5000.0;
            }
            else if (itemById.maxStack > 1 && itemById.value == 0)
            {
                num = 5.0;
            }
            switch (itemById.type)
            {
                case 757:
                case 1156:
                case 1260:
                case 1569:
                case 1571:
                case 1572:
                case 2611:
                case 2621:
                case 2622:
                case 2623:
                case 2624:
                case 3827:
                case 3858:
                case 3859:
                case 3870:
                case 4607:
                case 4715:
                case 4914:
                case 4923:
                case 4952:
                case 4953:
                    num = 500000.0;
                    break;
                case 496:
                case 756:
                case 1324:
                    num = 270000.0;
                    break;
                case 561:
                    num = 230000.0;
                    break;
                case 4281:
                    num = 25000.0;
                    break;
                case 98:
                    num = 30000.0;
                    break;
                case 4273:
                case 4381:
                    num = 50000.0;
                    break;
                case 1265:
                case 4703:
                case 4760:
                    num = 125000.0;
                    break;
                case 1327:
                case 2270:
                case 3006:
                case 3007:
                case 3008:
                case 3012:
                case 3013:
                case 3014:
                case 3029:
                case 3030:
                case 3051:
                    num /= 2.0;
                    break;
                case 674:
                case 675:
                case 4348:
                case 5065:
                    num = 350000.0;
                    break;
                case 3389:
                case 3473:
                case 3474:
                case 3475:
                case 3476:
                case 3531:
                case 3540:
                case 3541:
                case 3542:
                case 3543:
                case 3569:
                case 3570:
                case 3571:
                case 3930:
                    num = 650000.0;
                    break;
            }
            var num2 = 2.0;
            var num3 = num / num2;
            switch (plus)
            {
                case PlusType.damage:
                {
                    var num5 = (int) (this.orig_damage * 0.05 * (this.damage_level + i));
                    if (num5 < this.damage_level + i)
                    {
                        num3 = 1.0 / this.orig_damage * (num / (num2 / 20.0));
                        num5 = this.damage_level + i;
                    }
                    var num6 = itemById.melee ? (int) (WeaponPlus.config.MaximumDamageMultipleOfMeleeWeapons * this.orig_damage) : itemById.magic ? (int) (WeaponPlus.config.MaximumDamageMultipleOfMagicWeapons * this.orig_damage) : itemById.ranged ? (int) (WeaponPlus.config.MaximumDamageMultipleOfRangeWeapons * this.orig_damage) : !itemById.summon ? (int) (WeaponPlus.config.MaximumDamageMultipleOfOtherWeapons * this.orig_damage) : (int) (WeaponPlus.config.MaximumDamageMultipleOfSummonWeapons * this.orig_damage);
                    if (num5 + this.orig_damage > num6)
                    {
                        return false;
                    }
                    if (itemById.magic)
                    {
                        num3 *= 0.9;
                    }
                    break;
                }
                case PlusType.scale:
                    if (this.orig_scale + (this.orig_scale * 0.05 * (this.scale_level + i)) > (double) (this.orig_scale * WeaponPlus.config.MaximumScaleMultipleOfWeaponUpgrade))
                    {
                        return false;
                    }
                    if (!itemById.melee)
                    {
                        num3 *= 0.7;
                    }
                    break;
                case PlusType.knockBack:
                    if (this.orig_knockBack + (this.orig_knockBack * 0.05 * (this.knockBack_level + i)) > (double) (this.orig_knockBack * WeaponPlus.config.MaximumKnockBackMultipleOfWeaponUpgrade))
                    {
                        return false;
                    }
                    num3 = !itemById.summon ? num3 * 0.85 : num3 * 0.7;
                    break;
                case PlusType.useSpeed:
                {
                    var num4 = itemById.melee ? WeaponPlus.config.MaximumAttackSpeedOfMeleeWeaponUpgrade : itemById.magic ? WeaponPlus.config.MaximumAttackSpeedOfMagicWeaponUpgrade : itemById.ranged ? WeaponPlus.config.MaximumAttackSpeedOfRangeWeaponUpgrade : !itemById.summon ? WeaponPlus.config.MaximumAttackSpeedOfOtherWeaponUpgrade : WeaponPlus.config.MaximumAttackSpeedOfSummonWeaponUpgrade;
                    if (this.orig_useAnimation - (this.useSpeed_level + i) < num4 || this.orig_useAnimation <= num4)
                    {
                        return false;
                    }
                    if (this.orig_useTime - (this.useSpeed_level + i) < num4 || this.orig_useTime <= num4)
                    {
                        return false;
                    }
                    if (this.orig_useAnimation * 1.0 / (this.orig_useAnimation - (this.useSpeed_level + i)) > WeaponPlus.config.MaximumUseTimeMultipleOfWeaponUpgrade)
                    {
                        return false;
                    }
                    num3 = ((this.orig_useAnimation * 1.0 / (this.orig_useAnimation - (this.useSpeed_level + (i * 1.0)))) - (this.orig_useAnimation * 1.0 / (this.orig_useAnimation - (this.useSpeed_level + i - 1.0)))) * (num / (num2 / 20.0));
                    num3 *= 1.05;
                    if (itemById.summon && itemById.mana > 0)
                    {
                        num3 *= 0.3;
                    }
                    break;
                }
                case PlusType.shootSpeed:
                    if (this.orig_shootSpeed + (this.orig_shootSpeed * 0.05 * (this.shootSpeed_level + i)) > (double) (this.orig_shootSpeed * WeaponPlus.config.MaximumProjectileSpeedMultipleOfWeaponUpgrade))
                    {
                        return false;
                    }
                    break;
            }
            num3 *= (1.0 + (WeaponPlus.config.UpgradeCostsIncrease * (this.Level + i))) * 0.7;
            if (this.Level + i - 1 < 3)
            {
                num3 *= 0.2;
            }
            price += (long) (num3 * itemById.maxStack * WeaponPlus.config.CostParameters);
            if (price < 0)
            {
                return false;
            }
        }
        return true;
    }
    #endregion
}