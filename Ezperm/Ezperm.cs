using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Ezperm
{
    [ApiVersion(2, 1)]
    public class Ezperm : TerrariaPlugin
    {
        public override string Name => "Ezperm";
        public override string Author => "大豆子,肝帝熙恩优化1449";
        public override string Description => "一个指令帮助小白给初始服务器添加缺失的权限，还可以批量添删权限";
        public override Version Version => new Version(1, 2, 2);
        internal static Configuration Config;
        public Ezperm(Main game) : base(game)
        {
            LoadConfig();

        }
        private static void LoadConfig()
        {

            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);

        }
        private static void ReloadConfig(ReloadEventArgs args)
        {
            LoadConfig();
            args.Player?.SendSuccessMessage("重新加载{0}配置完毕。", typeof(Ezperm).Name);
        }
        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            Commands.ChatCommands.Add(new Command("inperms.admin",Cmd,"inperms","批量改权限"));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Cmd);
                GeneralHooks.ReloadEvent -= ReloadConfig;
            }
            base.Dispose(disposing);
        }
        private void Cmd(CommandArgs args)
        {
            Configuration config = Configuration.Read(Configuration.FilePath);

            foreach (var groupInfo in config.Groups)
            {
                string group = groupInfo.Name;

                // 添加权限
                foreach (var addPermission in groupInfo.AddPermissions)
                {
                    string addCommand = $"/group addperm {group} {addPermission}";
                    Commands.HandleCommand(TSPlayer.Server, addCommand);
                }

                // 删除权限
                foreach (var delPermission in groupInfo.DelPermissions)
                {
                    string delCommand = $"/group delperm {group} {delPermission}";
                    Commands.HandleCommand(TSPlayer.Server, delCommand);
                }

                // 修改这行，显示具体的组和权限信息
                args.Player.SendSuccessMessage($"成功为组 {group} 添加权限: {string.Join(", ", groupInfo.AddPermissions)}");
                args.Player.SendSuccessMessage($"成功为组 {group} 删除权限: {string.Join(", ", groupInfo.DelPermissions)}");
            }
        }
    }
}
