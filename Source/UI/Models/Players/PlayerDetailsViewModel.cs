﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BusinessLogic.Models.Players;
using UI.Models.PlayedGame;

namespace UI.Models.Players
{
    public class PlayerDetailsViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        [Display(Name = "Player Registered")]
        public bool PlayerRegistered { get; set; }
        public bool Active { get; set; }
        public List<GameResultViewModel> PlayerGameResultDetails { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int TotalPoints { get; set; }
        public float AveragePointsPerGame { get; set; }
        public float AveragePlayersPerGame { get; set; }
        public float AveragePointsPerPlayer { get; set; }
        public bool HasNemesis { get; set; }
        public int NemesisPlayerId { get; set; }
        public string NemesisName { get; set; }
        public float LossPercentageVersusPlayer { get; set; }
        public int NumberOfGamesLostVersusNemesis { get; set; }
        public bool UserCanEdit { get; set; }
        public List<MinionViewModel> Minions { get; set; }
        public List<PlayerGameSummary> PlayerGameSummaries { get; set; }
        public List<ChampionViewModel> ChampionedGames { get; set; } 

        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public string MinionBraggingTweetUrl { get; set; }
    }
}
