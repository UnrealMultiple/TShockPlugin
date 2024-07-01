using TShockAPI;
using Microsoft.Xna.Framework;

namespace DisableMonsLoot
{
    public class Commands
    {

        public static void kdm(CommandArgs args)
        {

            if (args.Parameters.Count == 0)
            {
                if (args == null) return;
                else
                {
                    args.Player.SendMessage("【禁怪掉落指令菜单】\n" +
                     "/kdm 或 /禁掉落 —— 查看禁怪掉落指令菜单\n" +
                     "/kdm on —— 开启|关闭禁怪掉落功能\n" +
                     "/kdm list —— 列出禁止掉落的怪物表\n" +
                     "/kdm kill —— 开启|关闭全杀功能\n" +
                     "/kdm f 数字 —— 设置清理范围(默认35格)\n" +
                     "/reload —— 重载禁怪掉落配置文件\n" +
                     "/kdm reset —— 恢复所有清理开关\n", Color.AntiqueWhite);
                };
                return;
            }

            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "on")
            {
                bool isEnabled = Plugin.Config.Enabled;
                Plugin.Config.Enabled = !isEnabled;
                string statusStr = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage($"【禁怪物掉落】已{statusStr}");
                Plugin.Config.Write();
            }

            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
            {
                var itemList = string.Join(", ",Plugin.Config.BossList.SelectMany(data => data.ID.Select(id => TShock.Utils.GetNPCById(id)?.FullName ?? $"未知NPC")));
                args.Player.SendInfoMessage("【禁止掉落的怪物表】\n" + itemList);
                return;
            }

            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset")
            {
                Plugin.BossDowned.Clear();
                foreach (var npc in Plugin.Config.BossList)
                {
                    npc.Enabled = true;
                }
                Plugin.Config.Write();
                args.Player.SendInfoMessage("【禁怪物掉落】\n" + "'清理列表'中所有怪物的清理功能已开启");
                return;
            }

            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "kill")
            {
                bool isEnabled = Plugin.Config.KillAll;
                Plugin.Config.KillAll = !isEnabled;
                string statusStr = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage($"【禁怪物掉落】已{statusStr}全杀功能。");
                Plugin.Config.Write();
                return;
            }

            if (args.Parameters.Count == 2)
            {
                switch (args.Parameters[0].ToLower())
                {
                    case "f":
                    case "radius":
                    case "范围":
                        {
                            if (int.TryParse(args.Parameters[1], out int num))
                            {
                                Plugin.Config.radius = num;
                                Plugin.Config.Write();
                                args.Player.SendSuccessMessage($"【禁怪物掉落】\n"+"已成功将清理范围设置为: {num} 格!");
                            }
                            break;
                        }
                }
            }

        }

    }
}
