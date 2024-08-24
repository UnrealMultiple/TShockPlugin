using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static Plugin.Configuration;
using static TShockAPI.GetDataHandlers;
using static TShockAPI.Hooks.AccountHooks;

namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region 插件信息
        public override string Name => "服主特权";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 8, 1);
        public override string Description => "涡轮增压不蒸鸭";
        #endregion


        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        private GeneralHooks.ReloadEventD _reloadHandler;
        public override void Initialize()
        {
            LoadConfig();
            _reloadHandler = (_) => LoadConfig();
            GeneralHooks.ReloadEvent += _reloadHandler;
            PlayerSpawn += OnSpawn!;
            AccountCreate += OnRegister;
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= _reloadHandler;
                PlayerSpawn -= OnSpawn!;
                AccountCreate -= OnRegister;
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            }
            base.Dispose(disposing);
        }
        #endregion


        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private static void LoadConfig()
        {
            Config = Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[服主特权]重新加载配置完毕。");
        }
        #endregion


        #region 玩家生成事件
        private void OnSpawn(object o, SpawnEventArgs args)
        {
            TSPlayer plr = args.Player;
            var AllPerm = Config.AllPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var BuffPerm = Config.BuffPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var ItemPerm = Config.ItemPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var CmdPerm = Config.CmdPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var SSCPerm = Config.SSCPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var GodPerm = Config.GodPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var Power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();

            if (plr == null || args == null || !plr.IsLoggedIn) return;

            if (Config.PlayersList.Contains(plr.Name) || AllPerm.Contains(plr.Group.Name))
            {
                if (Config.Buff || BuffPerm.Contains(plr.Group.Name)) { SetBuff(plr); };
                if (Config.item || ItemPerm.Contains(plr.Group.Name)) { GetItems(plr); };
                if (Config.Cmd || CmdPerm.Contains(plr.Group.Name)) { Cmd(plr); }
                if (Config.SSC || SSCPerm.Contains(plr.Name)) { SetVip(); }
                if (Config.GodMode || GodPerm.Contains(plr.Group.Name))
                {
                    plr.GodMode = true;
                    Power.SetEnabledState(plr.Index, plr.GodMode);
                }
            }
        }
        #endregion


        #region 玩家注册/加入/离开事件
        //注册根据名单设置特权背包
        private void OnRegister(AccountCreateEventArgs e)
        {
            var SSCPerm = Config.SSCPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var AllPerm = Config.AllPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (e.Account.Name == null || e.Account.Group == "guest" || e.Account.Group == "superadmin") return;

            if ((Config.SSC && SSCPerm.Contains(e.Account.Name)) || AllPerm.Contains(e.Account.Name)) SetVip();
            else SetDefault();
        }

        //加入服务器根据名单设置特权背包，开启无敌
        private void OnJoin(JoinEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.Who];
            if (plr == null || args == null) return;

            var AllPerm = Config.AllPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var SSCPerm = Config.SSCPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var GodPerm = Config.GodPerm.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var Power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();

            if (Config.PlayersList.Contains(plr.Name) || AllPerm.Contains(plr.Group.Name))
            {
                if (Config.SSC || SSCPerm.Contains(plr.Name)) { SetVip(); }
                if (Config.GodMode || GodPerm.Contains(plr.Group.Name))
                {
                    plr.GodMode = true;
                    Power.SetEnabledState(plr.Index, plr.GodMode);
                }
            }
            else SetDefault();
        }

        //任何人离开服务器 恢复SSC默认背包
        private void OnLeave(LeaveEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.Who];
            if (plr == null || args == null) return;
            else SetDefault();
        }
        #endregion


        #region 设置玩家强制开荒背包方法
        //设置特权背包
        private void SetVip()
        {
            var SSCConfig = TShock.ServerSideCharacterConfig.Settings;
            foreach (SSCData i in Config.SSCLists)
            {
                SSCConfig.StartingHealth = i.Health;
                SSCConfig.StartingMana = i.Mana;
                SSCConfig.StartingInventory = i.Inventory;
            }
        }

        //设置默认背包
        private void SetDefault()
        {
            var SSCConfig = TShock.ServerSideCharacterConfig.Settings;
            foreach (SSCData2 i in Config.SSCLists2)
            {
                SSCConfig.StartingHealth = i.Health;
                SSCConfig.StartingMana = i.Mana;
                SSCConfig.StartingInventory = i.Inventory;
            }
        }
        #endregion


        #region 设置BUFF、给物品、执行命令
        //设置永久BUFF支持-1写法
        private void SetBuff(TSPlayer plr)
        {
            var timeLimit = (int.MaxValue / 60 / 60) - 1;
            foreach (BuffData i in Config.BuffList)
            {
                int id = i.ID;
                int time = i.Min;
                if (time < 0 || time > timeLimit) time = timeLimit;
                plr.SetBuff(id, time * 60 * 60);
            }
        }

        //用临时超管权限让玩家执行命令
        private void Cmd(TSPlayer plr)
        {
            Group group = plr.Group;
            try
            {
                plr.Group = new SuperAdminGroup();
                foreach (var cmd in Config.CmdList)
                {
                    Commands.HandleCommand(plr, cmd);
                }
            }
            finally
            {
                plr.Group = group;
            }
        }

        //给物品
        private void GetItems(TSPlayer plr)
        {
            foreach (var itemData in Config.ItemList)
            {
                int id = itemData.ID;
                int stack = itemData.Stack;
                plr.GiveItem(id, stack);
            }
        }
        #endregion

    }
}