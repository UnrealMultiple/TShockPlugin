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
        if (string.IsNullOrEmpty(cultureInfo.Name))
        {
            cultureInfo = new CultureInfo("en-US");
        }

        var asm = Assembly.GetExecutingAssembly();
        var moFilePath = $"i18n.{cultureInfo.Name}.mo";

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