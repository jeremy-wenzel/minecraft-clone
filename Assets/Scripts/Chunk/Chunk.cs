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

    private static Dictionary<Tuple<int, int>, int> allCubePositions = new Dictionary<Tuple<int, int>, int>();
    private Dictionary<Tuple<int, int>, int> localCubePosition = new Dictionary<Tuple<int, int>, int>();
    private HashSet<GameObject> cubes = new HashSet<GameObject>();
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
                localCubePosition.Add(new Tuple<int, int>((int)StartX + i, (int)StartZ + j), (int) totalY);
                allCubePositions.Add(new Tuple<int, int>((int)StartX + i, (int)StartZ + j), (int)totalY);                
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
                GameObject gameO = Instantiate(PrefabManager.GetPrefab(prefabType));
                gameO.transform.SetPositionAndRotation(pos, new Quaternion());
                cubes.Add(gameO);
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
        foreach (GameObject o in cubes)
        {
            Destroy(o);
        }
        foreach (KeyValuePair<Tuple<int, int>, int> position in localCubePosition)
        {
            allCubePositions.Remove(position.Key);
        }

        cubes.Clear();
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

        //Debug.Log($"Chunk key = {x} {z}");
        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }

    private void BuildColumns()
    {
        foreach(KeyValuePair<Tuple<int, int>, int> cubePosition in localCubePosition)
        {
            for (int i = -1; i <= 1; i+=2)
            {
                for (int j = -1; j <= 1; j+=2)
                {
                    Tuple<int, int> adjacentPos = new Tuple<int, int>(cubePosition.Key.Item1 + i, cubePosition.Key.Item2 + j);
                    if (allCubePositions.ContainsKey(adjacentPos))
                    {
                        int adjacentHeight = allCubePositions[adjacentPos];
                        int diffHeight = cubePosition.Value - adjacentHeight;

                        if (diffHeight > 1)
                        {
                            for (int k = 1; k <= diffHeight; ++k)
                            {
                                Vector3 newPos = new Vector3(cubePosition.Key.Item1, cubePosition.Value - k, cubePosition.Key.Item2);
                                Instantiate(PrefabManager.GetPrefab(PrefabType.GRASS)).transform.SetPositionAndRotation(newPos, new Quaternion());
                            }
                        }
                    }
                }
            }
        }
    }


}
