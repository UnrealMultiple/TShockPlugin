using LazyAPI.Attributes;
using System.Reflection;
using TShockAPI;

namespace LazyAPI.Commands;

internal abstract class CommandBase
{
    protected internal readonly struct ParseResult(CommandBase current, int num)
    {
        public readonly int unmatched = num;
        public readonly CommandBase current = current;
    }

    private static string NoPerm => GetString("You do not have access to this command.");
    private static string MustReal => GetString("You must use this command in-game.");

    protected string[] permissions;
    private readonly bool _realPlayer;
    internal string? usage;

    public abstract ParseResult TryParse(CommandArgs args, int current);
    public override string? ToString()
    {
        return this.usage;
    }

    protected CommandBase(MemberInfo member)
    {
        this.permissions =
        [
            .. member.GetCustomAttributes<Permission>().Select(p => p.Name)
,
            .. member.GetCustomAttributes<PermissionsAttribute>().Select(p => p.perm),
        ];
        if (member.GetCustomAttribute<RealPlayerAttribute>() != null)
        {
            this._realPlayer = true;
        }
    }

    protected CommandBase()
    {
        this.permissions = [];
    }

    public bool CanExec(TSPlayer plr)
    {
        return (!this._realPlayer || plr.RealPlayer) && this.permissions.All(plr.HasPermission);
    }

    protected bool CheckPlayer(TSPlayer plr)
    {
        if (this._realPlayer && !plr.RealPlayer)
        {
            plr.SendErrorMessage(MustReal);
        }
        else if (this.permissions.Any(perm => !plr.HasPermission(perm)))
        {
            plr.SendErrorMessage(NoPerm);
        }
        else
        {
            return true;
        }

        return false;
    }
    protected ParseResult GetResult(int num)
    {
        return new ParseResult(this, num);
    }
}
