
namespace GameLibrary.Arcade
{
    public static class CircleBuilderConsts
    {
        public const int MinDistanceLeft = 160;

        public const int MinDistanceRight = 840;

        public const int MinimumBallR = 50;

        public const int MinimumDeathBallR = 50;

        public const int MaximumBallR = 100;

        public const int MinimalSpeed = 3;

        public const int MaximumSpeed = 4;

        public const int MinimalDistanceBetweenCircles = 30; //was 50

        public const int MaximumSecondaryBallR = 75;

        public const int InkMinDistance = 10;

        public const int InkMaxDistance = 20;

        //public const int ChancesThatPowerUpApears = 50;

        //public const int LevelWhenPowerUpApears = 2;

        // prie apskritimo greicio pridedamas aukstis dalintas ir konstantos, pasiekus 4000 auksti, apskritimu greitis padides maziausiai 1
        public const int MinCircleSpeedDivider = 4500; 

        public const int MaxCircleSpeedDivider = 3000;

        //public const int PowerUpWidth = 30;

        public const int MaxRToBeDeathBall = 75;

        public static CircleLevelConsts[] CircleLevelConst = new CircleLevelConsts[9];


         static CircleBuilderConsts()
         {
             for (int i = 0; i < CircleLevelConst.Length; i++)
             {
                 CircleLevelConst[i] = new CircleLevelConsts
                     {
                         MinimumVerticalDistance = 50 + (i*15), //was 75
                         MaximumVerticalDistance = 125 + (i*25), //was 150
                         ChanceMainBallIsDeathBall = i * 5,
                         MinimumDeathBallDelay = 6 - i/3,
                         MaximumDeathBallDelay = 9 - i/3,
                         ChanceSecondaryBallIsDeathBall = i * 10,
                         MaximumDistanceBetweenBalls = 300,
                         //RequiredDistanceForPowerUp = 230,
                         ChanceMiddleBallIsDeathBall = 8*i
                     };
             }
             
         }
    }
}

public class CircleLevelConsts
{
    public int MinimumVerticalDistance;
    public int MaximumVerticalDistance;
    public int ChanceMainBallIsDeathBall;
    public int MinimumDeathBallDelay;
    public int MaximumDeathBallDelay;
    public int ChanceSecondaryBallIsDeathBall;
    public int MaximumDistanceBetweenBalls;
    public int ChanceMiddleBallIsDeathBall;
    //public int RequiredDistanceForPowerUp;
}
