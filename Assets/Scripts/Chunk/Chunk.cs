using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 16;
    private const float _scaleFactor = 20f;
    private const float _worldScale = 5f;
    private const float _steepnessScale = 200f;
    private const int _offset = 1000;

    private const int SNOW_MAX_Y = 8;
    private static readonly Perlin perlin = new Perlin();

    private static Dictionary<Tuple<int, int>, Cube> allCubePositions = new Dictionary<Tuple<int, int>, Cube>();
    private Dictionary<Tuple<int, int>, Cube> localCubePosition = new Dictionary<Tuple<int, int>, Cube>();

    public float StartX { get; private set; }

    public float StartZ { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        StartX = gameObject.transform.position.x;
        StartZ = gameObject.transform.position.z;
        BiomeTypeEnum biome = BiomeManager.GetBiome(StartX, StartZ);
        for (int i = 0; i < CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < CHUNK_SIZE; ++j)
            {
                float newX = (StartX + i + _offset) / _scaleFactor;
                float newZ = (StartZ + j + _offset) / _scaleFactor;

                // this essentially allows us to generate the steepness. Dividing by _worldScale
                // allows us to have plains and montains because the steepness spans over a longer distance
                float steepnessY = perlin.DoPerlin(newX / _worldScale, newZ / _worldScale) * _steepnessScale;
                float totalY =  perlin.DoPerlin(newX, newZ) * steepnessY;
                Vector3 pos = new Vector3(StartX + i, (int)totalY, StartZ + j);
                               
                PrefabType prefabType;
                switch (biome)
                {
                    case BiomeTypeEnum.Grass:
                        prefabType = PrefabType.GRASS;
                        if (totalY > SNOW_MAX_Y)
                        {
                            prefabType = PrefabType.SNOW;
                        }
                        break;
                    case BiomeTypeEnum.Snow:
                        prefabType = PrefabType.SNOW;
                        break;
                    default:
                        prefabType = PrefabType.GRASS;
                        UnityEngine.Debug.Log($"Unknown BiomeType {biome}");
                        break;
                }
                Cube cube = new Cube(Instantiate(PrefabManager.GetPrefab(prefabType), pos, new Quaternion()));
                AddPositionToDictionaries(pos, cube);
            }
        }

        BuildColumns();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Debug.Log($"Destroying chunk key = {GetKey()}");
        foreach (KeyValuePair<Tuple<int, int>, Cube> localCube in localCubePosition)
        {
            Destroy(localCube.Value.gameObject);
            allCubePositions.Remove(localCube.Key);
        }

        localCubePosition.Clear();
    }

    public bool IsPositionInChunk(Vector3 pos)
    {
        return GetKey() == GetKey(pos);
    }

    public string GetKey()
    {
        return GetKey(gameObject.transform.position);
    }

    public static string GetKey(Vector3 position)
    {
        float x = position.x / CHUNK_SIZE;
        float z = position.z / CHUNK_SIZE;
        
        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }

    private void AddPositionToDictionaries(Vector3 position, Cube cube)
    {
        Tuple<int, int> key = new Tuple<int, int>((int)position.x, (int)position.z);
        localCubePosition.Add(key, cube);
        allCubePositions.Add(key, cube); 
    }

    private void BuildColumns()
    {
        foreach(KeyValuePair<Tuple<int, int>, Cube> localCube in localCubePosition)
        {
            for (int i = -1; i <= 1; i+=2)
            {
                for (int j = -1; j <= 1; j+=2)
                {
                    Tuple<int, int> adjacentPos = new Tuple<int, int>(localCube.Key.Item1 + i, localCube.Key.Item2 + j);
                    if (allCubePositions.ContainsKey(adjacentPos))
                    {
                        int adjacentHeight = allCubePositions[adjacentPos].VerticalPosition;
                        int diffHeight = localCube.Value.VerticalPosition - adjacentHeight;

                        if (diffHeight > 1)
                        {
                            for (int k = 1; k <= diffHeight; ++k)
                            {
                                Vector3 newPos = new Vector3(localCube.Key.Item1, localCube.Value.VerticalPosition - k, localCube.Key.Item2);
                                Cube newCube = new Cube(Instantiate(PrefabManager.GetPrefab(PrefabType.GRASS), newPos, new Quaternion()));
                                // This is where the bug lies. We need to find another way to build out the columns
                                AddPositionToDictionaries(newPos, newCube);
                            }
                        }
                    }
                }
            }
        }
    }


}
