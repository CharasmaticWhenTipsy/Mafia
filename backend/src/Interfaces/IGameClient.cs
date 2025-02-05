using System.Threading.Tasks;
using Mafia.Models;

namespace Mafia.Interfaces
{
    public interface IGameClient
    {
        Task Run(Lobby Lobby);
    }
}