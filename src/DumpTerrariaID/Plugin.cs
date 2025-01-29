using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace DumpTerrariaID;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";
    public override string Description => GetString("DumpID");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 4);
    public Dump Dump = new();
    public readonly Dictionary<int, string> EnglishBuffs = new();
    public readonly Dictionary<int, string> Prefixs = new();
    public readonly Dictionary<int, string> Projecttile = new();
    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.SaveName();
        Commands.ChatCommands.Add(new Command("dump.id", this.DumpID, "dumpid"));

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.DumpID);
            this.EnglishBuffs.Clear();
            this.Projecttile.Clear();
            this.Prefixs.Clear();
        }

        base.Dispose(disposing);
    }

    public void SaveName()
    {
        Terraria.Localization.LanguageManager.Instance.SetLanguage((int) Terraria.Localization.GameCulture.CultureName.English);
        for (var i = 0; i < BuffID.Count; i++)
        {
            this.EnglishBuffs[i] = Lang.GetBuffName(i);
        }

        for (var i = 0; i < ProjectileID.Count; i++)
        {
            this.Projecttile.Add(i, Lang.GetProjectileName(i).Value);
        }

        Terraria.Localization.LanguageManager.Instance.SetLanguage((int) Terraria.Localization.GameCulture.CultureName.Chinese);
        for (var i = 0; i < Lang.prefix.Count(); i++)
        {
            this.Prefixs[i] = TShock.Utils.GetPrefixById(i);
        }
    }

    private void DumpID(CommandArgs args)
    {
        for (var i = 0; i < ItemID.Count; i++)
        {
            this.Dump.ItemTable.Add(new Project()
            {
                ID = i,
                Chains = Lang.GetItemName(i).Value,
                English = TShockAPI.Localization.EnglishLanguage.GetItemNameById(i)
            });
        }

        for (var i = 0; i < BuffID.Count; i++)
        {
            this.Dump.BuffTable.Add(new Project()
            {
                ID = i,
                Chains = Lang.GetBuffName(i),
                English = this.EnglishBuffs[i]
            });
        }

        for (var i = 0; i < NPCID.Count; i++)
        {
            this.Dump.NpcTable.Add(new Project()
            {
                ID = i,
                Chains = Lang.GetNPCName(i).Value,
                English = TShockAPI.Localization.EnglishLanguage.GetNpcNameById(i)
            });
        }

        for (var i = 0; i < ProjectileID.Count; i++)
        {
            this.Dump.ProjecttileTable.Add(new Project()
            {
                ID = i,
                Chains = Lang.GetProjectileName(i).Value,
                English = this.Projecttile[i]
            });
        }

        foreach (var prefix in this.Prefixs)
        {
            this.Dump.PrefixTable.Add(new Project()
            {
                ID = prefix.Key,
                Chains = prefix.Value,
                English = TShockAPI.Localization.EnglishLanguage.GetPrefixById(prefix.Key)
            });
        }
        File.WriteAllText(Path.Combine(TShock.SavePath, "TerrariaID.json"), Newtonsoft.Json.JsonConvert.SerializeObject(this.Dump, Newtonsoft.Json.Formatting.Indented));
        args.Player.SendSuccessMessage(GetString("写入成功"));
    }
}