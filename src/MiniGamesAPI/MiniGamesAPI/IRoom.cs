using Microsoft.Xna.Framework;

namespace MiniGamesAPI;

public interface IRoom
{
    int GetPlayerCount();

    void Initialize();

    void Dispose();

    void Conclude();

    void Start();

    void Stop();

    void Restore();

    void ShowRoomMemberInfo();

    void ShowVictory();

    void Broadcast(string msg, Color color);
}