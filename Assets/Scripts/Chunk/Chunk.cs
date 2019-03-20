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
    private static Dictionary<Tuple<int, int>, int> allColumnHeights = new Dictionary<Tuple<int, int>, int>();
    private static HashSet<Vector3> allVectors = new HashSet<Vector3>();
    #endregion Static Variables

    #region Local Variables
    private HashSet<Tuple<int, int>> localCubePosition = new HashSet<Tuple<int, int>>();
    private HashSet<Cube> localCubes = new HashSet<Cube>();

    public float StartX { get; private set; }
    public float StartZ { get; private set; }

    private static readonly Perlin perlin = new Perlin();

    public bool IsChanged { get; private set; } = false;
    public bool IsVisible { get; private set; } = true;

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
        foreach (Tuple<int, int> position in localCubePosition)
        {
            allColumnHeights.Remove(position);
        }

        // We need to make a shallow copy because the localCube is deleting from the localCubes
        foreach (Cube localCube in localCubes.ToArray())
        {
            if (localCube != null)
            {
                localCube.DeactivateCube();
                RemoveCubeFromChunk(localCube);
            }
        }
    }

    #endregion Unity Overrides

    #region Cube Creation methods

    /// <summary>
    /// Creates the Cube
    /// </summary>
    /// <param name="biome"></param>
    /// <param name="position"></param>
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
        AddAirCube(position);
    }

    /// <summary>
    /// Adds the aircube 1 spot above the current position
    /// </summary>
    /// <param name="position"></param>
    private void AddAirCube(Vector3 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y + 1, position.z);
        AirCube cube = Instantiate(PrefabManager.GetPrefab(PrefabType.Air), newPosition, new Quaternion()).GetComponent<AirCube>();
        cube.Spawn(this);
        localCubes.Add(cube);
        allVectors.Add(newPosition);
    }

    /// <summary>
    /// Builds out the columns from (StartX - 1 , StartZ - 1) to (StartX + CHUNK_SIZE, StartZ + CHUNK_SIZE)
    /// What this means is that we will recompute columns on the edge of other Chunks surrounding this chunk.
    /// This needs to happen in case we run into a situation where we have a column that is shorter in this chunk
    /// but ther chunk's columns have already been computed chunks.
    /// 
    /// This took me forever to get the math right so I can understand that figuring out what is going on is hard.
    /// Sorry future me if you're reading this.
    /// </summary>
    private void BuildColumns()
    {
        for (int x = (int)StartX - 1; x <= StartX + CHUNK_SIZE; ++x)
        {
            for (int z = (int)StartZ - 1; z <= StartZ + CHUNK_SIZE; ++z)
            {
                Tuple<int, int> position = new Tuple<int, int>(x, z);
                if (allColumnHeights.ContainsKey(position))
                {
                    BuildColumnForPosition(position);
                }
            }
        }   
    }

    /// <summary>
    /// Builds the column of cubes needed to make sure there are no gaps
    /// </summary>
    /// <param name="position"></param>
    private void BuildColumnForPosition(Tuple<int, int> position)
    {
        int minimumHeight = ComputeMinHeight(position);
        int currentHeight = allColumnHeights[position];
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
    /// <summary>
    /// Computes the minimum height needed to be built for a column
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private int ComputeMinHeight(Tuple<int, int> position)
    {
        int minimumHeight = -9999999;
        for (int i = 0; i < xArr.Length; i++) // This is a competitve programming trick to easily access adjecent memebers
        {
            Tuple<int, int> adjacentPosition = new Tuple<int, int>(position.Item1 + xArr[i], position.Item2 + zArr[i]);
            if (allColumnHeights.ContainsKey(adjacentPosition))
            {
                int distance = Math.Abs(allColumnHeights[position] - allColumnHeights[adjacentPosition]);
                if (distance > minimumHeight)
                {
                    minimumHeight = distance;
                }
            }
        }

        return minimumHeight;
    }

    /// <summary>
    /// Adds position and cube to appropriate directories
    /// </summary>
    /// <param name="position"></param>
    /// <param name="cube"></param>
    private void AddPositionToDictionaries(Vector3 position, Cube cube)
    {
        Tuple<int, int> key = new Tuple<int, int>((int)position.x, (int)position.z);
        if (!allColumnHeights.ContainsKey(key))
        {
            localCubePosition.Add(key);
            allColumnHeights.Add(key, (int)position.y);
        }

        localCubes.Add(cube);
        allVectors.Add(position);
    }

    /// <summary>
    /// Adds the cube
    /// </summary>
    /// <param name="newPosition"></param>
    private void AddCube(Vector3 newPosition, bool shouldAddAir)
    {
        if (!allVectors.Contains(newPosition))
        {
            Cube newCube = Instantiate(PrefabManager.GetPrefab(PrefabType.Grass), newPosition, new Quaternion()).GetComponent<Cube>();
            AddPositionToDictionaries(newPosition, newCube);
            newCube.Spawn(this);
            if (shouldAddAir)
            {
                AddAirCube(newPosition);
            }
        }
    }

    #endregion Cube Creation methods

    #region Cube Deletion Methods

    /// <summary>
    /// Removes the cube from the chunk
    /// </summary>
    /// <param name="cube"></param>
    private void RemoveCubeFromChunk(Cube cube)
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

    /// <summary>
    /// Deactivates and removes the cube from the chunk
    /// </summary>
    /// <param name="cube"></param>
    private void DeactivateAndRemove(Cube cube)
    {
        CubeManager.AddGameObjectToPool(cube.gameObject);
        cube.DeactivateCube();
        RemoveCubeFromChunk(cube);
    }

    /// <summary>
    /// Mines the cube. Deactivates and removes the cube
    /// </summary>
    /// <param name="cube"></param>
    public void MineCube(Cube cube)
    {
        IsChanged = true;
        DeactivateAndRemove(cube);
        PlaceSurroundingCubes(cube);
    }

    /// <summary>
    /// Places the surrounding cubes around the cube that was mined
    /// </summary>
    /// <param name="cube"></param>
    private void PlaceSurroundingCubes(Cube cube)
    {
        // Right
        Vector3 position = new Vector3(cube.X + 1, cube.Y, cube.Z);
        AddCube(position, false);

        // Up
        position = new Vector3(cube.X, cube.Y, cube.Z + 1);
        AddCube(position, false);

        // Left
        position = new Vector3(cube.X - 1, cube.Y, cube.Z);
        AddCube(position, false);

        // Down
        position = new Vector3(cube.X, cube.Y, cube.Z - 1);
        AddCube(position, false);

        // Below (Incidently adds air cube right above current cube
        position = new Vector3(cube.X, cube.Y - 1, cube.Z);
        AddCube(position, true);
    }
    
    #endregion Cube Deletion Methods

    #region Position Related Methods

    /// <summary>
    /// Is the position currently in the chunk? Determines if the x and z are both
    /// inside of the chunk. We don't care about y.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsPositionInChunk(Vector3 pos)
    {
        return GetKey() == GetKey(pos);
    }

    /// <summary>
    /// Gets the position key for the current chunk
    /// </summary>
    /// <returns></returns>
    public string GetKey()
    {
        return GetKey(gameObject.transform.position);
    }

    /// <summary>
    /// Takes in a Vector3 to generate a key that represents the Chunks position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static string GetKey(Vector3 position)
    {
        float x = position.x / CHUNK_SIZE;
        float z = position.z / CHUNK_SIZE;

        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }

    #endregion Position Related Methods

    public void SetVisibility(bool makeVisible)
    {
        if ((makeVisible && !IsVisible) || (!makeVisible && IsVisible))
        {
            // We only want to set the state if we need to change.
            localCubes.ForEach(c => c.SetVisibility(makeVisible));
        }

        IsVisible = makeVisible;
    }
}
