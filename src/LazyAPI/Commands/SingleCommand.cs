using MonoMod.Utils;
using System.Reflection;
using System.Text;
using TShockAPI;

namespace LazyAPI.Commands;

internal partial class SingleCommand : CommandBase
{
    private readonly CommandParser.Parser[] argParsers;
    private readonly FastReflectionHelper.FastInvoker method;

    public SingleCommand(MethodInfo method, string infoPrefix) : base(method)
    {
        var param = method.GetParameters();
        var ap = new List<CommandParser.Parser>();
        var sb = new StringBuilder();
        sb.Append(infoPrefix);

        foreach (var p in param.Skip(1))
        {
            ap.Add(CommandParser.GetParser(p.ParameterType));
            sb.Append($"<{p.Name}: {CommandParser.GetFriendlyName(p.ParameterType)}> ");
        }

        this.argParsers = [.. ap];
        this.usage = CommandHelper.GetCommandUsage(method) ?? sb.ToString();
        this.method = method.GetFastInvoker();
    }

    public override ParseResult TryParse(CommandArgs args, int current)
    {
        var p = args.Parameters;
        var n = this.argParsers.Length;
        if (p.Count != n + current)
        {
            return this.GetResult(Math.Abs(n + current - p.Count));
        }

        var a = new object?[n + 1];
        a[0] = args;
        var unmatched = this.argParsers.Where((t, i) => !t(p[current + i], out a[i + 1])).Count();
        if (unmatched != 0)
        {
            return this.GetResult(unmatched);
        }

        if (this.CheckPlayer(args.Player))
        {
            this.method(null, a);
        }

        return this.GetResult(0);
    }
}