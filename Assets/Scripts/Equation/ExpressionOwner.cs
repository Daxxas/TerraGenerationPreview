public static class ExpressionOwner
{
    private static FastNoiseLite noise;

    public static void SetupNoise(int seed)
    {
        noise = new FastNoiseLite(seed);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.0075f);
        noise.SetFractalOctaves(4);
    }

    public static double Noise2(double x, double y)
    {
        return noise.GetNoise((float) x,(float) y);
    }

    // public static double If(double predicate, double a, double b)
    // {
    //     if (predicate > 0)
    //     {
    //         return a;
    //     }
    //     else
    //     {
    //         return b;
    //     }
    // }
}
