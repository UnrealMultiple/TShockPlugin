using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace GroundCraft;

public sealed partial class GroundCraft
{
    private void SpawnCraftEffect(Vector2 center)
    {
        NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 2, (int)center.X, (int)center.Y, 0f, 2);
    }

    private static void SpawnZenithFinale(Vector2 center)
    {
        for (int color = 0; color < 3; color++)
        {
            float angle = MathHelper.TwoPi * color / 3f;
            Vector2 point = center + new Vector2(MathF.Cos(angle) * 18f, MathF.Sin(angle) * 10f);
            NetMessage.SendData(MessageID.SpecialFX, -1, -1, null, 2, (int)point.X, (int)point.Y, 0f, color);
        }
    }
}
