using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;
using Terraria.Localization;

namespace CaiLib
{
    public static class CaiCommand
    {
        public static void DelCommand(List<string> names)
        {
            Commands.ChatCommands.RemoveAll(c => c.Names.Any(n => names.Contains(n)));
        }
        public static void DelCommand(string names)
        {
            Commands.ChatCommands.RemoveAll(c => c.Names.Contains(names));
        }
        public static void AddCommand(List<string> perms, CommandDelegate commandDelegate, List<string> names, bool replaceCommand = false, string helpText = null,string[] helpDesc = null, bool allowServer = true, bool doLog = true)
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
