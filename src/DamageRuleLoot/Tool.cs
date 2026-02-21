using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DamageRuleLoot;

public static class Tool
{
    #region 合并多个伤害字典
    public static Dictionary<string, double> CombineDamages(params Dictionary<string, double>[] Damages)
    {
        var comb = new Dictionary<string, double>();
        foreach (var Dict in Damages)
        {
            foreach (var data in Dict)
            {
                if (comb.ContainsKey(data.Key))
                {
                    comb[data.Key] += data.Value;
                }
                else
                {
                    comb.Add(data.Key, data.Value);
                }
            }
        }
        return comb;
    }

    public static double GetCombineDamages(Dictionary<string, double> damage)
    {
        double Damage = 0;
        foreach (var item in damage)
        {
            Damage += item.Value;
        }
        return Damage;
    }

    #endregion

    #region 同键名值相减
    public static Dictionary<string, double> SubtractDics(Dictionary<string, double> dictA, Dictionary<string, double> dictB)
    {
        var result = new Dictionary<string, double>();

        foreach (var key in dictA.Keys)
        {
            if (dictB.ContainsKey(key))
            {
                result[key] = dictA[key] - dictB[key];
            }
            else
            {
                // 如果 dictB 中没有这个键，则保留原值
                result[key] = dictA[key];
            }
        }

        return result;
    }
    #endregion

