using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChestRestore
{
	[ApiVersion(2, 1)]
	public class MainPlugin : TerrariaPlugin
	{
		public MainPlugin(Main game) : base(game)
		{
		}
		public override string Name
		{
			get
			{
				return "ChestRestore";
			}
		}
		public override Version Version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}
		public override string Author
		{
			get
			{
				return "Cjx修改";
			}
		}
		public override string Description
		{
			get
			{
				return "箱子自动补充资源";
			}
		}
		public override void Initialize()
		{
            ServerApi.Hooks.NetGetData.Register(this, new HookHandler<GetDataEventArgs>(this.OnGetData));
			GetDataHandlers.ChestOpen += new EventHandler<GetDataHandlers.ChestOpenEventArgs>(this.OnChestOpen);
		}
		private void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs args)
		{
			int num = Chest.FindChest(args.X, args.Y);
			Chest chest = Main.chest[num];
			if (chest != null)
			{
				bool flag = false;
				for (int i = 0; i < chest.item.Length; i++)
				{
					if (chest.item[i].stack != 0)
					{
						flag = true;
					}
				}
				if (flag)
				{
					List<NetItem> list = new List<NetItem>();
					for (int j = 0; j < chest.item.Length; j++)
					{
						Item item = chest.item[j];
						list.Add(new NetItem(item.netID, item.stack, item.prefix));
					}
					args.Player.SetData<string>("chestrestore", JsonConvert.SerializeObject(list));
					args.Player.SetData<int>("chestx", args.X);
					args.Player.SetData<int>("chesty", args.Y);
				}
			}
		}
		private void OnGetData(GetDataEventArgs args)
		{
			if (args.MsgID == PacketTypes.ChestOpen)
			{
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
				{
					TSPlayer tsplayer = TShock.Players[args.Msg.whoAmI];
					int num = (int)binaryReader.ReadInt16();
					int num2 = Chest.FindChest(tsplayer.GetData<int>("chestx"), tsplayer.GetData<int>("chesty"));
					Chest chest = null;
					if (num2 != -1)
					{
						chest = Main.chest[num2];
					}
					if (num == -1 && chest != null)
					{
						List<NetItem> list = JsonConvert.DeserializeObject<List<NetItem>>(tsplayer.GetData<string>("chestrestore"));
						for (int i = 0; i < chest.item.Length; i++)
						{
							Item item = chest.item[i];
							item.netDefaults(list[i].NetId);
							item.stack = list[i].Stack;
							item.prefix = list[i].PrefixId;
							TSPlayer.All.SendData(PacketTypes.ChestItem, "", num2, (float)i, 0f, 0f, 0);
						}
						tsplayer.SetData<string>("chestrestore", "");
						tsplayer.SetData<int>("chestx", 0);
						tsplayer.SetData<int>("chesty", 0);
					}
				}
			}
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, new HookHandler<GetDataEventArgs>(this.OnGetData));
				GetDataHandlers.ChestOpen -= new EventHandler<GetDataHandlers.ChestOpenEventArgs>(this.OnChestOpen);
			}
			base.Dispose(disposing);
		}
	}
}
