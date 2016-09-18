namespace GameLibrary.Objects
{
    public class AchievementButton : MenuObject
    {
        public int AchievementId { get; protected set; }

        public AchievementButton(int achievementId)
        {
            AchievementId = achievementId;
        }

    }
}
