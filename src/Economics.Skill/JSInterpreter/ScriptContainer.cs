using System.Text.RegularExpressions;

namespace Economics.Skill.JSInterpreter;

public class ScriptContainer
{
    public static void PreprocessRequires(JsScript script)
    {
        if (script == null || string.IsNullOrEmpty(script.Script) || !PreprocessorDirectives.requiresRegex.IsMatch(script.Script))
        {
            return;
        }
        foreach (Match item in PreprocessorDirectives.requiresRegex.Matches(script.Script))
        {
            var array = item.Groups[2].Value.Split(',');
            var array2 = array;
            foreach (var text in array2)
            {
                var text2 = text.Trim().Replace("\"", "");
                if (string.IsNullOrEmpty(text2))
                {
                    return;
                }
                script.PackageRequirements.Add(text2);
                script.Script = script.Script.Replace(item.Value, $"/** #pragma require \"{text2}\" - DO NOT CHANGE THIS LINE **/\r\n");
            }
        }
    }
}