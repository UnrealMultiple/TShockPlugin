using Terraria;

namespace MusicPlayer.Music;

internal class PlaySongInfo
{
    public List<List<float>> Notes;
    public int Tempo;

    public bool PlayCompleted { get; private set; }
    public VirtualPerformer Performer { get; private set; }
    private int delta;
    private DateTime lastUpdate;
    private int noteIndex;
    public PlaySongInfo(List<List<float>> notes, int tempo, VirtualPerformer performer)
    {
        this.Notes = notes;
        this.Tempo = tempo;
        this.Performer = performer;
    }
    public void Update(int index)
    {
        if (this.PlayCompleted)
        {
            return;
        }
        if (this.noteIndex == this.Notes.Count)
        {
            this.PlayCompleted = true;
            var songPlayer = MusicPlayer.SongPlayers[index];
            songPlayer?.EndSong();
            return;
        }
        this.delta += (int) (DateTime.Now - this.lastUpdate).TotalMilliseconds;

        if (this.delta > this.Tempo)
        {
            foreach (var noteValue in this.Notes[this.noteIndex++])
            {
                if (noteValue >= -1f && noteValue <= 1f)
                {
                    //PlayNote(index, noteValue);
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
        this.lastUpdate = DateTime.Now;
    }
    public void Stop()
    {
        this.PlayCompleted = true;
    }
    public static void PlayNote(int index, float note)
    {
        NetMessage.SendData((int) PacketTypes.PlayHarp, index, -1, null, index, note, 0f, 0f, 0);
    }
}