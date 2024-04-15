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

        public static bool IsPlayerInArena(TSPlayer player)
        {
            return player.X >= PvPer.Config.ArenaPosX1 * 16 &&
                   player.Y >= PvPer.Config.ArenaPosY1 * 16 &&
                   player.X <= PvPer.Config.ArenaPosX2 * 16 &&
                   player.Y <= PvPer.Config.ArenaPosY2 * 16;
        }

        public static bool IsLocationInArena(int x, int y)
        {
            return x >= PvPer.Config.ArenaPosX1 * 16 &&
                   y >= PvPer.Config.ArenaPosY1 * 16 &&
                   x <= PvPer.Config.ArenaPosX2 * 16 &&
                   y <= PvPer.Config.ArenaPosY2 * 16;
        }

    }
}