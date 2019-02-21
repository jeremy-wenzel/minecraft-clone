using Assets.Scripts;
using System.Collections.Generic;
using System;

public class BiomeManager : Singleton<BiomeManager>
{
    private static int X_RANGE = 100;
    private static int Z_RANGE = 100;

    private static Dictionary<Tuple<float, float>, BiomeTypeEnum> BiomeDictionary = new Dictionary<Tuple<float, float>, BiomeTypeEnum>();

    public static BiomeTypeEnum GetBiome(float x, float z)
    {
        float cx = (int)x / X_RANGE;
        float cz = (int)z / Z_RANGE;

        Tuple<float, float> position = new Tuple<float, float>(cx, cz);

        if (BiomeDictionary.ContainsKey(position))
        {
            return BiomeDictionary[position];
        }
        else
        {
            // Just make a random one
            Random rand = new Random();
            int value = rand.Next() % 2;
            BiomeTypeEnum newBiome;
            switch (value)
            {
                case 0:
                    newBiome = BiomeTypeEnum.Grass;
                    break;
                case 1:
                    newBiome = BiomeTypeEnum.Snow;
                    break;
                default:
                    newBiome = BiomeTypeEnum.Grass;
                    break;
            }

            BiomeDictionary[position] = newBiome;
            return newBiome;
        }
    }
}