using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace TrCDK
{
	public static class TSPlayerExtensions
	{
		public static void SendMessageFormat(this TSPlayer Player, Color Colour, string MessageFormat, params object[] args)
		{
			Player.SendMessage(string.Format(MessageFormat, args), Colour);
		}

		public static bool PermissionlessInvoke(this TSPlayer player, string text, bool silent = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			var text2 = text.Remove(0, 1);
			var list = typeof(Commands).CallPrivateMethod<List<string>>(StaticMember: true, "ParseParameters", new object[1] { text2 });
			if (list.Count < 1)
			{
				return false;
			}
			var cmdName = list[0].ToLower();
			list.RemoveAt(0);
			var enumerable = Commands.ChatCommands.Where((Command c) => c.HasAlias(cmdName));
			if (enumerable.Count() == 0)
			{
				if (player.AwaitingResponse.ContainsKey(cmdName))
				{
					Action<CommandArgs> action = player.AwaitingResponse[cmdName];
					player.AwaitingResponse.Remove(cmdName);
					action(new CommandArgs(text2, player, list));
					return true;
				}
				player.SendErrorMessage(GetString("输入的命令无效.键入/help获取有效命令列表."));
				return true;
			}
			foreach (var item in enumerable)
			{
				if (!item.AllowServer && !player.RealPlayer)
				{
					player.SendErrorMessage(GetString("你必须在游戏中使用这个命令."));
					continue;
				}
				if (item.DoLog && !silent)
				{
					TShock.Utils.SendLogs(player.Name + GetString(" 执行: /") + text2 + ".", Color.Red);
				}
				item.RunWithoutPermissions(text2, player, list);
			}
			return true;
		}
	}
}
