using System.Threading.Tasks;

namespace Mafia.Interfaces
{
    public interface ILobbyClient
    {
        Task LobbyCreated(string lobbyId);
        Task PlayerConnected(string playerName);
        Task PlayerLeft(string playerName);
        Task PlayerReconnected(string playerName);
        Task Reconnected(string playerName);
        Task Error(string error);
    }
}