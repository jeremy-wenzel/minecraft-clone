using System.Collections.Generic;
using System;

public class BiomeManager : Singleton<BiomeManager>
{
    private static int X_RANGE = 100;
    private static int Z_RANGE = 100;

    private static Dictionary<Tuple<float, float>, BaseBiome> biomeDictionary = new Dictionary<Tuple<float, float>, BaseBiome>();
    private static Random rand = new Random();

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
            int value = rand.Next() % 2;
            BaseBiome newBiome;
            switch (value)
            {
                case 0:
                    newBiome = new GrassBiome();
                    break;
                case 1:
                    newBiome = new SnowBiome();
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