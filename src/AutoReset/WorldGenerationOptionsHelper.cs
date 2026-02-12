using System.Text;
using Terraria.WorldBuilding;

namespace AutoReset;

public class WorldGenerationOptionsHelper
{
    public static long WorldGenerationOptionsToFlag()
    {
        var generationOptions = WorldGenerationOptions.Options.ToList();
        foreach (var option in generationOptions)
        {
            option.Load();
        }
        long result = 0;

        for (var i = 0; i < generationOptions.Count && i < 64; i++)
        {
            if (generationOptions[i].Enabled)
            {
                result |= 1L << i;
            }
        }

        return result;
    }

    public static void FlagToWorldGenerationOptions(long flag)
    {
        for (var i = 0; i < WorldGenerationOptions.Options.Count() && i < 64; i++)
        {
            WorldGenerationOptions._options[i].Enabled = (flag & (1L << i)) != 0;
            WorldGenerationOptions._options[i].Load();
        }
    }

    public static string BuildWorldGenerationOptions()
    {
        var generationOptions = WorldGenerationOptions.Options.ToList();
        foreach (var option in generationOptions)
        {
            option.Load();
        }
        const string enabledMarker = "[x]";
        const string disabledMarker = "[ ]";

        return string.Join("\n", generationOptions.Select((option, index) => 
            $"<{index + 1,-2}> {(option.Enabled ? enabledMarker : disabledMarker)} {option.Title.Value}"
        ));
    }
    
    public static string BuildWorldGenerationEnableOptions()
    {
        var generationOptions = WorldGenerationOptions.Options.ToList();
        foreach (var option in generationOptions)
        {
            option.Load();
        }
        const string enabledMarker = "[x]";

        return string.Join("\n", generationOptions.Where(x=>x.Enabled).Select((option, index) => 
            $"<{index + 1,-2}> {enabledMarker} {option.Title.Value}"
        ));
    }
    
    public static void SetWorldGenerationOptions(int index)
    {
        var generationOptions = WorldGenerationOptions.Options.ToList();
        foreach (var option in generationOptions)
        {
            option.Load();
        }
        generationOptions[index - 1].Enabled = !generationOptions[index - 1].Enabled;
    }
}