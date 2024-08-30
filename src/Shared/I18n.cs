global using static Localizer.I18n;

using GetText;
using System.Globalization;
using System.Reflection;
using TShockAPI;

namespace Localizer;

// ReSharper disable once InconsistentNaming
internal static class I18n
{
    // ReSharper disable once InconsistentNaming
    private static readonly Catalog C = GetCatalog();

    private static Catalog GetCatalog()
    {
        var cultureInfo = (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationCultureInfo",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        var asm = Assembly.GetExecutingAssembly();
        var moFilePath = $"i18n.{cultureInfo.Name}.mo";
/* Unmerged change from project 'EconomicsAPI'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.RPG'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Deal'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Shop'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Skill'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CreateSpawn'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AutoBroadcast'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AutoTeam'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'LifemaxExtra'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BridgeBuilder'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Back'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BanNpc'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'MapTp'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RandRespawn'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RainbowChat'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CGive'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DisableGodMod'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DisableSurfaceProjectiles'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'NormalDropsBags'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'OnlineGiftPackage'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RecipesBrowser'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'TownNPCHomes'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RegionView'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Noagent'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SwitchCommands'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'GolfRewards'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ProgressBag'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'PermaBuff'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DataSync'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ProgressRestrict'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CriticalHit'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'PacketsStop'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DeathDrop'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'PerPlayerLoot'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'PvPer'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DamageStatistic'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DumpTerrariaID'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AdditionalPylons'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'History'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Invincibility'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Ezperm'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Autoclear'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'EssentialsPlus'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ShowArmors'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'VeinMiner'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'PersonalPermission'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ItemPreserver'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Regain'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SimultaneousUseFix'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Challenger'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'MiniGamesAPI'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BuildMaster'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'journeyUnlock'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'WikiLangPackLoader'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ListPlugins'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BagPing'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ServerTools'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Platform'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CaiLib'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'GenerateMap'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RestInventory'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'HelpPlus'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ShortCommand'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DTEntryBlock'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CaiBot'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'HouseRegion'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SignInSign'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'WeaponPlus'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Projectile'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.NPC'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.Task'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RolesModifying'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Economics.WeaponPlus'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Respawn'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'EndureBoost'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AnnouncementBoxPlus'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ConsoleSql'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ProgressControls'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RealTime'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'GoodNight'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'MusicPlayer'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'TimerKeeper'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Chameleon'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AutoPluginManager'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SpclPerm'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'MonsterRegen'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'DisableMonsLoot'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'HardPlayerDrop'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ReFishTask'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'Sandstorm'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'RandomBroadcast'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BedSet'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ConvertWorld'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AutoStoreItems'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ZHIPlayerManager'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SpawnInfra'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CaiRewardChest'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CNPCShop'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SessionSentinel'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'TeleportRequest'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'CaiCustomEmojiCommand'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'BetterWhitelist'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'AutoReset'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'SmartRegions'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ProxyProtocolSocket'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'UnseenInventory'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

/* Unmerged change from project 'ChestRestore'
Before:
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
After:
        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
*/

        return asm.GetManifestResourceInfo(moFilePath) == null ? new Catalog() : new Catalog(asm.GetManifestResourceStream(moFilePath));
    }

    public static string GetString(FormattableStringAdapter text)
    {
        return C.GetString(text);
    }

    public static string GetString(FormattableStringAdapter text, params object[] args)
    {
        return C.GetString(text, args);
    }

    public static string GetString(FormattableString text)
    {
        return C.GetString(text);
    }

    public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n)
    {
        return C.GetPluralString(text, pluralText, n);
    }

    public static string GetPluralString(FormattableString text, FormattableString pluralText, long n)
    {
        return C.GetPluralString(text, pluralText, n);
    }

    public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n,
        params object[] args)
    {
        return C.GetPluralString(text, pluralText, n, args);
    }

    public static string GetParticularString(string context, FormattableStringAdapter text)
    {
        return C.GetParticularString(context, text);
    }

    public static string GetParticularString(string context, FormattableString text)
    {
        return C.GetParticularString(context, text);
    }

    public static string GetParticularString(string context, FormattableStringAdapter text, params object[] args)
    {
        return C.GetParticularString(context, text, args);
    }

    public static string GetParticularPluralString(string context, FormattableStringAdapter text,
        FormattableStringAdapter pluralText, long n)
    {
        return C.GetParticularPluralString(context, text, pluralText, n);
    }

    public static string GetParticularPluralString(string context, FormattableString text,
        FormattableString pluralText, long n)
    {
        return C.GetParticularPluralString(context, text, pluralText, n);
    }

    public static string GetParticularPluralString(string context, FormattableStringAdapter text,
        FormattableStringAdapter pluralText, long n, params object[] args)
    {
        return C.GetParticularPluralString(context, text, pluralText, n, args);
    }
}