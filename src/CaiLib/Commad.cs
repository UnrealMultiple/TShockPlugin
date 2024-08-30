using TShockAPI;

namespace CaiLib
{
    public static class CaiCommand
    {
        /// <summary>
		/// 删除指定命令
		/// </summary>
		/// <param name="names">命令名字</param>
        public static void DelCommand(List<string> names)
        {
            Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
        }
        /// <summary>
		/// 删除指定命令
		/// </summary>
		/// <param name="names">命令名字</param>
        public static void DelCommand(string names)
        {
            Commands.ChatCommands.RemoveAll(c => c.Names.Contains(names));
        }
        /// <summary>
		/// 添加一条命令
		/// </summary>
		/// <param name="perms">权限</param>
        /// <param name="commandDelegate">命令回调函数</param>
        /// <param name="names">命令名字</param>
        /// <param name="replaceCommand">替换原命令</param>
        /// <param name="helpText">帮助提示</param>
        /// <param name="helpDesc">帮助文档</param>
        /// <param name="allowServer">允许非玩家</param>
        /// <param name="doLog">日志记录参数</param>
        public static void AddCommand(List<string> perms, CommandDelegate commandDelegate, List<string> names, bool replaceCommand = false, string helpText = null, string[] helpDesc = null, bool allowServer = true, bool doLog = true)
        {
            if (replaceCommand)
            {
                Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
            }
            Commands.ChatCommands.Add(new Command(perms, commandDelegate, names.ToArray())
            {
                AllowServer = allowServer,
                HelpDesc = helpDesc,
                HelpText = helpText,
                DoLog = doLog,
            });
        }
        /// <summary>
		/// 添加一条命令
		/// </summary>
		/// <param name="perms">权限</param>
        /// <param name="commandDelegate">命令回调函数</param>
        /// <param name="names">命令名字</param>
        /// <param name="replaceCommand">替换原命令</param>
        /// <param name="helpText">帮助提示</param>
        /// <param name="helpDesc">帮助文档</param>
        /// <param name="allowServer">允许非玩家</param>
        /// <param name="doLog">日志记录参数</param>
        public static void AddCommand(string perms, CommandDelegate commandDelegate, List<string> names, bool replaceCommand = false, string helpText = null, string[] helpDesc = null, bool allowServer = true, bool doLog = true)
        {
            if (replaceCommand)
            {
                Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
            }
            Commands.ChatCommands.Add(new Command(perms, commandDelegate, names.ToArray())
            {
                AllowServer = allowServer,
                HelpDesc = helpDesc,
                HelpText = helpText,
                DoLog = doLog,
            });
        }
        /// <summary>
		/// 添加一条命令
		/// </summary>
		/// <param name="perms">权限</param>
        /// <param name="commandDelegate">命令回调函数</param>
        /// <param name="names">命令名字</param>
        /// <param name="replaceCommand">替换原命令</param>
        /// <param name="helpText">帮助提示</param>
        /// <param name="helpDesc">帮助文档</param>
        /// <param name="allowServer">允许非玩家</param>
        /// <param name="doLog">日志记录参数</param>
        public static void AddCommand(List<string> perms, CommandDelegate commandDelegate, string names, bool replaceCommand = false, string helpText = null, string[] helpDesc = null, bool allowServer = true, bool doLog = true)
        {
            if (replaceCommand)
            {
                Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
            }
            Commands.ChatCommands.Add(new Command(perms, commandDelegate, names)
            {
                AllowServer = allowServer,
                HelpDesc = helpDesc,
                HelpText = helpText,
                DoLog = doLog,
            });
        }
        /// <summary>
		/// 添加一条命令
		/// </summary>
		/// <param name="perms">权限</param>
        /// <param name="commandDelegate">命令回调函数</param>
        /// <param name="names">命令名字</param>
        /// <param name="replaceCommand">替换原命令</param>
        /// <param name="helpText">帮助提示</param>
        /// <param name="helpDesc">帮助文档</param>
        /// <param name="allowServer">允许非玩家</param>
        /// <param name="doLog">日志记录参数</param>
        public static void AddCommand(string perms, CommandDelegate commandDelegate, string names, bool replaceCommand = false, string helpText = null, string[] helpDesc = null, bool allowServer = true, bool doLog = true)
        {
            if (replaceCommand)
            {
                Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
            }
            Commands.ChatCommands.Add(new Command(perms, commandDelegate, names)
            {
                AllowServer = allowServer,
                HelpDesc = helpDesc,
                HelpText = helpText,
                DoLog = doLog,
            });
        }
    }
}
