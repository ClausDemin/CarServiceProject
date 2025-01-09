namespace CarServiceProject.Utils
{
    public static class UserUtils
    {
        private static Random s_Random = new Random();

        public static float GetNextSingle(float minValue = 0, float maxValue = 1)
        {
            return s_Random.NextSingle() * (maxValue - minValue) + minValue;
        }

        public static int GetNextInt(int minValue = int.MinValue, int maxValue = int.MaxValue) 
        { 
            return s_Random.Next(minValue, maxValue);
        }
    }
}
