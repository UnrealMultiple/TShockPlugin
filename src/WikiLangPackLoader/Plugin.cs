using Microsoft.Xna.Framework;
using On.Terraria.Localization;
using ReLogic.Content.Sources;
using System.IO.Compression;
using System.Reflection;
using Terraria;
using Terraria.IO;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using GameCulture = Terraria.Localization.GameCulture;

namespace WikiLangPackLoader;

[ApiVersion(2, 1)]
public class WikiLangPackLoader : TerrariaPlugin
{
    public WikiLangPackLoader(Main game)
        : base(game)
    {
        this.Order = int.MaxValue;
    }

    public override string Author => "Cai";

    public override string Description => GetString("加载Wiki语言包");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2025, 05, 18, 2);

    public override void Initialize()
    {
        LanguageManager.SetLanguage_GameCulture += this.LanguageManager_SetLanguage_GameCulture;
        //On.Terraria.Localization.LanguageManager.SetLanguage_int += LanguageManager_SetLanguage_int;
        //On.Terraria.Localization.LanguageManager.SetLanguage_string += LanguageManager_SetLanguage_string;
        Terraria.Localization.LanguageManager.Instance.SetLanguage(7);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnGamePostInitialize, int.MinValue);
        GeneralHooks.ReloadEvent += this.GeneralHooks_ReloadEvent;
    }

    private void OnGamePostInitialize(EventArgs args)
    {
        this.Load();
    }

    private void GeneralHooks_ReloadEvent(ReloadEventArgs e)
    {
        this.Load();
    }


    private void LanguageManager_SetLanguage_GameCulture(LanguageManager.orig_SetLanguage_GameCulture orig, Terraria.Localization.LanguageManager self, GameCulture culture)
    {
        orig(self, culture);
        if (culture.LegacyId == 7)
        {
            this.Load();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red; // 设置前景色为红色
            Console.BackgroundColor = ConsoleColor.Yellow; // 设置背景色为黄色
            Console.WriteLine("\n[中文Wiki语言包加载器]语言包跳过加载：Terraria处于非中文状态，可能是插件修改导致");
            Console.ResetColor(); // 重置为默认颜色
        }
    }

    private void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var services = new GameServiceContainer();
        var resourceName = "WikiLangPackLoader.ResourcePack.zip";
        var filePath = @"tshock/LangResourcePack.zip";
        using (var resourceStream = assembly.GetManifestResourceStream(resourceName)!)
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            resourceStream.CopyTo(fileStream);
        }

        Utils.TryCreatingDirectory(@"tshock/LangResourcePack/");
        ZipFile.ExtractToDirectory(filePath, @"tshock/LangResourcePack/", true);
        File.Delete(filePath);
        var pack = new ResourcePack(services, @"tshock/LangResourcePack/");
        var list = new List<IContentSource> { pack.GetContentSource() };
        Terraria.Localization.LanguageManager.Instance.UseSources(list);

        Console.ForegroundColor = ConsoleColor.Green; // 设置前景色为红色
        Console.WriteLine("[中文Wiki语言包加载器]语言包已经加载！\n" +
                          $"作者：{pack.Author}\n" +
                          $"版本：{pack.Version.Major}.{pack.Version.Minor}");

        Console.ResetColor(); // 重置为默认颜色
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LanguageManager.SetLanguage_GameCulture -= this.LanguageManager_SetLanguage_GameCulture;
            //On.Terraria.Localization.LanguageManager.SetLanguage_int += LanguageManager_SetLanguage_int;
            //On.Terraria.Localization.LanguageManager.SetLanguage_string += LanguageManager_SetLanguage_string;
            GeneralHooks.ReloadEvent -= this.GeneralHooks_ReloadEvent;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnGamePostInitialize);
        }

        base.Dispose(disposing);
    }
}