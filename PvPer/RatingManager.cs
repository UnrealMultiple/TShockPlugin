namespace PvPer
{
    public static class RatingManager
    {
        // 简化版Glicko-2，由于本插件旨在单次对决后计算及更新玩家评级，故不包含求和运算符。
        // 如欲了解下方代码的详细原理，请参阅 http://www.glicko.net/glicko/glicko2.pdf 

        public static double ComputeEstimatedVariance(DPlayer plr, DPlayer enemy)
        {
            return G(enemy.RatingDeviation) * E(plr.Rating, enemy.Rating, enemy.RatingDeviation) * (1 - E(plr.Rating, enemy.Rating, enemy.RatingDeviation));
        }

        public static double ComputeEstimatedImprovement(DPlayer plr, DPlayer enemy, double score)
        {
            return ComputeEstimatedVariance(plr, enemy) * G(enemy.RatingDeviation) * (score - E(plr.Rating, enemy.Rating, enemy.RatingDeviation));
        }

        private static double G(double phi)
        {
            return 1 / (1 + 3 * phi * phi / (Math.PI * Math.PI));
        }

        private static double E(double mu, double enemyMu, double enemyPhi)
        {
            return 1 / (1 + Math.Exp(-G(enemyPhi) * (mu - enemyMu)));
        }
    }

}