    #region 更新伤害字典
    public static void UpdateDict(Dictionary<string, double> dictionary, string key, double value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
    #endregion

    #region  清空伤害字典
    public static void ClearDictionaries(params Dictionary<string, double>[] dictionaries)
    {
        foreach (var dict in dictionaries)
        {
            dict.Clear();
        }
    }
    #endregion

    #region 获取NPCID的中文名
    public static void WriteName()
    {
        foreach (var list in DamageRuleLoot.Config.TList)
        {
            if (list.Name.Count() < 1)
            {
                var npcName = (string) Lang.GetNPCName(list.NPCA);
                list.Name = npcName;
            }
        }
    }
    #endregion

    #region 通用伤害判断法，用于把肢体伤害合成到头部
    public static bool CombDmg(NpcKilledEventArgs args, int i, bool flag, int id, int[] ids, float value)
    {
        var index = -1;
        foreach (var n in Main.npc)
        {
            if (n.whoAmI != args.npc.whoAmI && ids.Contains(n.type) && n.active)
            {
                flag = false;
            }
            if (n.netID == id)
            {
                index = n.whoAmI;
            }
        }
        if (index >= 0)
        {
            var st = StrikeNPC.strikeNPC.Find(x => x.npcID == id);
            if (st == null)
            {
                StrikeNPC.strikeNPC.Add(new StrikeNPC(index, id, Lang.GetNPCNameValue(id), StrikeNPC.strikeNPC[i].PlayerOrDamage, StrikeNPC.strikeNPC[i].AllDamage, value));
            }
            else
            {
                foreach (var y in StrikeNPC.strikeNPC[i].PlayerOrDamage)
                {
                    if (st.PlayerOrDamage.ContainsKey(y.Key))
                    {
                        st.PlayerOrDamage[y.Key] += y.Value;
                        st.AllDamage += y.Value;
                    }
                    else
                    {
                        st.PlayerOrDamage.Add(y.Key, y.Value);
                        st.AllDamage += y.Value;
                    }
                }
            }
        }

        return flag;
    }


    public static void CombDmg2(int i, ref StrikeNPC? strike2, int id, int[] ids, float value)
    {
        if (strike2 == null)
        {
            var index = -1;
            foreach (var n in Main.npc)
            {
                if (n.netID == id)
                {
                    index = n.whoAmI;
                }
            }
            if (index == -1)
            {
                StrikeNPC.strikeNPC.RemoveAll(x => ids.Contains(x.npcID) || x.npcID == id || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                return;
            }
            strike2 = new StrikeNPC(index, id, Main.npc[index].FullName, StrikeNPC.strikeNPC[i].PlayerOrDamage, StrikeNPC.strikeNPC[i].AllDamage, value);
            StrikeNPC.strikeNPC.Add(strike2);
        }

        else if (StrikeNPC.strikeNPC[i].npcID != id)
        {
            foreach (var v in StrikeNPC.strikeNPC[i].PlayerOrDamage)
            {
                if (strike2.PlayerOrDamage.ContainsKey(v.Key))
                {
                    strike2.PlayerOrDamage[v.Key] += v.Value;
                    strike2.AllDamage += v.Value;
                }
                else
                {
                    strike2.PlayerOrDamage.Add(v.Key, v.Value);
                    strike2.AllDamage += v.Value;
                }
            }
        }
    }

    //机械骷髅王的单独处理
    public static void CombDmg3(bool flag, int i, ref StrikeNPC? strike2, int id, int[] ids, float value)
    {
        if (strike2 == null)
        {
            var index = -1;
            foreach (var n in Main.npc)
            {
                if (n.netID == id)
                {
                    index = n.whoAmI;
                }
            }
            if (index == -1)
            {
                StrikeNPC.strikeNPC.RemoveAll(x => ids.Contains(x.npcID) || x.npcID == id || x.npcID != Main.npc[x.npcIndex].netID || !Main.npc[x.npcIndex].active);
                return;
            }
            strike2 = new StrikeNPC(index, id, Main.npc[index].FullName, StrikeNPC.strikeNPC[i].PlayerOrDamage, StrikeNPC.strikeNPC[i].AllDamage, value);
            StrikeNPC.strikeNPC.Add(strike2);
        }

        else if (StrikeNPC.strikeNPC[i].npcID != id)
        {
            if (!flag)
            {
                return; //比CombDmg2多了这一行 这个Config.Prime同时也影响伤害转移
            }

            foreach (var v in StrikeNPC.strikeNPC[i].PlayerOrDamage)
            {
                if (strike2.PlayerOrDamage.ContainsKey(v.Key))
                {
                    strike2.PlayerOrDamage[v.Key] += v.Value;
                    strike2.AllDamage += v.Value;
                }
                else
                {
                    strike2.PlayerOrDamage.Add(v.Key, v.Value);
                    strike2.AllDamage += v.Value;
                }
            }
        }
    }
    #endregion

    #region 通用转换伤害法
    public static void General(NPC npc, int Damage, Player plr, StrikeNPC? strike, float life)
    {
        if (strike == null)
        {
            return;
        }

        if (Damage > 0 && Main.npc[strike.npcIndex].life > life)
        {
            if (Main.npc[strike.npcIndex].active)
            {
                Main.npc[strike.npcIndex].life -= Damage;
                Main.npc[strike.npcIndex].netUpdate = true;

                if (strike.PlayerOrDamage.ContainsKey(plr.name))
                {
                    strike.PlayerOrDamage[plr.name] += Damage;
                    strike.AllDamage += Damage;
                }
            }

            if (DamageRuleLoot.Config.TransferInfo)
            {
                if (Main.npc[strike.npcIndex].life > life)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"[c/FBF069:【转移】] 玩家:[c/F06576:{plr.name}] ") +
                        GetString($"攻击对象:[c/AEA3E4:{npc.FullName}] | ") +
                        GetString($"转移:[c/6DDA6D:{strike.npcName}] 伤害:[c/F06576:{Damage}] ") +
                        GetString($"生命:[c/FBF069:{Main.npc[strike.npcIndex].life}]"), 202, 221, 222);
                }

                if (Main.npc[strike.npcIndex].life <= life)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"[c/F06576:【停转】] 玩家:[c/F06576:{plr.name}] ") +
                        GetString($"转伤对象:[c/AEA3E4:{strike.npcName}] | 生命值:[c/6DDA6D:{Main.npc[strike.npcIndex].life}] < ") +
                        GetString($"[c/F06576:{life}]"), 202, 221, 222);
                }
            }
        }
    }
    #endregion

    #region 自定义转换伤害方法
    public static void TransformDamage(NPC npc, int Damage, Player plr, Configuration.ItemData Custom, StrikeNPC? strike)
    {
        if (Main.npc[strike!.npcIndex].active)
        {
            Main.npc[strike.npcIndex].life -= Damage;
            Main.npc[strike.npcIndex].netUpdate = true;

            if (strike.PlayerOrDamage.ContainsKey(plr.name))
            {
                strike.PlayerOrDamage[plr.name] += Damage;
                strike.AllDamage += Damage;
            }
        }

        if (DamageRuleLoot.Config.TransferInfo)
        {
            if (Main.npc[strike.npcIndex].life > Custom.LifeLimit)
            {
                TSPlayer.All.SendInfoMessage(GetString($"[c/FBF069:【转伤】] 玩家:[c/F06576:{plr.name}] ") +
                    GetString($"攻击对象:[c/AEA3E4:{npc.FullName}] | ") +
                    GetString($"转移:[c/6DDA6D:{strike.npcName}] 伤害:[c/F06576:{Damage}] ") +
                    GetString($"生命:[c/FBF069:{Main.npc[strike.npcIndex].life}]"), 202, 221, 222);
            }

            if (Main.npc[strike.npcIndex].life <= Custom.LifeLimit)
            {
                TSPlayer.All.SendInfoMessage(GetString($"[c/F06576:【停转】] 玩家:[c/F06576:{plr.name}] ") +
                    GetString($"转伤对象:[c/AEA3E4:{strike.npcName}] | 生命值:[c/6DDA6D:{Main.npc[strike.npcIndex].life}] < ") +
                    GetString($"[c/F06576:{Custom.LifeLimit}] "), 202, 221, 222);
            }
        }
    }
    #endregion

    #region 统计伤害+惩罚 并发松消息方法
    public static void SendKillMessage(string BossName, Dictionary<string, double> PlayerOrDamage, double allDamage)
    {
        var mess = new StringBuilder();
        var sortpairs = new Dictionary<string, double>();
        var LowDamager = new StringBuilder();
        var PlayerCount = TShock.Utils.GetActivePlayerCount();
        var Escape = PlayerCount - PlayerOrDamage.Count;
        mess.AppendLine(GetString($"            [i:3455][c/AD89D5:伤][c/D68ACA:害][c/DF909A:排][c/E5A894:行][c/E5BE94:榜][i:3454]"));
        mess.AppendLine(GetString($" 当前服务器有 [c/74F3C9:{PlayerCount}位] 玩家 | 未参战: [c/A7DDF0:{Escape}位]"));
        mess.AppendLine(GetString($" 恭喜以下 [c/74F3C9:{PlayerOrDamage.Count}位] 玩家击败了 [c/F7686D:{BossName}]"));

        while (PlayerOrDamage.Count > 0)
        {
            string key = null!;
            double damage = 0;
            foreach (var v in PlayerOrDamage)
            {
                if (v.Value > damage)
                {
                    key = v.Key;
                    damage = v.Value;
                }
            }
            if (key != null)
            {
                sortpairs.Add(key, damage);
                PlayerOrDamage.Remove(key);
            }
        }

        foreach (var data in sortpairs)
        {
            mess.AppendLine(GetString($" [c/A7DDF0:{TShock.UserAccounts.GetUserAccountByName(data.Key).Name}]") +
                GetString($"   伤害:[c/74F3C9:{data.Value}]") +
                GetString($"   暴击:[c/74F3C9:{CritTracker.GetCritCount(TShock.UserAccounts.GetUserAccountByName(data.Key).Name)}]") +
                GetString($"   比例:[c/74F3C9:{data.Value * 1.0f / allDamage:0.00%}]"));

            CritTracker.CritCounts[data.Key] = 0;

            if (DamageRuleLoot.Config.Enabled2)
            {
                var npc = Terraria.Main.npc.Any(npc => DamageRuleLoot.Config.Expand.Contains(npc.netID));

                if (((data.Value / allDamage) < DamageRuleLoot.Config.Damages) && !npc)
                {
                    LowDamager.AppendFormat(GetString(" [c/A7DDF0:{0}]([c/74F3C9:{1:0.00%}])", TShock.UserAccounts.GetUserAccountByName(data.Key).Name, data.Value / allDamage));

                    if (LowDamager.Length > 0)
                    {
                        LowDamager.Append(", ");
                    }

                    foreach (var plr in TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn))
                    {
                        if (plr.Name == data.Key)
                        {
                            for (var i = 0; i < Terraria.Main.maxItems; i++)
                            {
                                if (Terraria.Main.timeItemSlotCannotBeReusedFor[i] == 54000)
                                {
                                    Main.item[i].TurnToAir();
                                    plr.SendData(PacketTypes.SyncItemDespawn, "", i);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (DamageRuleLoot.Config.Broadcast)
        {
            if (DamageRuleLoot.Config.Enabled3)
            {
                mess.AppendLine(DamageRuleLoot.Config.Advertisement);
            }

            TSPlayer.All.SendMessage(mess.ToString(), 247, 244, 150);
        }

        if (LowDamager.Length > 0 && DamageRuleLoot.Config.Broadcast2)
        {
            var playerNames = LowDamager.ToString().Split(new[] { "," }, StringSplitOptions.None);
            var joinedNames = string.Join(", ", playerNames);

            LowDamager.Insert(0, GetString($"[c/F06576:【注意】]输出少于 [c/A7DDF0:{DamageRuleLoot.Config.Damages:0.00%}] 禁止掉落宝藏袋:\n"));

            TSPlayer.All.SendMessage(LowDamager.ToString(), 247, 244, 150);
        }
    }
    #endregion
}
