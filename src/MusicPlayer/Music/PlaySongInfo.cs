using Terraria;

namespace MusicPlayer.Music;

internal class PlaySongInfo
{
    public List<List<float>> Notes;
    public int Tempo;
    public string? SongName;

    public bool PlayCompleted { get; private set; }
    public VirtualPerformer Performer { get; private set; }
    private int delta;
    private DateTime lastUpdate;
    private int noteIndex;
    private bool completedTriggered;
    
    public event Action<int>? OnCompleted;

    public PlaySongInfo(List<List<float>> notes, int tempo, VirtualPerformer performer, string? songName = null)
    {
        this.Notes = notes;
        this.Tempo = tempo;
        this.Performer = performer;
        this.SongName = songName;
    }

    public void Update(int index)
    {
        if (this.PlayCompleted)
        {
            return;
        }
        
        if (this.noteIndex == this.Notes.Count)
        {
            if (!this.completedTriggered)
            {
                this.completedTriggered = true;
                this.PlayCompleted = true;
                this.OnCompleted?.Invoke(index);
            }
            return;
        }
        
        this.delta += (int)(DateTime.Now - this.lastUpdate).TotalMilliseconds;

        if (this.delta > this.Tempo)
        {
            foreach (var noteValue in this.Notes[this.noteIndex++])
            {
                if (noteValue >= -1f && noteValue <= 1f)
                {
                    this.Performer.PlayNote(index, noteValue);
                }
            }
            this.delta -= this.Tempo;
        }
        this.lastUpdate = DateTime.Now;
    }

    public void Play()
    {
        this.PlayCompleted = false;
        this.noteIndex = 0;
        this.delta = 0;
        this.completedTriggered = false;
        this.lastUpdate = DateTime.Now;
    }

    public void Stop()
    {
        this.PlayCompleted = true;
    }

    public static void PlayNote(int index, float note)
    {
        NetMessage.SendData((int)PacketTypes.PlayHarp, index, -1, null, index, note, 0f, 0f, 0);
    }
}
