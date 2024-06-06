using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.WeaponPlus
{
    public class WItem
    {
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

        public int Level => damage_level + scale_level + knockBack_level + useSpeed_level + shootSpeed_level;

        public WItem(int ID, string owner = "")
        {
            //IL_0097: Unknown result type (might be due to invalid IL or missing references)
            //IL_009c: Unknown result type (might be due to invalid IL or missing references)
            Item itemById = TShock.Utils.GetItemById(ID);
            id = ID;
            this.owner = owner;
            damage_level = 0;
            scale_level = 0;
            knockBack_level = 0;
            useSpeed_level = 0;
            shootSpeed_level = 0;
            allCost = 0L;
            orig_damage = itemById.damage;
            orig_scale = itemById.scale;
            orig_knockBack = itemById.knockBack;
            orig_useAnimation = itemById.useAnimation;
            orig_useTime = itemById.useTime;
            orig_shootSpeed = itemById.shootSpeed;
            orig_color = itemById.color;
            orig_shoot = itemById.shoot;
        }

        public void CheckDB()
        {
            if (string.IsNullOrWhiteSpace(owner) || Level > 0)
            {
                return;
            }
            try
            {
                WeaponPlus.DB.DeleteDB(owner, id);
                List<TSPlayer> list = TSPlayer.FindByNameOrID(owner);
                if (list.Count != 1)
                {
                    return;
                }
                WPlayer wPlayer = WeaponPlus.wPlayers[list[0].Index];
                if (wPlayer != null && wPlayer.isActive)
                {
                    wPlayer.hasItems.RemoveAll((x) => x.id == id);
                }
            }
            catch
            {
            }
        }

        public string ItemMess()
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2;
            StringBuilder.AppendInterpolatedStringHandler handler;
            if (Level > 0)
            {
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder3 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(16, 15, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("当前总等级："));
                handler.AppendFormatted(Level);
                handler.AppendLiteral("   ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("剩余强化次数："));
                handler.AppendFormatted(WeaponPlus.config.MaximunofLevel - Level);
                handler.AppendLiteral(" ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("次"));
                handler.AppendLiteral("    ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("伤害等级："));
                handler.AppendFormatted(damage_level);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("大小等级："));
                handler.AppendFormatted(scale_level);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("击退等级："));
                handler.AppendFormatted(knockBack_level);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("攻速等级："));
                handler.AppendFormatted(useSpeed_level);
                handler.AppendLiteral(", ");
                handler.AppendFormatted(WeaponPlus.LangTipsGet("射弹飞行速度等级："));
                handler.AppendFormatted(shootSpeed_level);
                stringBuilder3.AppendLine(ref handler);
            }
            else
            {
                stringBuilder.AppendLine(WeaponPlus.LangTipsGet("未升级过，无任何加成"));
            }
            int num = (int)(orig_damage * 0.05f * damage_level);
            if (num < damage_level)
            {
                num = damage_level;
            }
            float value = num * 1f / orig_damage;
            stringBuilder2 = stringBuilder;
            StringBuilder stringBuilder4 = stringBuilder2;
            handler = new StringBuilder.AppendInterpolatedStringHandler(9, 7, stringBuilder2);
            handler.AppendFormatted(WeaponPlus.LangTipsGet("当前状态："));
            handler.AppendFormatted(WeaponPlus.LangTipsGet("伤害"));
            handler.AppendLiteral(" +");
            handler.AppendFormatted(value, "0.00%");
            handler.AppendLiteral("，");
            handler.AppendFormatted(WeaponPlus.LangTipsGet("大小"));
            handler.AppendLiteral(" +");
            handler.AppendFormatted(0.05f * scale_level, "0.00%");
            handler.AppendLiteral("，");
            handler.AppendFormatted(WeaponPlus.LangTipsGet("击退"));
            handler.AppendLiteral(" +");
            handler.AppendFormatted(0.05f * knockBack_level, "0.00%");
            handler.AppendLiteral("，");
            stringBuilder4.Append(ref handler);
            value = orig_useAnimation * 1f / (orig_useAnimation - useSpeed_level) - 1f;
            stringBuilder2 = stringBuilder;
            StringBuilder stringBuilder5 = stringBuilder2;
            handler = new StringBuilder.AppendInterpolatedStringHandler(3, 4, stringBuilder2);
            handler.AppendFormatted(WeaponPlus.LangTipsGet("攻速"));
            handler.AppendLiteral("+");
            handler.AppendFormatted(value, "0.00%");
            handler.AppendLiteral("，");
            handler.AppendFormatted(WeaponPlus.LangTipsGet("射弹飞速"));
            handler.AppendLiteral("+");
            handler.AppendFormatted(0.05f * shootSpeed_level, "0.00%");
            stringBuilder5.AppendLine(ref handler);
            if (Level < WeaponPlus.config.MaximunofLevel)
            {
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder6 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("伤害升至下一级需："));
                handler.AppendFormatted(plusPrice(PlusType.damage, out var price) ? price : WeaponPlus.LangTipsGet("当前已满级"));
                stringBuilder6.AppendLine(ref handler);
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder7 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("大小升至下一级需："));
                handler.AppendFormatted(plusPrice(PlusType.scale, out price) ? price : WeaponPlus.LangTipsGet("当前已满级"));
                stringBuilder7.AppendLine(ref handler);
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder8 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("击退升至下一级需："));
                handler.AppendFormatted(plusPrice(PlusType.knockBack, out price) ? price : WeaponPlus.LangTipsGet("当前已满级"));
                stringBuilder8.AppendLine(ref handler);
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder9 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("攻速升至下一级需："));
                handler.AppendFormatted(plusPrice(PlusType.useSpeed, out price) ? price : WeaponPlus.LangTipsGet("当前已满级"));
                stringBuilder9.AppendLine(ref handler);
                stringBuilder2 = stringBuilder;
                StringBuilder stringBuilder10 = stringBuilder2;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder2);
                handler.AppendFormatted(WeaponPlus.LangTipsGet("射弹飞速升至下一级需："));
                handler.AppendFormatted(plusPrice(PlusType.shootSpeed, out price) ? price : WeaponPlus.LangTipsGet("当前已满级"));
                stringBuilder10.Append(ref handler);
            }
            else
            {
                stringBuilder.Append(WeaponPlus.LangTipsGet("已达到最大武器总等级"));
            }
            return stringBuilder.ToString();
        }

        public bool plusPrice(PlusType plus, out long price, int gap = 1)
        {
            Item itemById = TShock.Utils.GetItemById(id);
            price = 0L;
            if (Level + gap > WeaponPlus.config.MaximunofLevel)
            {
                return false;
            }
            for (int i = 1; i <= gap; i++)
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
                double num2 = 2.0;
                double num3 = num / num2;
                switch (plus)
                {
                    case PlusType.damage:
                        {
                            int num5 = (int)(orig_damage * 0.05 * (damage_level + i));
                            if (num5 < damage_level + i)
                            {
                                num3 = 1.0 / orig_damage * (num / (num2 / 20.0));
                                num5 = damage_level + i;
                            }
                            int num6 = itemById.melee ? (int)(WeaponPlus.config.MaximumDamageMultipleOfMeleeWeapons * orig_damage) : itemById.magic ? (int)(WeaponPlus.config.MaximumDamageMultipleOfMagicWeapons * orig_damage) : itemById.ranged ? (int)(WeaponPlus.config.MaximumDamageMultipleOfRangeWeapons * orig_damage) : !itemById.summon ? (int)(WeaponPlus.config.MaximumDamageMultipleOfOtherWeapons * orig_damage) : (int)(WeaponPlus.config.MaximumDamageMultipleOfSummonWeapons * orig_damage);
                            if (num5 + orig_damage > num6)
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
                        if (orig_scale + orig_scale * 0.05 * (scale_level + i) > (double)(orig_scale * WeaponPlus.config.MaximumScaleMultipleOfWeaponUpgrade))
                        {
                            return false;
                        }
                        if (!itemById.melee)
                        {
                            num3 *= 0.7;
                        }
                        break;
                    case PlusType.knockBack:
                        if (orig_knockBack + orig_knockBack * 0.05 * (knockBack_level + i) > (double)(orig_knockBack * WeaponPlus.config.MaximumKnockBackMultipleOfWeaponUpgrade))
                        {
                            return false;
                        }
                        num3 = !itemById.summon ? num3 * 0.85 : num3 * 0.7;
                        break;
                    case PlusType.useSpeed:
                        {
                            int num4 = itemById.melee ? WeaponPlus.config.MaximumAttackSpeedOfMeleeWeaponUpgrade : itemById.magic ? WeaponPlus.config.MaximumAttackSpeedOfMagicWeaponUpgrade : itemById.ranged ? WeaponPlus.config.MaximumAttackSpeedOfRangeWeaponUpgrade : !itemById.summon ? WeaponPlus.config.MaximumAttackSpeedOfOtherWeaponUpgrade : WeaponPlus.config.MaximumAttackSpeedOfSummonWeaponUpgrade;
                            if (orig_useAnimation - (useSpeed_level + i) < num4 || orig_useAnimation <= num4)
                            {
                                return false;
                            }
                            if (orig_useTime - (useSpeed_level + i) < num4 || orig_useTime <= num4)
                            {
                                return false;
                            }
                            if (orig_useAnimation * 1.0 / (orig_useAnimation - (useSpeed_level + i)) > WeaponPlus.config.MaximumUseTimeMultipleOfWeaponUpgrade)
                            {
                                return false;
                            }
                            num3 = (orig_useAnimation * 1.0 / (orig_useAnimation - (useSpeed_level + i * 1.0)) - orig_useAnimation * 1.0 / (orig_useAnimation - (useSpeed_level + i - 1.0))) * (num / (num2 / 20.0));
                            num3 *= 1.05;
                            if (itemById.summon && itemById.mana > 0)
                            {
                                num3 *= 0.3;
                            }
                            break;
                        }
                    case PlusType.shootSpeed:
                        if (orig_shootSpeed + orig_shootSpeed * 0.05 * (shootSpeed_level + i) > (double)(orig_shootSpeed * WeaponPlus.config.MaximumProjectileSpeedMultipleOfWeaponUpgrade))
                        {
                            return false;
                        }
                        break;
                }
                num3 *= (1.0 + WeaponPlus.config.UpgradeCostsIncrease * (Level + i)) * 0.7;
                if (Level + i - 1 < 3)
                {
                    num3 *= 0.2;
                }
                price += (long)(num3 * itemById.maxStack * WeaponPlus.config.CostParameters);
                if (price < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

}
