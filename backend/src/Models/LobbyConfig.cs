using System;
using System.Collections.Generic;

namespace Mafia.Models
{
    public class LobbyConfig
    {
        public int NumMafiaPlayers { get; set; } = 1;
        public List<TownRole> AvailableTownRoles { get; set; } = [TownRole.Investigator, TownRole.Doctor, TownRole.Vigilante];
        public int MaximumDuplicateTownRoles { get; set; } = 1;
    }
}