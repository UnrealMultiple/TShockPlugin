using MonoMod.Utils;
using System.Reflection;
using System.Text;
using TShockAPI;

namespace LazyAPI.Commands;
internal class FlexibleCommand : CommandBase
{
    private readonly CommandParser.Parser[] argParsers;
    private readonly (CommandParser.Parser, object?)[] defaultParsers;
    private readonly FastReflectionHelper.FastInvoker method;

    public FlexibleCommand(MethodInfo method, string infoPrefix) : base(method)
    {
        var param = method.GetParameters();
        var ap = new List<CommandParser.Parser>();
        var dp = new List<(CommandParser.Parser, object?)>();
        var sb = new StringBuilder();
        sb.Append(infoPrefix);

        foreach (var p in param.Skip(1))
        {
            if (p.IsOptional)
            {
                dp.Add((CommandParser.GetParser(p.ParameterType), p.DefaultValue));
                sb.Append(@$"[{p.Name}：{CommandParser.GetFriendlyName(p.ParameterType)}] ");
            }
            else
            {
                ap.Add(CommandParser.GetParser(p.ParameterType));
                sb.Append($"<{p.Name}：{CommandParser.GetFriendlyName(p.ParameterType)}> ");
            }
        }
        this.argParsers = [.. ap];
        this.defaultParsers = [.. dp];
        this.usage = CommandHelper.GetCommandUsage(method) ?? sb.ToString();
        this.method = method.GetFastInvoker();
    }

    public override ParseResult TryParse(CommandArgs args, int current)
    {
        var p = args.Parameters;
        var n = this.argParsers.Length;
        var d = this.defaultParsers.Length;
        if (p.Count < n + current)
        {
            return this.GetResult(Math.Abs(n + current - p.Count));
        }

        var a = new object?[n + d + 1];
        a[0] = args;
        var unmatched = this.argParsers.Where((t, i) => !t(p[current + i], out a[i + 1])).Count();
        if (unmatched != 0)
        {
            return this.GetResult(unmatched);
        }
        if (this.defaultParsers.Where((t, i) =>
        {
            var (k, v) = t;
            if (p.Count <= n + i + 1)
            {
                a[n + i + 1] = v;
                return false;
            }
            else
            {
                return !k(p[current + n + i], out a[n + i + 1]);
            }
        }).Any())
        {
            return this.GetResult(0);
        }
        if (this.CheckPlayer(args.Player))
        {
            this.method(null, a);
        }

        return this.GetResult(0);
    }
}
