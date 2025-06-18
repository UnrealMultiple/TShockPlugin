using TShockAPI;

namespace ServerTools;
public static class CommandExtension
{
    public static void SendPage(this CommandArgs args, IEnumerable<string> pages, int expectedParameterIndex, PaginationTools.Settings settings)
    {
        if (PaginationTools.TryParsePageNumber(args.Parameters, expectedParameterIndex, args.Player, out var pageNumber))
        {
            PaginationTools.SendPage(args.Player, pageNumber, pages, pages.Count(), settings);
        }
    }
}
