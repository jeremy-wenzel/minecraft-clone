using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;

    private const int INIT_SIZE = 10;
    private Chunk currentChunk = null;
    private const int BUILD_WIDTH = 5;
    private HashSet<Chunk> visibleChunks = new HashSet<Chunk>();
    private Queue<Vector3> chunkCreationSet = new Queue<Vector3>();
    private HashSet<string> chunkDestructionSet = new HashSet<string>();

    void Start()
    {
        for (int i = -BUILD_WIDTH; i < BUILD_WIDTH; i++)
        {
            for (int j = -BUILD_WIDTH; j < BUILD_WIDTH; j++)
            {
                // TODO: Refactor to a method
                var newChunk = Instantiate(PrefabManager.GetPrefab(PrefabType.Chunk), 
                    new Vector3(i * WorldConstants.CHUNK_SIZE, 0, j * WorldConstants.CHUNK_SIZE), 
                    new Quaternion());
                var chunk = (Chunk)newChunk.GetComponent(typeof(Chunk));
                ChunkManager.AddChunk(chunk);
                visibleChunks.Add(chunk);

            }
        }

        // TODO: Hacky, fix up
        currentChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(new Vector3(0, 0, 0)));
    }

    void Update()
    {
        if (!currentChunk.IsPositionInChunk(player.transform.position))
        {
            Chunk newChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(player.transform.position));
            if (newChunk == null)
            {
                Debug.LogWarning($"NewChunk null, player position = {player.transform.position} chunkKey = {Chunk.GetKey(player.transform.position)}");
                return;
            }

            HashSet<string> newChunks = GetNewChunksForCreationFromCurrentChunk(newChunk);
            DestroyOrHideUnseenChunks(newChunks);

            visibleChunks.Clear();
            AddAlreadyVisibileChunks(newChunks);

            currentChunk = newChunk;
        }

        InitializeNewChunk();

        DestroyChunkInQueue();
    }


    /// <summary>
    /// Gets the new chunks to be created using the current position.
    /// </summary>
    /// <param name="newChunk"></param>
    /// <returns></returns>
    private HashSet<string> GetNewChunksForCreationFromCurrentChunk(Chunk newChunk)
    {
        HashSet<string> newChunks = new HashSet<string>();
        for (float xOffset = -WorldConstants.CHUNK_SIZE * BUILD_WIDTH; xOffset < WorldConstants.CHUNK_SIZE * BUILD_WIDTH; xOffset += WorldConstants.CHUNK_SIZE)
        {
            for (float zOffset = -WorldConstants.CHUNK_SIZE * BUILD_WIDTH; zOffset < WorldConstants.CHUNK_SIZE * BUILD_WIDTH; zOffset += WorldConstants.CHUNK_SIZE)
            {
                Vector3 newChunkPosition = new Vector3(newChunk.StartX + xOffset, 0, newChunk.StartZ + zOffset);
                string chunkKey = Chunk.GetKey(newChunkPosition);
                if (chunkDestructionSet.Contains(chunkKey))
                {
                    chunkDestructionSet.Remove(chunkKey);
                }

                if (!ChunkManager.ChunkExists(chunkKey))
                {
                    AddChunkToCreationSetIfNecessary(newChunkPosition);
                }
                else
                {
                    MakeExistingChunkVisible(chunkKey, newChunks);
                }
            }
        }

        return newChunks;
    }

    private void AddChunkToCreationSetIfNecessary(Vector3 newChunkPosition)
    {
        // We want to check if the chunk exists in the creation set
        bool chunkExistsInCreationSet = false;
        foreach (Vector3 existingChunkPosition in chunkCreationSet)
        {
            if (existingChunkPosition == newChunkPosition)
            {
                chunkExistsInCreationSet = true;
                break;
            }
        }

        if (!chunkExistsInCreationSet)
        {
            // Add the chunk to the creation queue if it doesn't exist already
            chunkCreationSet.Enqueue(newChunkPosition);
        }
    }

    private void MakeExistingChunkVisible(string chunkKey, HashSet<string> newChunks)
    {
        Chunk existingChunk = ChunkManager.GetChunkWithKey(chunkKey);
        existingChunk.SetVisibility(true);
        newChunks.Add(existingChunk.GetKey());
    }

    private void DestroyOrHideUnseenChunks(HashSet<string> newChunks)
    {
        foreach (Chunk c in visibleChunks)
        {
            if (!newChunks.Contains(c.GetKey()))
            {
                if (c.IsChanged)
                {
                    c.SetVisibility(false);
                }
                else
                {
                    chunkDestructionSet.Add(c.GetKey());
                }
            }
        }
    }

    private void AddAlreadyVisibileChunks(HashSet<string> newChunks)
    {
        foreach (string s in newChunks)
        {
            if (ChunkManager.ChunkExists(s))
            {
                visibleChunks.Add(ChunkManager.GetChunkWithKey(s));
            }
        }
    }

    private void InitializeNewChunk()
    {
        if (chunkCreationSet.Count > 0)
        {
            // The downside to this is we will only build one per frame
            var pos = chunkCreationSet.Dequeue();
            var newChunkZ = Instantiate(PrefabManager.GetPrefab(PrefabType.Chunk), pos, new Quaternion());
            var chunk = (Chunk)newChunkZ.GetComponent(typeof(Chunk));
            ChunkManager.AddChunk(chunk);
            visibleChunks.Add(chunk);
        }
    }

    private void DestroyChunkInQueue()
    {
        if (chunkDestructionSet.Count > 0)
        {
            string key = chunkDestructionSet.First();
            Chunk c = ChunkManager.GetChunkWithKey(key);
            ChunkManager.RemoveChunk(c);
            Destroy(c.gameObject);
            chunkDestructionSet.Remove(key);
        }
    }
}
