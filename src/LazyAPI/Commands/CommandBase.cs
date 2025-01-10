using LazyAPI.Attributes;
using System.Reflection;
using TShockAPI;

namespace LazyAPI.Commands;

internal abstract class CommandBase
{
    protected internal struct ParseResult
    {
        public readonly int unmatched;
        public readonly CommandBase current;

        public ParseResult(CommandBase current, int num)
        {
            this.current = current;
            this.unmatched = num;
        }
    }

    private string NoPerm => GetString("You do not have access to this command.");
    private string MustReal => GetString("You must use this command in-game.");

    protected string[] permissions;
    private readonly bool _realPlayer;
    protected string? info;

    public abstract ParseResult TryParse(CommandArgs args, int current);
    public override string? ToString()
    {
        return this.info;
    }

    protected CommandBase(MemberInfo member)
    {
        this.permissions = member.GetCustomAttributes<Permission>().Select(p => p.Name)
            .Concat(member.GetCustomAttributes<PermissionsAttribute>().Select(p => p.perm)).ToArray();
        if (member.GetCustomAttribute<RealPlayerAttribute>() != null)
        {
            this._realPlayer = true;
        }
    }

    protected CommandBase()
    {
        this.permissions = Array.Empty<string>();
    }

    public bool CanExec(TSPlayer plr)
    {
        return (!this._realPlayer || plr.RealPlayer) && this.permissions.All(plr.HasPermission);
    }

    protected bool CheckPlayer(TSPlayer plr)
    {
        if (this._realPlayer && !plr.RealPlayer)
        {
            plr.SendErrorMessage(this.MustReal);
        }
        else if (this.permissions.Any(perm => !plr.HasPermission(perm)))
        {
            plr.SendErrorMessage(this.NoPerm);
        }
        else
        {
            return true;
        }

        return false;
    }
    protected ParseResult GetResult(int num)
    {
        return new(this, num);
    }
}
