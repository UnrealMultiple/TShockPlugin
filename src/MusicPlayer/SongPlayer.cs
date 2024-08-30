using MusicPlayer.Music;
using System.Diagnostics.CodeAnalysis;
using TShockAPI;

namespace MusicPlayer;

internal class SongPlayer
{
    [MemberNotNullWhen(true, nameof(CurrentSong))]
    public bool Listening { get; private set; }
    public TSPlayer Player { get; set; }
    public PlaySongInfo? CurrentSong { get; private set; }

    public SongPlayer(TSPlayer ply)
    {
        this.Player = ply;
        this.Listening = false;
    }

    public bool StartSong(PlaySongInfo? playSongInfo = null)
    {
        this.Listening = true;
        if (playSongInfo is null)
        {
            if (this.CurrentSong is null)
            {
                return false;
            }
            this.CurrentSong.Play();
            return true;
        }
        this.CurrentSong = playSongInfo;
        playSongInfo.Play();
        return true;
    }

    public bool EndSong()
    {
        this.Listening = false;
        if (this.CurrentSong is null)
        {
            return false;
        }
        this.CurrentSong.Stop();
        MusicPlayer.ListeningCheck();
        return true;
    }
}