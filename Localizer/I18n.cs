using System.Globalization;
using System.Reflection;
using GetText;
using TShockAPI;

namespace Localizer;

// ReSharper disable once InconsistentNaming
internal static class I18n
{
    // ReSharper disable once InconsistentNaming
    private static readonly Catalog C = GetCatalog();

    private static Catalog GetCatalog()
    {
        var cultureInfo = (CultureInfo)typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationCultureInfo",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        var asm = Assembly.GetExecutingAssembly();
        var moFilePath = $"i18n.{cultureInfo.Name}.mo";
        if (asm.GetManifestResourceInfo(moFilePath) == null)
            return new Catalog();

        return new Catalog(asm.GetManifestResourceStream(moFilePath));
    }

    public static string GetString(FormattableStringAdapter text) =>
        C.GetString(text);

    public static string GetString(FormattableStringAdapter text, params object[] args) =>
        C.GetString(text, args);

    public static string GetString(FormattableString text) =>
        C.GetString(text);

    public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n) =>
        C.GetPluralString(text, pluralText, n);

    public static string GetPluralString(FormattableString text, FormattableString pluralText, long n) =>
        C.GetPluralString(text, pluralText, n);

    public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n,
        params object[] args) =>
        C.GetPluralString(text, pluralText, n, args);

    public static string GetParticularString(string context, FormattableStringAdapter text) =>
        C.GetParticularString(context, text);

    public static string GetParticularString(string context, FormattableString text) =>
        C.GetParticularString(context, text);

    public static string GetParticularString(string context, FormattableStringAdapter text, params object[] args) =>
        C.GetParticularString(context, text, args);

    public static string GetParticularPluralString(string context, FormattableStringAdapter text,
        FormattableStringAdapter pluralText, long n) =>
        C.GetParticularPluralString(context, text, pluralText, n);

    public static string GetParticularPluralString(string context, FormattableString text,
        FormattableString pluralText, long n) =>
        C.GetParticularPluralString(context, text, pluralText, n);

    public static string GetParticularPluralString(string context, FormattableStringAdapter text,
        FormattableStringAdapter pluralText, long n, params object[] args) =>
        C.GetParticularPluralString(context, text, pluralText, n, args);
}