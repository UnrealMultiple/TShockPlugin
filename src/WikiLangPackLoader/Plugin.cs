using GetText;
using Localizer;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using ReLogic.Content.Sources;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Utils = Terraria.Utils;

namespace WikiLangPackLoader;

[ApiVersion(2, 1)]
public class WikiLangPackLoader : TerrariaPlugin
{
    public WikiLangPackLoader(Main game) : base(game)
    {
        // ReSharper disable once RedundantBaseQualifier
        base.Order = int.MinValue;
    }
    public override string Author => "Cai";

    public override string Description => GetString("加载Wiki语言包");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (2025, 07, 19, 0);

    private readonly Hook _langHook = new (typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
        "TranslationCultureInfo",
        BindingFlags.NonPublic | BindingFlags.Static)!.GetGetMethod(nonPublic:true)!, ()=>  new CultureInfo("zh-CN"));
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnGamePostInitialize, int.MinValue);
        GeneralHooks.ReloadEvent += this.GeneralHooks_ReloadEvent;
        if (LanguageManager.Instance.ActiveCulture != GameCulture.FromLegacyId(7))
        {
            LanguageManager.Instance.SetLanguage(7);
        }
        this._langHook.Apply();
        var path = (string)typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationsDirectory",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetField(
            "C",
            BindingFlags.Public | BindingFlags.Static)!.SetValue(null, new Catalog("TShockAPI", path, new CultureInfo("zh-CN")));
    }

    private void OnGamePostInitialize(EventArgs args)
    {
        this.Load();
    }

    private void GeneralHooks_ReloadEvent(ReloadEventArgs e)
    {
        this.Load();
    }

    

    private void Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var services = new GameServiceContainer();
        const string resourceName = "WikiLangPackLoader.ResourcePack.zip";
        const string filePath = @"tshock/LangResourcePack.zip";
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
            this._langHook.Dispose();
            GeneralHooks.ReloadEvent -= this.GeneralHooks_ReloadEvent;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnGamePostInitialize);
        }

        base.Dispose(disposing);
    }
}