using TheGoo;

namespace GameLibrary.Arcade
{
    public static class Random
    {
        public static bool RandomTrueFalse()
        {
            return GameGlobals.Random.Next(0, 2) == 1;
        }

        public static int RandomSign()
        {
            return RandomTrueFalse() ? -1 : 1;
        }

        public static bool RandomProc(int proc)
        {
            return (proc >= GameGlobals.Random.Next(0, 100));
        }
    }
}
