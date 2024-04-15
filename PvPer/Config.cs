using Newtonsoft.Json;

namespace PvPer
{
    public class Config
    {
        public int Player1PositionX, Player1PositionY, Player2PositionX, Player2PositionY, ArenaPosX1, ArenaPosY1, ArenaPosX2, ArenaPosY2;
        public void Write()
        {
            File.WriteAllText(PvPer.ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static Config Read()
        {
            Config? newConfig = null;
            if (File.Exists(PvPer.ConfigPath))
            {
                newConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PvPer.ConfigPath));
            }

            if (newConfig == null)
            {
                newConfig = new Config();
            }

            newConfig.Write();
            return newConfig;
        }
    }
}