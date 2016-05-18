using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class NemePointsAchievement : BaseAchievement
    {
        public NemePointsAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.NemePoints;

        public override AchievementGroup Group => AchievementGroup.NotApply;

        public override string Name => "NemePoints collector";

        public override string Description => "Play games to earn NemePoints to unlock this achievement.";

        public override string IconClass => "fa fa-eye"; //TODO: a custom SVG letter N

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 2500},
            {AchievementLevel.Silver, 25000},
            {AchievementLevel.Gold, 100000}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalPoints =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId)
                    .Select(pg => pg.TotalPoints)
                    .DefaultIfEmpty(0)
                    .Sum();

            
            result.PlayerProgress = totalPoints;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(totalPoints);
            return result;
        }
    }
}