using Unity.Services.Lobbies.Models;

namespace Network
{
    public class LobbyEvents
    {
        public delegate void LobbyUpdated(Lobby lobby);

        public static LobbyUpdated OnLobbyUpdated;
    }
}
