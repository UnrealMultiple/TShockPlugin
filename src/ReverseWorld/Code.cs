using Global;
using Terraria;
using TShockAPI;

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
                TileSection range = new TileSection(0, (int)Main.rockLayer, Main.maxTilesX, Main.maxTilesY - 200 - (int)Main.rockLayer);
                Replenisher.Replenish(result, range, result * 5);
                range.UpdateToPlayer();
                player.SendSuccessMessage($"{result}个地雷已放置");
            }
            catch (Exception arg)
            {
                player.SendErrorMessage($"失败：{arg}");
            }
        }
    }
}

