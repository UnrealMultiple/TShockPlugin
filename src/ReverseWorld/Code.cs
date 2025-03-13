using Terraria;
using TShockAPI;
namespace ReverseWorld;
public static class Code
{
    public static void Method(TSPlayer player, List<string> args)
    {
        Task.Run((Action)Place);
        void Place()
        {
            try
            {
                if (args.Count <= 0 || !int.TryParse(args[0], out var result))
                {
                    result = 2000;
                }
                var range = new TileSection(0, (int)Main.rockLayer, Main.maxTilesX, Main.maxTilesY - 200 - (int)Main.rockLayer);
                Replenisher.Replenish(result, range, result * 5);
                range.UpdateToPlayer();
                player.SendSuccessMessage(GetString($"{result}个地雷已放置"));
            }
            catch (Exception arg)
            {
                player.SendErrorMessage(GetString($"失败：{arg}"));
            }
        }
    }
}

