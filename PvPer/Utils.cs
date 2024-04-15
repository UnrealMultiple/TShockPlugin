using TShockAPI;

namespace PvPer
{
    public class Utils
    {
        public static bool IsPlayerInADuel(int playerIndex)
        {
            foreach (Pair p in PvPer.ActiveDuels)
            {
                if (p.Player1 == playerIndex || p.Player2 == playerIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public static Pair? GetInvitationFromSenderIndex(int playerIndex)
        {
            foreach (Pair p in PvPer.Invitations)
            {
                if (p.Player1 == playerIndex)
                {
                    return p;
                }
            }

            return null;
        }

        public static Pair? GetInvitationFromReceiverIndex(int playerIndex)
        {
            foreach (Pair p in PvPer.Invitations)
            {
                if (p.Player2 == playerIndex)
                {
                    return p;
                }
            }

            return null;
        }

        public static Pair? GetDuel(int playerIndex)
        {
            foreach (Pair p in PvPer.ActiveDuels)
            {
                if (p.Player1 == playerIndex || p.Player2 == playerIndex)
                {
                    return p;
                }
            }

            return null;
        }

        public static bool IsPlayerInArena(TSPlayer player)
        {
            return player.TileX > PvPer.Config.ArenaPosX1 &&
                   player.TileY > PvPer.Config.ArenaPosY1 &&
                   player.TileX < PvPer.Config.ArenaPosX2 &&
                   player.TileY < PvPer.Config.ArenaPosY2;
        }

        public static bool IsLocationInArena(int x, int y) {
            return x > PvPer.Config.ArenaPosX1 &&
                   y > PvPer.Config.ArenaPosY1 &&
                   x < PvPer.Config.ArenaPosX2 &&
                   y < PvPer.Config.ArenaPosY2;
        }
    }
}