using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TeleportRequest;

[ApiVersion(2, 1)]
public class TeleportRequest : TerrariaPlugin
{
	private System.Timers.Timer Timer;

	private bool[] TPAllows = new bool[256];

	private bool[] TPacpt = new bool[256];

	private TPRequest[] TPRequests = new TPRequest[256];

	public override string Author => "原作者: MarioE, 修改者: Dr.Toxic，肝帝熙恩";

	public static Config tpConfig { get; set; }

	internal static string tpConfigPath => Path.Combine(TShock.SavePath, "tpconfig.json");

	public override string Description => "传送前需要被传送者接受或拒绝请求";

	public override string Name => "传送请求";

	public override Version Version => new(1, 0, 1);

	public TeleportRequest(Main game)
		: base(game)
	{
		tpConfig = new Config();
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequests[i] = new TPRequest();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
			ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            GeneralHooks.ReloadEvent += ReloadTPR;

            Timer.Dispose();
		}
	}

	public override void Initialize()
	{
		ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
		ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        GeneralHooks.ReloadEvent -= ReloadTPR;
    }

	private void OnElapsed(object sender, ElapsedEventArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout <= 0)
			{
				continue;
			}
			TSPlayer tSPlayer = TShock.Players[tPRequest.dst];
			TSPlayer tSPlayer2 = TShock.Players[i];
			tPRequest.timeout--;
			if (tPRequest.timeout == 0)
			{
				tSPlayer2.SendErrorMessage("传送请求已超时.");
				tSPlayer.SendInfoMessage("玩家[{0}]的传送请求已超时.", tSPlayer2.Name);
				continue;
			}
			string format = string.Format("玩家[{{0}}]要求传送到你当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
			if (tPRequest.dir)
			{
				format = string.Format("你被请求传送到玩家[{{0}}]的当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
			}
			tSPlayer.SendInfoMessage(format, tSPlayer2.Name);
		}
	}

	private void OnInitialize(EventArgs e)
	{
		Commands.ChatCommands.Add(new Command("tprequest.gettpr", TPAccept, "接受tp", "atp")
		{
			AllowServer = false,
			HelpText = "接受传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpauto", TPAutoDeny, "自动拒绝tp", "autodeny")
		{
			AllowServer = false,
			HelpText = "自动拒绝所有人的传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpauto", TPAutoAccept, "自动接受tp", "autoaccept")
		{
			AllowServer = false,
			HelpText = "自动接受所有人的传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.gettpr", TPDeny, "拒绝tp", "dtp")
		{
			AllowServer = false,
			HelpText = "拒绝传送请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpat", TPAHere, "tpahere")
		{
			AllowServer = false,
			HelpText = "发出把指定玩家传送到你当前位置的请求."
		});
		Commands.ChatCommands.Add(new Command("tprequest.tpat", TPA, "tpa")
		{
			AllowServer = false,
			HelpText = "发出传送到指定玩家当前位置的请求."
		});
		SetupConfig();
		Timer = new System.Timers.Timer(tpConfig.IntervalInSeconds * 1000);
		Timer.Elapsed += OnElapsed;
		Timer.Start();
	}

	private void OnLeave(LeaveEventArgs e)
	{
		TPAllows[e.Who] = false;
		TPacpt[e.Who] = false;
		TPRequests[e.Who].timeout = 0;
	}

	private void TPA(CommandArgs e)
	{
		if (e.Parameters.Count == 0)
		{
			e.Player.SendErrorMessage("格式错误! 正确格式为: {0}tpa <玩家>", Commands.Specifier);
			return;
		}
		string search = string.Join(" ", e.Parameters.ToArray());
		List<TSPlayer> list = TSPlayer.FindByNameOrID(search);
		if (list.Count == 0)
		{
			e.Player.SendErrorMessage("找不到这位玩家!");
			return;
		}
		if (list.Count > 1)
		{
			e.Player.SendErrorMessage("匹对到多于一位玩家!");
			return;
		}
		if (list[0].Equals(e.Player))
		{
			e.Player.SendErrorMessage("禁止向自己发送传送请求！");
			return;
		}
		if ((!list[0].TPAllow || TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			e.Player.SendErrorMessage("你无法传送到玩家[{0}].", list[0].Name);
			return;
		}
		if ((list[0].TPAllow && TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			bool flag = false;
			TSPlayer tSPlayer = (flag ? TShock.Players[list[0].Index] : e.Player);
			TSPlayer tSPlayer2 = (flag ? e.Player : TShock.Players[list[0].Index]);
			if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
			{
				tSPlayer.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", tSPlayer2.Name);
				tSPlayer2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", tSPlayer.Name);
			}
			return;
		}
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
			{
				e.Player.SendErrorMessage("玩家[{0}]已被其他玩家发出传送请求.", list[0].Name);
				return;
			}
		}
		TPRequests[e.Player.Index].dir = false;
		TPRequests[e.Player.Index].dst = (byte)list[0].Index;
		TPRequests[e.Player.Index].timeout = tpConfig.TimeoutCount + 1;
		e.Player.SendSuccessMessage("已成功向玩家[{0}]发出传送请求.", list[0].Name);
	}

	private void TPAccept(CommandArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
			{
				TSPlayer tSPlayer = (tPRequest.dir ? e.Player : TShock.Players[i]);
				TSPlayer tSPlayer2 = (tPRequest.dir ? TShock.Players[i] : e.Player);
				if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
				{
					tSPlayer.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", tSPlayer2.Name);
					tSPlayer2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", tSPlayer.Name);
				}
				tPRequest.timeout = 0;
				return;
			}
		}
		e.Player.SendErrorMessage("你暂时没有收到其他玩家的传送请求.");
	}

	private void TPAHere(CommandArgs e)
	{
		if (e.Parameters.Count == 0)
		{
			e.Player.SendErrorMessage("格式错误! 正确格式为: {0}tpahere <玩家>", Commands.Specifier);
			return;
		}
		string search = string.Join(" ", e.Parameters.ToArray());
		List<TSPlayer> list = TSPlayer.FindByNameOrID(search);
		if (list.Count == 0)
		{
			e.Player.SendErrorMessage("找不到这位玩家!");
			return;
		}
		if (list.Count > 1)
		{
			e.Player.SendErrorMessage("匹对到多于一位玩家!");
			return;
		}
		if ((!list[0].TPAllow || TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			e.Player.SendErrorMessage("你无法传送到玩家[{0}].", list[0].Name);
			return;
		}
		if ((list[0].TPAllow && TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
		{
			bool flag = true;
			TSPlayer tSPlayer = (flag ? TShock.Players[list[0].Index] : e.Player);
			TSPlayer tSPlayer2 = (flag ? e.Player : TShock.Players[list[0].Index]);
			if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
			{
				tSPlayer.SendSuccessMessage("已经传送到玩家[{0}]的当前位置.", tSPlayer2.Name);
				tSPlayer2.SendSuccessMessage("玩家[{0}]已传送到你的当前位置.", tSPlayer.Name);
			}
			return;
		}
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
			{
				e.Player.SendErrorMessage("玩家[{0}]已被其他玩家发出传送请求.", list[0].Name);
				return;
			}
		}
		TPRequests[e.Player.Index].dir = true;
		TPRequests[e.Player.Index].dst = (byte)list[0].Index;
		TPRequests[e.Player.Index].timeout = tpConfig.TimeoutCount + 1;
		e.Player.SendSuccessMessage("已成功向玩家[{0}]发出传送请求.", list[0].Name);
	}

	private void TPAutoDeny(CommandArgs e)
	{
		if (TPacpt[e.Player.Index])
		{
			e.Player.SendErrorMessage("请先解除自动接受传送");
			return;
		}
		TPAllows[e.Player.Index] = !TPAllows[e.Player.Index];
		e.Player.SendInfoMessage("{0}自动拒绝传送请求.", TPAllows[e.Player.Index] ? "启用" : "解除");
	}

	private void TPDeny(CommandArgs e)
	{
		for (int i = 0; i < TPRequests.Length; i++)
		{
			TPRequest tPRequest = TPRequests[i];
			if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
			{
				e.Player.SendSuccessMessage("已拒绝玩家[{0}]的传送请求.", TShock.Players[i].Name);
				TShock.Players[i].SendErrorMessage("玩家[{0}]拒绝你的传送请求.", e.Player.Name);
				tPRequest.timeout = 0;
				return;
			}
		}
		e.Player.SendErrorMessage("你暂时没有收到其他玩家的传送请求.");
	}

	private void TPAutoAccept(CommandArgs e)
	{
		if (TPAllows[e.Player.Index])
		{
			e.Player.SendErrorMessage("请先解除自动拒绝传送");
			return;
		}
		TPacpt[e.Player.Index] = !TPacpt[e.Player.Index];
		e.Player.SendInfoMessage("{0}自动接受传送请求.", TPacpt[e.Player.Index] ? "启用" : "解除");
	}

	private void SetupConfig()
	{
		try
		{
			if (File.Exists(tpConfigPath))
			{
				tpConfig = Config.Read(tpConfigPath);
			}
			tpConfig.Write(tpConfigPath);
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("TPR配置发生错误");
			Console.ForegroundColor = ConsoleColor.Gray;
			TShock.Log.ConsoleError("TPR配置出现异常");
			TShock.Log.ConsoleError(ex.ToString());
		}
	}

	private void ReloadTPR(ReloadEventArgs args)
	{
		SetupConfig();
        args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(TeleportRequest).Name);
    }
}
