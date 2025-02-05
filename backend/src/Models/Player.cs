using System.Reflection;

namespace Mafia.Models
{
    // Example Player class to track players
    public class Player
    {
        public Player(string playerName, string connectionId)
        {
            PlayerName = playerName;
            ConnectionId = connectionId;
        }

        public string PlayerName { get; private set; }
        public string ConnectionId { get; private set; }
        public bool IsHost { get; set; } = false;
        public Team Team { get; set; } = Team.None;
        public Role Role { get; set; } = Role.None;
        public bool IsAlive { get; set; } = true;
    }
}
