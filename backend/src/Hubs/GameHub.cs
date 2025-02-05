using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Mafia.Models;
using Mafia.Interfaces;

namespace Mafia.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public void Run(Lobby lobby)
        {
            // inital calc and setup of roles ??
            // use rand and config here.

        }
    }
}