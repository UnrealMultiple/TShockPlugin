namespace MusicPlayer.Music;

internal class NoteFileParser
{
    public static List<List<float>> Read(string path, out int tempo)
    {
        var list = new List<List<float>>();
        var num = Tempo;
        using (var streamReader = new StreamReader(path))
        {
            var headerIsReaded = false;
            string? text;
            while ((text = streamReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(text) || text.StartsWith('#'))
                {
                    continue;
                }
                if (headerIsReaded)
                {
                    var noteValues = new List<float>();
                    foreach (var noteNameText in text.Split(',', StringSplitOptions.None))
                    {
                        if (NoteName.TryGetNoteByName(noteNameText, out var noteValue))
                        {
                            noteValues.Add(noteValue);
                        }
                        else
                        {
                            Console.Error.WriteLine("错误的读取: {0}", noteNameText);
                        }
                    }
                    list.Add(noteValues);
                }
                else
                {
                    if (int.TryParse(text, out num))
                    {
                        headerIsReaded = true;
                    }
                }
            }
        }
        tempo = num;
        return list;
    }

    public static int Tempo = 250;
}