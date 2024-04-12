using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RainbowChat;

[ApiVersion(2, 1)]
public class RainbowChat : TerrariaPlugin
{
	private Random _rand = new Random();

	private bool[] _rainbowChat = new bool[255];

	public override string Name => "【五彩斑斓聊天】 Rainbow Chat";

	public override string Author => "Professor X制作,nnt升级/汉化,肝帝熙恩更新1449";

	public override string Description => "使玩家每次说话的颜色不一样.";

	public override Version Version => new Version(1, 0, 2);

	public RainbowChat(Main game)
		: base(game)
	{
	}

    public override void Initialize()
    {
        // 注册事件
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 注销事件
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
        }

        // 调用基类的Dispose方法
        base.Dispose(disposing);
    }



    private void OnInitialize(EventArgs args)
	{
		Commands.ChatCommands.Add(new Command("rainbowchat.use", RainbowChatCallback, "rainbowchat", "rc"));
	}

	private void OnChat(ServerChatEventArgs e)
	{
        // 如果事件已经被处理，直接返回
        if (e.Handled)
		{
			return;
		}

        TSPlayer player = TShock.Players[e.Who];
        if (player == null || !player.HasPermission(Permissions.canchat) || player.mute)
		{
            return;
        }

        // 检查是否是命令，如果是，不进行处理
        if (e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) ||
            e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
			{
            return;
			}

        // 检查是否启用了彩虹聊天功能
        if (!_rainbowChat[player.Index])
        {
            return;
		}

        // 获取颜色列表
        List<Color> colors = GetColors();

        // 使用string.Join和Lambda表达式来构建彩色消息
        string coloredMessage = string.Join(" ", e.Text.Split(' ').Select((word, index) =>
            TShock.Utils.ColorTag(word, colors[_rand.Next(colors.Count)])));

        // 发送彩色消息给所有玩家
        TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, coloredMessage), player.Group.R, player.Group.G, player.Group.B);

        // 发送原始消息到服务器日志
        TSPlayer.Server.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, e.Text), player.Group.R, player.Group.G, player.Group.B);

        // 标记事件为已处理
        e.Handled = true;
	}


	private void RainbowChatCallback(CommandArgs e)
	{
        // 获取当前玩家的彩虹聊天状态
        bool currentState = _rainbowChat[e.Player.Index];

        // 切换状态
        _rainbowChat[e.Player.Index] = !currentState;

        // 如果没有提供参数，则只更新当前玩家的状态
		if (e.Parameters.Count == 0)
		{
            e.Player.SendSuccessMessage(string.Format("你 {0} 彩虹聊天.", currentState ? "已关闭" : "已开启"));
			return;
		}

        // 查找玩家
        List<TSPlayer> players = TSPlayer.FindByNameOrID(e.Parameters[0]);
        if (players.Count == 0)
		{
			e.Player.SendErrorMessage("错误的玩家!");
            return;
		}

        // 确保只有一个匹配的玩家
        if (players.Count > 1)
		{
            e.Player.SendMultipleMatchError(players.Select(p => p.Name));
            return;
		}

        // 切换找到的玩家的彩虹聊天状态
        _rainbowChat[players[0].Index] = !_rainbowChat[players[0].Index];

        // 发送成功消息
        e.Player.SendSuccessMessage(string.Format("{0} 已 {1} 彩虹聊天.", players[0].Name, currentState ? "已关闭" : "已开启"));
	}


	private List<Color> GetColors()
	{
        List<Color> colors = new List<Color>();

        // 获取Color类型的所有静态颜色属性
        PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);

        foreach (PropertyInfo property in properties)
		{
            // 确保属性是Color类型且可读
            if (property.PropertyType == typeof(Color) && property.CanRead)
			{
                // 添加静态颜色属性到列表中
                colors.Add((Color)property.GetValue(null));
			}
		}

        return colors;
	}

}
