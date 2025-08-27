using System.Reflection;
using TShockAPI;

namespace LazyAPI.Commands;

internal partial class Command : CommandBase
{
    private readonly Dictionary<string, List<CommandBase>> _dict = [];
    private readonly List<CommandBase> _main = [];
    private readonly string _infoPrefix;

    public Command(MemberInfo type, string infoPrefix) : base(type)
    {
        this.usage = CommandHelper.GetCommandUsage(type) ?? $"{infoPrefix} <...>";
        this._infoPrefix = infoPrefix;
    }

    public void PostBuildTree()
    {
        this._main.Add(new HelpCommand(this, this._infoPrefix));
    }

    public void Add(string? cmd, CommandBase sub)
    {
        if (string.IsNullOrEmpty(cmd))
        {
            this._main.Add(sub);
        }
        else
            if (this._dict.TryGetValue(cmd, out var lst))
        {
            lst.Add(sub);
        }
        else
        {
            this._dict.Add(cmd,
            [
                sub
            ]);
        }
    }

    public override ParseResult TryParse(CommandArgs args, int current)
    {
        if (!this.CheckPlayer(args.Player))
        {
            return this.GetResult(0);
        }

        var most = this.GetResult(args.Parameters.Count - current + 1);

        if (current < args.Parameters.Count && this._dict.TryGetValue(args.Parameters[current], out var subs))
        {
            foreach (var sub in subs)
            {
                var res = sub.TryParse(args, current + 1);
                if (res.unmatched == 0)
                {
                    return res;
                }

                if (res.unmatched < most.unmatched)
                {
                    most = res;
                }
            }
        }

        foreach (var sub in this._main)
        {
            var res = sub.TryParse(args, current);
            if (res.unmatched == 0)
            {
                return res;
            }

            if (res.unmatched < most.unmatched)
            {
                most = res;
            }
        }

        return most;
    }
}