using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.Services
{
    public class LobbyService
    {
        private static Random _random = new Random();
        public string GenerateLobbyId(List<string> lobbyIds)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string lobbyId;
            do
            {
                lobbyId = new string(Enumerable.Range(0, 5)
                    .Select(_ => chars[_random.Next(chars.Length)])
                    .ToArray()
                );

            } while (lobbyIds.Contains(lobbyId));  // Ensure the generated ID is unique

            return lobbyId;
        }
    }
}
