using MusicPlayer.Music;
using System.Diagnostics.CodeAnalysis;
using TShockAPI;

namespace MusicPlayer
{
    internal class SongPlayer
    {
        private bool listening;
        [MemberNotNullWhen(true, nameof(CurrentSong))]
        public bool Listening { get => listening; }
        public TSPlayer Player { get; set; }
        public PlaySongInfo? CurrentSong { get; private set; }

        public SongPlayer(TSPlayer ply)
        {
            Player = ply;
            listening = false;
        }

        public bool StartSong(PlaySongInfo? playSongInfo = null)
        {
            listening = true;
            if (playSongInfo is null)
            {
                if (CurrentSong is null)
                {
                    return false;
                }
                CurrentSong.Play();
                return true;
            }
            CurrentSong = playSongInfo;
            playSongInfo.Play();
            return true;
        }

        public bool EndSong()
        {
            listening = false;
            if (CurrentSong is null)
            {
                return false;
            }
            CurrentSong.Stop();
            MusicPlayer.ListeningCheck();
            return true;
        }
    }
}