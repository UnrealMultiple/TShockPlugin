using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace DumpTerrariaID
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "少司命";
        public override string Description => "DumpID";
        public override string Name => "DumpTerrariaID";
        public override Version Version => new(1, 0, 0, 1);
        public Dump Dump = new();
        public readonly Dictionary<int, string> EnglishBuffs = new();
        public readonly Dictionary<int, string> Prefixs = new();
        public readonly Dictionary<int, string> Projecttile = new();
        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            SaveName();
            Commands.ChatCommands.Add(new Command("dump.id", DumpID, "dumpid"));

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == DumpID);
                EnglishBuffs.Clear();
                Projecttile.Clear();
                Prefixs.Clear();
            }

            base.Dispose(disposing);
        }

        public void SaveName()
        {
            Terraria.Localization.LanguageManager.Instance.SetLanguage((int)Terraria.Localization.GameCulture.CultureName.English);
            for (int i = 0; i < BuffID.Count; i++)
            {
                EnglishBuffs[i] = Lang.GetBuffName(i);
            }

            for (int i = 0; i < ProjectileID.Count; i++)
            {
                Projecttile.Add(i, Lang.GetProjectileName(i).Value);
            }

            Terraria.Localization.LanguageManager.Instance.SetLanguage((int)Terraria.Localization.GameCulture.CultureName.Chinese);
            for (int i = 0; i < Lang.prefix.Count(); i++)
            {
                Prefixs[i] = TShock.Utils.GetPrefixById(i);
            }
        }

        private void DumpID(CommandArgs args)
        {
            for (int i = 0; i < ItemID.Count; i++)
            {
                Dump.ItemTable.Add(new Project()
                {
                    ID = i,
                    Chains = Lang.GetItemName(i).Value,
                    English = TShockAPI.Localization.EnglishLanguage.GetItemNameById(i)
                });
            }

            for (int i = 0; i < BuffID.Count; i++)
            {
                Dump.BuffTable.Add(new Project()
                {
                    ID = i,
                    Chains = Lang.GetBuffName(i),
                    English = EnglishBuffs[i]
                });
            }

            for (int i = 0; i < NPCID.Count; i++)
            {
                Dump.NpcTable.Add(new Project()
                {
                    ID = i,
                    Chains = Lang.GetNPCName(i).Value,
                    English = TShockAPI.Localization.EnglishLanguage.GetNpcNameById(i)
                });
            }

            for (int i = 0; i < ProjectileID.Count; i++)
            {
                Dump.ProjecttileTable.Add(new Project()
                {
                    ID = i,
                    Chains = Lang.GetProjectileName(i).Value,
                    English = Projecttile[i]
                });
            }

            foreach (var prefix in Prefixs)
            {
                Dump.PrefixTable.Add(new Project()
                {
                    ID = prefix.Key,
                    Chains = prefix.Value,
                    English = TShockAPI.Localization.EnglishLanguage.GetPrefixById(prefix.Key)
                });
            }
            File.WriteAllText(Path.Combine(TShock.SavePath, "TerrariaID.json"), Newtonsoft.Json.JsonConvert.SerializeObject(Dump, Newtonsoft.Json.Formatting.Indented));
            args.Player.SendSuccessMessage("写入成功");
        }
    }
}