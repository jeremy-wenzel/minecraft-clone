using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    #region Constants
    public const int CHUNK_SIZE = 8;
    private const float scaleFactor = 20f;
    private const float worldScale = 5f;
    private const float steppnessScale = 200f;
    private const int offset = 1000;
    private const int SNOW_MAX_Y = 8;
    #endregion Constants

    #region Static variables
    private static Dictionary<Tuple<int, int>, int> allCubePositions = new Dictionary<Tuple<int, int>, int>();
    private static HashSet<Vector3> allVectors = new HashSet<Vector3>();
    #endregion Static Variables

    #region Local Variables
    private HashSet<Tuple<int, int>> localCubePosition = new HashSet<Tuple<int, int>>();
    private HashSet<Cube> localCubes = new HashSet<Cube>();

    public float StartX { get; private set; }
    public float StartZ { get; private set; }

    private static readonly Perlin perlin = new Perlin();

    #endregion Local Variables

    #region Unity Overrides
    void Start()
    {
        StartX = gameObject.transform.position.x;
        StartZ = gameObject.transform.position.z;
        BaseBiome biome = BiomeManager.GetBiome(StartX, StartZ);
        for (int i = 0; i < CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < CHUNK_SIZE; ++j)
            {
                float newX = (StartX + i + offset) / scaleFactor;
                float newZ = (StartZ + j + offset) / scaleFactor;

                // this essentially allows us to generate the steepness. Dividing by _worldScale
                // allows us to have plains and montains because the steepness spans over a longer distance
                float steepnessY = perlin.DoPerlin(newX / worldScale, newZ / worldScale) * steppnessScale;
                float totalY = perlin.DoPerlin(newX, newZ) * steepnessY;
                Vector3 pos = new Vector3(StartX + i, (int)totalY, StartZ + j);

                CreateGameObject(biome, pos);
            }
        }

        BuildColumns();
        enabled = false;
    }

    private void OnDestroy()
    {
        //Debug.Log($"Destroying chunk key = {GetKey()}");
        foreach (Tuple<int, int> position in localCubePosition)
        {
            allCubePositions.Remove(position);
        }

        // We need to make a shallow copy because the localCube is deleting from the localCubePositions
        foreach (Cube localCube in localCubes.ToArray())
        {
            if (localCube != null)
            {
                CubeManager.AddGameObjectToPool(localCube.gameObject);
                localCube.DeleteFromChunk();
            }
        }
    }

    #endregion Unity Overrides

    #region Cube Creation methods

    private void CreateGameObject(BaseBiome biome, Vector3 position)
    {
        GameObject prefab = biome.GetObjectForPosition(position);
        GameObject objectToSpawn;
        if (CubeManager.HasGameObjectOfPrefab(prefab))
        {
            objectToSpawn = CubeManager.GetGameObjectOfFromPool(prefab);
            objectToSpawn.transform.SetPositionAndRotation(position, new Quaternion());
        }
        else
        {
            objectToSpawn = Instantiate(prefab, position, new Quaternion());
        }

        Cube cube = objectToSpawn.GetComponent<Cube>();
        cube.Spawn(this);
        AddPositionToDictionaries(position, cube);
    }

    private void BuildColumns()
    {
        for (int x = (int)StartX - 1; x <= StartX + CHUNK_SIZE; ++x)
        {
            for (int z = (int)StartZ - 1; z <= StartZ + CHUNK_SIZE; ++z)
            {
                Tuple<int, int> position = new Tuple<int, int>(x, z);
                if (allCubePositions.ContainsKey(position))
                {
                    BuildColumnForPosition(position);
                }
            }
        }   
    }

    private void BuildColumnForPosition(Tuple<int, int> position)
    {
        int minimumHeight = ComputeMinHeight(position);
        int currentHeight = allCubePositions[position];
        if (minimumHeight > 0)
        {
            for (int i = 0; i < minimumHeight; i++)
            {
                Vector3 newPosition = new Vector3(position.Item1, currentHeight - i, position.Item2);
                if (!allVectors.Contains(newPosition))
                {
                    Cube newCube = Instantiate(PrefabManager.GetPrefab(PrefabType.Grass), newPosition, new Quaternion()).GetComponent<Cube>();
                    localCubes.Add(newCube);
                    newCube.Spawn(this);
                }
            }
        }
    }

    private static int[] xArr = new int[] { 0, 1, 0, -1 };
    private static int[] zArr = new int[] { 1, 0, -1, 0 };
    private int ComputeMinHeight(Tuple<int, int> position)
    {
        int minimumHeight = -9999999;
        for (int i = 0; i < xArr.Length; i++)
        {
            Tuple<int, int> adjacentPosition = new Tuple<int, int>(position.Item1 + xArr[i], position.Item2 + zArr[i]);
            if (allCubePositions.ContainsKey(adjacentPosition))
            {
                int distance = Math.Abs(allCubePositions[position] - allCubePositions[adjacentPosition]);
                if (distance > minimumHeight)
                {
                    minimumHeight = distance;
                }
            }
        }

        return minimumHeight;
    }

    private void AddPositionToDictionaries(Vector3 position, Cube cube)
    {
        Tuple<int, int> key = new Tuple<int, int>((int)position.x, (int)position.z);
        localCubePosition.Add(key);
        localCubes.Add(cube);
        allCubePositions.Add(key, (int)position.y);
        allVectors.Add(position);
    }

    #endregion Cube Creation methods

    #region Cube Deletion Methods

    public void DeleteCube(Cube cube)
    {
        
        if (localCubes.Contains(cube))
        {
            localCubes.Remove(cube);
        }
        else
        {
            Debug.LogError($"Could not find cube position");
        }
    }

    #endregion Cube Deletion Methods

    #region Position Related Methods

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

    #endregion Position Related Methods
}
