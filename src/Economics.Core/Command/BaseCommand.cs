using System.Reflection;
using TShockAPI;

namespace Economics.Core.Command;

public class BaseCommand
{
    public virtual string[] Alias { get; set; } = [];

    public virtual List<string> Permissions { get; set; } = [];

    public virtual string HelpText { get; set; } = string.Empty;

    public virtual string ErrorText { get; set; } = string.Empty;

    private readonly BindingFlags _flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;

    private readonly Dictionary<string, SubCommandExtra> SubCommands = [];

    public BaseCommand()
    {
        this.SubCommands = this.GetType()
            .GetMethods(this._flag)
            .Where(m => m.IsDefined(typeof(SubCommandAttribute), true))
            .Select(m =>
                new SubCommandExtra(m,
                    m.GetCustomAttribute<SubCommandAttribute>()!,
                    m.GetCustomAttribute<OnlyPlayerAttribute>(),
                    m.GetCustomAttribute<CommandPermissionAttribute>(),
                    m.GetCustomAttribute<HelpTextAttribute>()
                    )
                )
            .ToDictionary(s => s.SubCommand.Subname.ToLower());
    }

    public virtual void Invoke(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendErrorMessage(GetString(this.ErrorText));
            return;
        }
        var subcmd = args.Parameters[0].ToLower();
        if (!this.SubCommands.TryGetValue(subcmd, out var extar))
        { 
            args.Player.SendErrorMessage(GetString(this.ErrorText));
            return;
        }
        if (extar.OnlyPlayer != null && !args.Player.RealPlayer && !args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage(GetString("你只能在游戏中使用此命令!"));
            return;
        }
        if (extar.CommandPermission != null && !extar.CommandPermission.DetectPermission(args.Player))
        {
            args.Player.SendErrorMessage(GetString("你无权使用此命令!"));
            return;
        }
        if (extar.SubCommand.Length > args.Parameters.Count)
        {
            args.Player.SendErrorMessage(extar.HelpText != null ? GetString($"语法错误，正确语法:{extar.HelpText.Text}") : GetString(this.ErrorText));
            return;
        }
        extar.Method.Invoke(null, [args]);
    }
}

public record SubCommandExtra(MethodInfo Method, SubCommandAttribute SubCommand, OnlyPlayerAttribute? OnlyPlayer, CommandPermissionAttribute? CommandPermission, HelpTextAttribute? HelpText);
