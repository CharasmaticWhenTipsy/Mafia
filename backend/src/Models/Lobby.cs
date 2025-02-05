using System.Collections.Generic;

namespace Mafia.Models
{
    public class Lobby
    {
        public string LobbyId { get; set; }
        public GameState State { get; set; } = GameState.Waiting;
        public List<Player> Players { get; set; } = new List<Player>();
    }
}
