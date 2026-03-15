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
    public Queue<PlaySongInfo> SongQueue = new();

    public SongPlayer(TSPlayer ply)
    {
        this.Player = ply;
        this.Listening = false;
    }

    public bool StartSong(PlaySongInfo? playSongInfo = null, bool fromQueue = false)
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
        
        if (!fromQueue)
        {
            this.SongQueue.Clear();
        }
        
        this.CurrentSong = playSongInfo;
        playSongInfo.Performer.Create(this.Player.Index);
        playSongInfo.Play();
        playSongInfo.OnCompleted += OnSongFinished;
        return true;
    }

    private void OnSongFinished(int index)
    {
        if (SongQueue.Count > 0)
        {
            PlayNext();
        }
        else
        {
            this.Listening = false;
            MusicPlayer.ListeningCheck();
        }
    }

    public void PlayNext()
    {
        if (SongQueue.Count > 0)
        {
            var nextSong = SongQueue.Dequeue();
            StartSong(nextSong, true);
            this.Player.SendInfoMessage(GetString("正在播放队列中的下一首: {0}", nextSong.SongName ?? "未知歌曲"));
        }
        else
        {
            this.Listening = false;
            MusicPlayer.ListeningCheck();
        }
    }

    public void EnqueueSong(PlaySongInfo songInfo)
    {
        SongQueue.Enqueue(songInfo);
    }

    public bool EndSong(bool manual = true)
    {
        this.Listening = false;
        if (this.CurrentSong is null)
        {
            return false;
        }
        
        this.CurrentSong.Stop();
        this.CurrentSong.OnCompleted -= OnSongFinished;
        
        if (manual)
        {
            this.SongQueue.Clear();
        }
        
        MusicPlayer.ListeningCheck();
        return true;
    }
}
