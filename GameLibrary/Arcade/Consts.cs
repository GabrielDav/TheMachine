
namespace GameLibrary.Arcade
{
    public static class Consts
    {
        public const int LevelSwitchInterval = 2250;

        public const int MinimalDistanceBetweenCircles = 50;
        public const int HalfDistanceBetweenCircles = 15;

        public const int SawsSpeed = 85;
        public const int SawSpeedDivider = 400;
        public const int SawMaxSpeed = 105;

        public const int MapWidth = 1000;
        public const int MinimumDistanceFromWall = 60;
        public const int DistanceFromSpikes = 40;

        public const int WallWidth = 100;
        public const int WallHeight = 240;

        public const int SpikeHeight = 150;

        public const int SpikeShooterHeight = 75;

        public const int MinimumBallR = 50;
        public const int MaximumBallR = 150;
        public const int MaximumSecondaryBallR = 120; 

        public const int Left = 100;
        public const int Right = 900;

        public const int LeftSpike = 80;
        public const int RightSpike = 920;

        public const int MinInkDotsCount = 4;
        public const int MaxInkDotsCount = 6;

        public const int InkDotR = 5;
        public const int InkDotDistance = 20;

        public const int VercticalDistanceFromWallEdge = 5;
        public const int SpikeShooterHalfSize = 35;

        public const int DrawnDistance = 300;

        public const int ChanceOfSidePoweUp = 40;

        public static LevelConst[] LevelConst = new LevelConst[9];

         static Consts()
         {
             for (int i = 0; i < LevelConst.Length; i++)
             {
                 LevelConst[i] = new LevelConst
                     {
                         ChanceOfSpikeShooter = 25 + 5*i,/*i*15+10,*/
                         ChancesOfTrapsOnWall = 25 + 10*i,
                         BulletInterval = 3500 /*- 200*i */
                     };
             }
             
         }
    }

    public class LevelConst
    {
        public int ChanceOfSpikeShooter;
        public int ChancesOfTrapsOnWall;
        public int BulletInterval;
    }
}
