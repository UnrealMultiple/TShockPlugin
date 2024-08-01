using System.IO.Compression;
using System.Reflection;
using Microsoft.Xna.Framework;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace WikiLangPackLoader
{
    [ApiVersion(2, 1)]
    public class WikiLangPackLoader : TerrariaPlugin
    {

        public override string Author => "Cai";

        public override string Description => "加载Wiki语言包";

        public override string Name => "中文Wiki语言包加载器";
        public override Version Version => new Version(1, 2, 0, 0);

        public WikiLangPackLoader(Main game)
        : base(game)
        {
            Order = int.MaxValue;
        }
        public override void Initialize()
        {

            On.Terraria.Localization.LanguageManager.SetLanguage_GameCulture += LanguageManager_SetLanguage_GameCulture;
            //On.Terraria.Localization.LanguageManager.SetLanguage_int += LanguageManager_SetLanguage_int;
            //On.Terraria.Localization.LanguageManager.SetLanguage_string += LanguageManager_SetLanguage_string;
            LanguageManager.Instance.SetLanguage(7);
            GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
        }

        private void GeneralHooks_ReloadEvent(ReloadEventArgs e)
        {
            Load();
        }



        private void LanguageManager_SetLanguage_GameCulture(On.Terraria.Localization.LanguageManager.orig_SetLanguage_GameCulture orig, LanguageManager self, GameCulture culture)
        {

            orig(self, culture);
            if (culture.LegacyId == 7)
                Load();
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
            Assembly assembly = Assembly.GetExecutingAssembly();
            var services = new GameServiceContainer();
            string resourceName = "WikiLangPackLoader.ResourcePack.zip";
            string filePath = @"tshock/LangResourcePack.zip";
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }
            Utils.TryCreatingDirectory(@"tshock/LangResourcePack/");
            ZipFile.ExtractToDirectory(filePath, @"tshock/LangResourcePack/", true);
            File.Delete(filePath);
            var pack = new ResourcePack(services, @"tshock/LangResourcePack/");
            List<IContentSource> list = new List<IContentSource>
            {
                pack.GetContentSource()
            };
            LanguageManager.Instance.UseSources(list);

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
                On.Terraria.Localization.LanguageManager.SetLanguage_GameCulture -= LanguageManager_SetLanguage_GameCulture;
                //On.Terraria.Localization.LanguageManager.SetLanguage_int += LanguageManager_SetLanguage_int;
                //On.Terraria.Localization.LanguageManager.SetLanguage_string += LanguageManager_SetLanguage_string;
                GeneralHooks.ReloadEvent -= GeneralHooks_ReloadEvent;
            }
            base.Dispose(disposing);
        }


    }
}
