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

        public string PlayerName { get; set; }
        public string ConnectionId { get; set; }
    }
}
