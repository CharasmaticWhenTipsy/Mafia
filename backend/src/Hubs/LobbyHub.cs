using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mafia.Models;
using System;
using System.Linq;

namespace Mafia.Hubs
{
    public class LobbyHub : Hub
    {
        // Dictionary to track all the lobbies
        private static Dictionary<string, Lobby> _lobbies = new Dictionary<string, Lobby>();
        private static Random _random = new Random();

        // Create a new lobby
        public async Task CreateLobby(string userName)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";  // Possible characters for the lobby ID
            string lobbyId;

            // Generate a unique lobby ID
            do
            {
                lobbyId = new string(Enumerable.Range(0, 5)  // Length of the generated ID (5 characters)
                    .Select(_ => chars[_random.Next(chars.Length)]) // Select random character from chars
                    .ToArray());

            } while (_lobbies.ContainsKey(lobbyId));  // Ensure the generated ID is unique

            var lobby = new Lobby
            {
                LobbyId = lobbyId,
                State = GameState.Waiting
            };

            _lobbies[lobbyId] = lobby; // TODO Remove Lobby on close, or meeting other conditions, when user leaves => check if last member of lobby if so close, inactive lobby for ten mins = close ?

            // Notify the caller that the lobby was created
            await Clients.Caller.SendAsync("LobbyCreated", lobbyId);

            await JoinLobby(lobbyId, userName); // Also join user to lobby.
        }

        // Method to join a specific lobby
        //We probably call this directly after creating the lobby, or we make the client join the lobby automatically.
        public async Task JoinLobby(string lobbyId, string playerName)
        {
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.SendAsync("Error", "Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];

            // some form of spectator mode ?
            if (lobby.State != GameState.Waiting)
            {
                await Clients.Caller.SendAsync("Error", "Cannot join. The game has already started.");
                return;
            }

            // private function for if player is in lobby => pass it lobby and playerName?

            if (lobby.Players.Select(p => p.PlayerName).Contains(playerName)) // Select names as list or something here
            {
                await Clients.Caller.SendAsync("Error", "Player already in the lobby.");
                return;
            }

            Player newPlayer = new Player(
                playerName, Context.ConnectionId
            );

            lobby.Players.Add(newPlayer); // gen new Player then add.
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);

            await Clients.Group(lobbyId).SendAsync("PlayerJoined", playerName);
        }

        // Method to start the game in a specific lobby
        public async Task StartGame(string lobbyId)
        {
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.SendAsync("Error", "Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];

            if (lobby.State != GameState.Waiting)
            {
                await Clients.Caller.SendAsync("Error", "Game cannot be started. It is either already started or finished.");
                return;
            }

            lobby.State = GameState.InProgress;
            // also calculate other lobby attributes here => pass an expected object with counts of role types etc.
            // default config can be hard coded on frontend or passed by backend.

            // day time max timer, with skip option available. => basically vote to go to night time.
            // night time lasts one minute or 30 seconds or something.

            await Clients.Group(lobbyId).SendAsync("GameStarted");
        }

        // Handle player disconnection
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var playerConnectionId = Context.ConnectionId;

            foreach (var lobby in _lobbies.Values)
            {
                if (lobby.Players.Select(p => p.ConnectionId).Contains(playerConnectionId)) // remove them form every lobby, they should only belong to a single lobby ?
                {
                    lobby.Players.Remove(lobby.Players.Find(p => p.ConnectionId == playerConnectionId));

                    await Clients.Group(lobby.LobbyId).SendAsync("PlayerLeft", playerConnectionId);
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Handle player reconnection
        public async Task ReconnectGame(string lobbyId, string playerName)
        {
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.SendAsync("Error", "Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];

            // TODO: Remove this, reconnecting is valid at all times because of the loop nature of the game.
            // Lobby leader can remove players perhaps?

            if (lobby.State == GameState.Finished)
            {
                await Clients.Caller.SendAsync("Error", "The game is over. You cannot reconnect.");
                return;
            }

            if (IsPlayerInLobbyByPlayerName(lobby, playerName) && !lobby.ReconnectedPlayers.Contains(playerName))
            {
                lobby.ReconnectedPlayers.Add(playerName); // ?? surely we add this to normal players list or something
                await Clients.Caller.SendAsync("Reconnected", playerName);
                await Clients.Group(lobbyId).SendAsync("PlayerReconnected", playerName);
            }
        }

        private bool IsPlayerInLobbyByPlayerName(Lobby lobby, string playerName)
        {
            return lobby.Players.Select(p => p.PlayerName).Contains(playerName);
        }
    }
}
