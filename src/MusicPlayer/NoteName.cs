namespace MusicPlayer;

public static class NoteName
{
    public static float GetNoteByName(string name)
    {
        if (NoteDictionary.ContainsKey(name))
        {
            return NoteDictionary[name];
        }
        else if (name == "0") // 音符名称为0时返回一个负数表示停顿
        {
            return -3f; // 例如，返回-3f表示停顿
        }
        throw new ArgumentException(GetString($"给与的名称不是一个音符. 数值: [{name}]"), nameof(name));
    }
    public static bool TryGetNoteByName(string name, out float noteValue)
    {
        if (NoteDictionary.TryGetValue(name, out noteValue))
        {
            return true;
        }
        else if (name == "0") // 音符名称为0时返回一个负数表示停顿
        {
            noteValue = -3;
            return true; // 例如，返回-3f表示停顿
        }
        return false;
    }

    public static readonly Dictionary<string, float> NoteDictionary = new Dictionary<string, float>
    {
        {"C4",-1f},
        {"C#4",-0.916667f},
        {"D4",-0.833333f},
        {"D#4",-0.75f},
        {"E4",-0.6666667f},
        {"F4",-0.5833333f},
        {"F#4",-0.5f},
        {"G4",-0.41666666f},
        {"G#4",-0.33333334f},
        {"A4",-0.25f},
        {"A#4",-0.16666667f},
        {"B4",-0.083333336f},
        {"C5",0f},
        {"C#5",0.083333336f},
        {"D5",0.16666667f},
        {"D#5",0.25f},
        {"E5",0.33333334f},
        {"F5",0.41666666f},
        {"F#5",0.5f},
        {"G5",0.5833333f},
        {"G#5",0.6666667f},
        {"A5",0.75f},
        {"A#5",0.833333f},
        {"B5",0.916667f},
        {"C6",1f}
    };
}