using Terraria;

namespace MusicPlayer.Music
{
    internal class PlaySongInfo
    {
        public List<List<float>> Notes;
        public int Tempo;
        private bool playCompleted;
        public bool PlayCompleted { get => playCompleted; }
        public VirtualPerformer Performer { get; private set; }
        private int delta;
        private DateTime lastUpdate;
        private int noteIndex;
        public PlaySongInfo(List<List<float>> notes, int tempo, VirtualPerformer performer)
        {
            Notes = notes;
            Tempo = tempo;
            Performer = performer;
        }
        public void Update(int index)
        {
            if (playCompleted)
            {
                return;
            }
            if (noteIndex == Notes.Count)
            {
                playCompleted = true;
                var songPlayer = MusicPlayer.SongPlayers[index];
                if (songPlayer is not null)
                {
                    songPlayer.EndSong();
                }
                return;
            }
            delta += (int)(DateTime.Now - lastUpdate).TotalMilliseconds;

            if (delta > Tempo)
            {
                foreach (var noteValue in Notes[noteIndex++])
                {
                    if (noteValue >= -1f && noteValue <= 1f)
                    {
                        //PlayNote(index, noteValue);
                        Performer.PlayNote(index, noteValue);
                    }
                }
                delta -= Tempo;
            }
            lastUpdate = DateTime.Now;
        }
        public void Play()
        {
            playCompleted = false;
            noteIndex = 0;
            delta = 0;
            lastUpdate = DateTime.Now;
        }
        public void Stop()
        {
            playCompleted = true;
        }
        public static void PlayNote(int index, float note)
        {
            NetMessage.SendData((int)PacketTypes.PlayHarp, index, -1, null, index, note, 0f, 0f, 0);
        }
    }
}