using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mafia.Models;
using System;
using System.Linq;
using Mafia.Interfaces;
using Mafia.Services;

namespace Mafia.Hubs
{
    public class LobbyHub : Hub<ILobbyClient>
    {
        private static Dictionary<string, Lobby> _lobbies = new Dictionary<string, Lobby>();
        private readonly GameService gameService;
        private static Random _random = new Random();

        // TODO
        // add functionality to remove players during lobby.
        // Remove Lobby on close, or meeting other conditions, when user leaves => check if last member of lobby if so close, inactive lobby for ten mins = close ?

        // Create a new lobby
        public async Task CreateLobby(string userName)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lobbyId;

            do
            {
                lobbyId = new string(Enumerable.Range(0, 5)
                    .Select(_ => chars[_random.Next(chars.Length)])
                    .ToArray()
                );

            } while (_lobbies.ContainsKey(lobbyId));  // Ensure the generated ID is unique

            var lobby = new Lobby
            {
                LobbyId = lobbyId,
                State = GameState.Lobby
            };

            _lobbies[lobbyId] = lobby;

            await Clients.Caller.LobbyCreated(lobbyId);

            await JoinLobby(lobbyId, userName); // Also join user to lobby.
        }

        // Method to join a lobby
        public async Task JoinLobby(string lobbyId, string playerName)
        {
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.Error("Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];


            if (IsPlayerInLobbyByConnectionId(lobby, Context.ConnectionId))
            {
                // alternatively update name to new name and carry on
                await Clients.Caller.Error("Player already in the lobby.");
                return;
            }

            // Player name exists?
            if (lobby.Players.Any(p => p.PlayerName == playerName))
            {
                await Clients.Caller.Error("Player name is already taken.");
            }

            if (lobby.State != GameState.Lobby)
            {
                await Clients.Caller.Error("Cannot join. The game has already started.");
                // return message and prompt join as spectator.
                return;
            }

            Player newPlayer = new Player(playerName, Context.ConnectionId);

            lobby.Players.Add(newPlayer);

            if (lobby.Players.Count() == 1)  // If Joining Lobby as the only player, become host.
            {
                lobby.Players.Find(p => p.ConnectionId == newPlayer.ConnectionId).IsHost = true;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            await Clients.Group(lobbyId).PlayerConnected(playerName);
        }

        // Method to start the game
        public async Task StartGame(string lobbyId)
        {
            // auth host
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.Error("Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];

            if (lobby.State != GameState.Lobby)
            {
                await Clients.Caller.Error("Game cannot be started. It is either already started or finished.");
                return;
            }

            lobby.State = GameState.InProgress;
            // also calculate other lobby attributes here => pass an expected object with counts of role types etc.
            // default config can be hard coded on frontend or passed by backend.

            // day time max timer, with skip option available. => basically vote to go to night time.
            // night time lasts one minute or 30 seconds or something.

            await Clients.Group(lobbyId).GameStarted(lobbyId); // ??

            gameService.SetupGame(lobby);


        }

        // Handle player disconnect
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var playerConnectionId = Context.ConnectionId;

            foreach (var lobby in _lobbies.Values)
            {
                if (lobby.Players.Select(p => p.ConnectionId).Contains(playerConnectionId)) // remove them from every lobby, they should only belong to a single lobby ?
                {
                    lobby.Players.Remove(lobby.Players.Find(p => p.ConnectionId == playerConnectionId));

                    // pretty sure we want to post the name here?
                    // alternatively we return a key value for this.
                    await Clients.Group(lobby.LobbyId).PlayerLeft(playerConnectionId);
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Handle player reconnect
        public async Task ReconnectGame(string lobbyId, string playerName)
        {
            if (!_lobbies.ContainsKey(lobbyId))
            {
                await Clients.Caller.Error("Lobby does not exist.");
                return;
            }

            var lobby = _lobbies[lobbyId];

            if (IsPlayerInLobbyByConnectionId(lobby, playerName))
            {
                // lobby.ReconnectedPlayers.Add(playerName); // ?? surely we add this to normal players list or something
                await Clients.Caller.Reconnected(playerName);
                await Clients.Group(lobbyId).PlayerReconnected(playerName);
            }
        }

        private bool IsPlayerInLobbyByConnectionId(Lobby lobby, string playerName)
        {
            return lobby.Players.Select(p => p.PlayerName).Contains(playerName);
        }
    }
}
