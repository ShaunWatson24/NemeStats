﻿using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionSaver : IGameDefinitionSaver
    {
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE 
            = "gameDefinition.Name cannot be null or whitespace.";

        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
        }

        public GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            ValidateGameDefinitionIsNotNull(gameDefinition);
            ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinition.Name);
            bool isNewGameDefinition = !gameDefinition.AlreadyInDatabase();
            if (isNewGameDefinition)
            {
                new Task(() => eventTracker.TrackGameDefinitionCreation(currentUser, gameDefinition.Name)).Start();
            }
            GameDefinition newGameDefinition = dataContext.Save<GameDefinition>(gameDefinition, currentUser);

            return newGameDefinition;
        }

        private static void ValidateGameDefinitionIsNotNull(GameDefinition gameDefinition)
        {
            if (gameDefinition == null)
            {
                throw new ArgumentNullException("gameDefinition");
            }
        }

        private static void ValidateGameDefinitionNameIsNotNullOrWhitespace(string gameDefinitionName)
        {
            if (string.IsNullOrWhiteSpace(gameDefinitionName))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE);
            }
        }
    }
}
