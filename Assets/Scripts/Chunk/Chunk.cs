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

    #region Local Variables

    private static Dictionary<Tuple<int, int>, Cube> allCubePositions = new Dictionary<Tuple<int, int>, Cube>();
    private Dictionary<Tuple<int, int>, Cube> localCubePosition = new Dictionary<Tuple<int, int>, Cube>();

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

        //BuildColumns(); // Commenting out until I figure out how to do this better
        enabled = false;
    }

    private void OnDestroy()
    {
        //Debug.Log($"Destroying chunk key = {GetKey()}");
        // We need to make a showllow copy because the localCube is deleting from the localCubePositions
        foreach (Cube localCube in localCubePosition.Values.ToArray())
        {
            CubeManager.AddGameObjectToPool(localCube.gameObject);
            localCube.DeleteFromChunk();
        }

        localCubePosition.Clear();
    }

    #endregion Unity Overrides

    #region Cube Creation methods

    private void CreateGameObject(BaseBiome biome, Vector3 position)
    {
        GameObject prefab = biome.GetObjectForPosition(position);
        GameObject objectToSpawn;
        if (CubeManager.HasGameObjectOfPrefab(prefab))
        {
            //Debug.Log("Using CubeManager");
            objectToSpawn = CubeManager.GetGameObjectOfFromPool(prefab);
            objectToSpawn.transform.SetPositionAndRotation(position, new Quaternion());
        }
        else
        {
            //Debug.Log("Creating new GameObject");
            objectToSpawn = Instantiate(prefab, position, new Quaternion());
        }

        Cube cube = objectToSpawn.GetComponent<Cube>();
        cube.Spawn(this);
        AddPositionToDictionaries(position, cube);
    }

    // Commenting out until I figure out to handle this
    //private void BuildColumns()
    //{
    //    foreach (KeyValuePair<Tuple<int, int>, Cube> localCube in localCubePosition)
    //    {
    //        for (int i = -1; i <= 1; i += 2)
    //        {
    //            for (int j = -1; j <= 1; j += 2)
    //            {
    //                Tuple<int, int> adjacentPos = new Tuple<int, int>(localCube.Key.Item1 + i, localCube.Key.Item2 + j);
    //                if (allCubePositions.ContainsKey(adjacentPos))
    //                {
    //                    int adjacentHeight = allCubePositions[adjacentPos].Y;
    //                    int diffHeight = localCube.Value.Y - adjacentHeight;

    //                    if (diffHeight > 1)
    //                    {
    //                        for (int k = 1; k <= diffHeight; ++k)
    //                        {
    //                            Vector3 newPos = new Vector3(localCube.Key.Item1, localCube.Value.Y - k, localCube.Key.Item2);
    //                            //Cube newCube = new Cube(Instantiate(PrefabManager.GetPrefab(PrefabType.Grass), newPos, new Quaternion()), this);
    //                            // This is where the bug lies. We need to find another way to build out the columns
    //                            //AddPositionToDictionaries(newPos, newCube);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    private void AddPositionToDictionaries(Vector3 position, Cube cube)
    {
        Tuple<int, int> key = new Tuple<int, int>((int)position.x, (int)position.z);
        localCubePosition.Add(key, cube);
        allCubePositions.Add(key, cube);
    }

    #endregion Cube Creation methods

    #region Cube Deletion Methods

    public void DeleteCube(Cube cube)
    {
        Tuple<int, int> cubeCoordinates = cube.GetCoordinates();
        if (localCubePosition.ContainsKey(cubeCoordinates))
        {
            localCubePosition.Remove(cubeCoordinates);
            allCubePositions.Remove(cubeCoordinates);
        }
        else
        {
            Debug.LogError($"Could not find cube position {cubeCoordinates.Item1} {cubeCoordinates.Item2}");
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
