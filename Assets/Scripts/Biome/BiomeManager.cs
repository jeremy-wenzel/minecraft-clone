using System.Collections.Generic;
using System;

public class BiomeManager : Singleton<BiomeManager>
{
    private const int X_RANGE = 100;
    private const int Z_RANGE = 100;

    private static Dictionary<Tuple<float, float>, BaseBiome> biomeDictionary = new Dictionary<Tuple<float, float>, BaseBiome>();
    private static Random rand = new Random();

    /// <summary>
    /// Gets the biome from the local positions. Will cache previously created biomes so they
    /// are not constantly regenerated
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static BaseBiome GetBiome(float x, float z)
    {
        float cx = (int)x / X_RANGE;
        float cz = (int)z / Z_RANGE;

        Tuple<float, float> position = new Tuple<float, float>(cx, cz);

        if (biomeDictionary.ContainsKey(position))
        {
            return biomeDictionary[position];
        }
        else
        {
            // Just make a random one
            int value = rand.Next() % 4;
            BaseBiome newBiome;
            switch (value)
            {
                case 0:
                    newBiome = new GrassBiome();
                    break;
                case 1:
                    newBiome = new SnowBiome();
                    break;
                case 2:
                    newBiome = new StoneBiome();
                    break;
                case 3:
                    newBiome = new SandBiome();
                    break;
                default:
                    newBiome = new GrassBiome();
                    break;
            }

            biomeDictionary[position] = newBiome;
            return newBiome;
        }
    }
}