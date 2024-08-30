namespace EssentialsPlus.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Performs a case insensitive "Contains"
    /// </summary>
    /// <returns>
    /// true if the substring findText was found in the string, or
    /// false otherwise, or there was an error.
    /// </returns>
    public static bool ContainsInsensitive(this string str, string findText)
    {
        return string.IsNullOrEmpty(str) || string.IsNullOrEmpty(findText)
            ? false
            : str.IndexOf(findText, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}