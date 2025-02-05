using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Markup;
using Mafia.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Mafia.Services
{
    public class GameService
    {
        private static Random _random = new Random();
        public void SetupGame(Lobby lobby)
        {

            // TODO: Add more validation.
            if (lobby.State != GameState.InProgress)
            {
                throw new InvalidOperationException("Wrong game state");
                // make call to clients from here probably to send game back to lobby?
            }

            lobby.Players = lobby.Players.OrderBy(x => _random.Next()).ToList(); // Randomise players.

            // Use index (now random) to assign team and roles based on config rules.
            int playerIndex = 0;
            foreach (var player in lobby.Players)
            {

                if (playerIndex < lobby.Config.NumMafiaPlayers)
                {
                    // Assign Mafia Role in order.
                    player.Team = Team.Mafia;
                    switch (playerIndex)
                    {
                        case 0: player.MafiaRole = MafiaRole.Godfather; break;
                        case 1: player.MafiaRole = MafiaRole.Mafioso; break;
                        case > 1: player.MafiaRole = GetRandomEnumValue<MafiaRole>(3); break;
                    }
                }
                else
                {
                    // Assign a town role until it fills a valid role based on config values
                    player.Team = Team.Town;
                    TownRole TownRole;
                    do { TownRole = GetRandomEnumValue<TownRole>(1); }
                    while (lobby.Config.AvailableTownRoles.Contains(TownRole) && lobby.Players.Select(p => p.TownRole == TownRole).Count() >= lobby.Config.MaximumDuplicateTownRoles); // TODO: this is inefficient - fix
                }
                playerIndex++;
            }
            return;
        }
        private static T GetRandomEnumValue<T>(int minValue)
        {
            Array values = Enum.GetValues(typeof(T));
            int randomIndex = _random.Next(minValue, values.Length);
            return (T)values.GetValue(randomIndex);
        }
    }
}