using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    #region Static variables
    private static Dictionary<Tuple<int, int>, int> allColumnHeights = new Dictionary<Tuple<int, int>, int>();
    private static Dictionary<Vector3, Cube> allVectors = new Dictionary<Vector3, Cube>();
    #endregion Static Variables

    #region Local Variables
    private HashSet<Tuple<int, int>> localCubePosition = new HashSet<Tuple<int, int>>();
    private HashSet<Cube> localCubes = new HashSet<Cube>();
    private BaseBiome currentBiome;

    public int StartX { get; private set; }
    public int StartZ { get; private set; }

    public bool IsChanged { get; private set; } = false;
    public bool IsVisible { get; private set; } = true;

    #endregion Local Variables

    #region Unity Overrides
    void Start()
    {
        StartX = (int)gameObject.transform.position.x;
        StartZ = (int)gameObject.transform.position.z;
        currentBiome = BiomeManager.GetBiome(StartX, StartZ);
        for (int i = 0; i < WorldConstants.CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < WorldConstants.CHUNK_SIZE; ++j)
            {
                Vector3 pos = currentBiome.GetHeightForPosition(StartX, StartZ, i, j);
                CreateGameObject(pos);
            }
        }

        BuildColumns();
        BuildWater();
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
                DeactivateAndRemove(localCube);
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
    private void CreateGameObject(Vector3 position)
    {
        GameObject prefab = currentBiome.GetObjectForPosition(position);
        CreateGameObject(prefab, position);
        if (position.y > -1)
        {
            AddAirCube(position);
        }
    }

    private void CreateGameObject(GameObject prefab, Vector3 position)
    {
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
        if (cube != null)
        {
            if (allVectors.ContainsKey(position) && allVectors[position] is AirCube)
            {
                Destroy(allVectors[position]);
                allVectors.Remove(position);
            } 
            cube.Spawn(this);
            AddPositionToDictionaries(position, cube);
        }
        else
        {
            Cube[] cubes = objectToSpawn.GetComponentsInChildren<Cube>(true);
            foreach (Cube treeCube in cubes)
            {
                if (allVectors.ContainsKey(treeCube.GetPosition()))
                {
                    treeCube.DeactivateCube();
                }
                else
                {
                    treeCube.Spawn(this);
                    AddPositionToDictionaries(treeCube.GetPosition(), treeCube);
                }
            }
        }
    }

    /// <summary>
    /// Adds the aircube 1 spot above the current position
    /// </summary>
    /// <param name="position"></param>
    private void AddAirCube(Vector3 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y + 1, position.z);
        if (!allVectors.ContainsKey(newPosition))
        {
            CreateGameObject(PrefabManager.GetPrefab(PrefabType.Air), newPosition);
        }
    }

    /// <summary>
    /// Builds out the columns from (StartX - 1 , StartZ - 1) to (StartX + WorldConstants.CHUNK_SIZE, StartZ + WorldConstants.CHUNK_SIZE)
    /// What this means is that we will recompute columns on the edge of other Chunks surrounding this chunk.
    /// This needs to happen in case we run into a situation where we have a column that is shorter in this chunk
    /// but ther chunk's columns have already been computed chunks.
    /// 
    /// This took me forever to get the math right so I can understand that figuring out what is going on is hard.
    /// Sorry future me if you're reading this.
    /// </summary>
    private void BuildColumns()
    {
        for (int x = (int)StartX - 1; x <= StartX + WorldConstants.CHUNK_SIZE; ++x)
        {
            for (int z = (int)StartZ - 1; z <= StartZ +WorldConstants.CHUNK_SIZE; ++z)
            {
                Tuple<int, int> position = new Tuple<int, int>(x, z);
                if (allColumnHeights.ContainsKey(position))
                {
                    if (allVectors.TryGetValue(new Vector3(x, allColumnHeights[position], z), out Cube cube) && !(cube is TreeCube))
                    {
                        BuildColumnForPosition(position);
                    }
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
            for (int i = 1; i <= minimumHeight; i++)
            {
                Vector3 newPosition = new Vector3(position.Item1, currentHeight - i, position.Item2);
                if (!allVectors.ContainsKey(newPosition))
                {
                    CreateGameObject(currentBiome.GetColumnCube(), newPosition);
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

    private void BuildWater()
    {
        foreach (Tuple<int, int> cubeLocation in localCubePosition)
        {
            int height = allColumnHeights[cubeLocation];
            if (height < -1)
            {
                for (int i = -1; i > height; --i)
                {
                    Vector3 newPos = new Vector3(cubeLocation.Item1, i, cubeLocation.Item2);
                    CreateGameObject(PrefabManager.GetPrefab(PrefabType.Water), newPos);
                }
            }
        }
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
        allVectors.Add(position, cube);
    }

    /// <summary>
    /// Adds the cube
    /// </summary>
    /// <param name="newPosition"></param>
    private void AddAdjacentCube(Vector3 newPosition, bool shouldAddAir)
    {
        if (!allVectors.ContainsKey(newPosition))
        {
            CreateGameObject(currentBiome.GetAdjacentCube(), newPosition);
        }
        if (shouldAddAir)
        {
            AddAirCube(newPosition);
        }
    }

    public void CreateNewCube(Vector3 newPos, GameObject prefab)
    {
        IsChanged = true;
        CreateGameObject(prefab, newPos);
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

    private void RemoveCubeFromAllVectors(Cube cube)
    {
        Vector3 cubePos = new Vector3(cube.X, cube.Y, cube.Z);
        if (allVectors.ContainsKey(cubePos))
        {
            allVectors.Remove(cubePos);
        }
    }

    /// <summary>
    /// Deactivates and removes the cube from the chunk
    /// </summary>
    /// <param name="cube"></param>
    private void DeactivateAndRemove(Cube cube)
    {
        RemoveCubeFromChunk(cube);
        RemoveCubeFromAllVectors(cube);
        CubeManager.AddGameObjectToPool(cube.gameObject);
        cube.DeactivateCube();
    }

    /// <summary>
    /// Mines the cube. Deactivates and removes the cube
    /// </summary>
    /// <param name="cube"></param>
    public void MineCube(Cube cube)
    {
        IsChanged = true;
        // Need to refactor how we remove cubes
        RemoveCubeFromChunk(cube);
        RemoveCubeFromAllVectors(cube);
        PlaceSurroundingCubes(cube);
        CubeManager.AddGameObjectToPool(cube.gameObject);
        cube.DeactivateCube();
    }

    /// <summary>
    /// Places the surrounding cubes around the cube that was mined
    /// </summary>
    /// <param name="cube"></param>
    private void PlaceSurroundingCubes(Cube cube)
    {
        // Right
        Vector3 position = new Vector3(cube.X + 1, cube.Y, cube.Z);
        AddAdjacentCube(position, false);

        // Up
        position = new Vector3(cube.X, cube.Y, cube.Z + 1);
        AddAdjacentCube(position, false);

        // Left
        position = new Vector3(cube.X - 1, cube.Y, cube.Z);
        AddAdjacentCube(position, false);

        // Down
        position = new Vector3(cube.X, cube.Y, cube.Z - 1);
        AddAdjacentCube(position, false);

        // Below (Incidently adds air cube right above current cube
        position = new Vector3(cube.X, cube.Y - 1, cube.Z);
        AddAdjacentCube(position, true);
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
        float x = position.x / WorldConstants.CHUNK_SIZE;
        float z = position.z / WorldConstants.CHUNK_SIZE;

        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }

    public static Vector3 GetVector3FromKey(string key)
    {
        string[] coordinates = key.Split(' ');
        int x = int.Parse(coordinates[0]);
        int z = int.Parse(coordinates[1]);

        return new Vector3(x, 0, z);
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
