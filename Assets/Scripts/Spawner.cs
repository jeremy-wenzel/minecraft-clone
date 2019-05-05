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

    // Update is called once per frame
    void Update()
    {
        if (!currentChunk.IsPositionInChunk(player.transform.position))
        {
            chunkCreationSet.Clear();

            Chunk newChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(player.transform.position));
            if (newChunk == null)
            {
                Debug.LogWarning($"NewChunk null, player position = {player.transform.position} chunkKey = {Chunk.GetKey(player.transform.position)}");
                return;
            }

            HashSet<string> newChunks = new HashSet<string>();
            for (float xOffset = -WorldConstants.CHUNK_SIZE * BUILD_WIDTH; xOffset < WorldConstants.CHUNK_SIZE * BUILD_WIDTH; xOffset += WorldConstants.CHUNK_SIZE)
            {
                for (float zOffset = -WorldConstants.CHUNK_SIZE * BUILD_WIDTH; zOffset < WorldConstants.CHUNK_SIZE * BUILD_WIDTH; zOffset += WorldConstants.CHUNK_SIZE)
                {
                    Vector3 pos = new Vector3(newChunk.StartX + xOffset, 0, newChunk.StartZ + zOffset);
                    if (chunkDestructionSet.Contains(Chunk.GetKey(pos)))
                    {
                        chunkDestructionSet.Remove(Chunk.GetKey(pos));
                    }
                    if (!ChunkManager.ChunkExists(Chunk.GetKey(pos)))
                    {
                        chunkCreationSet.Enqueue(pos);
                    }
                    else
                    {
                        Chunk existingChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(pos));
                        existingChunk.SetVisibility(true);
                        newChunks.Add(existingChunk.GetKey());
                    }
                }
            }
            
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

            visibleChunks.Clear();
            foreach (string s in newChunks)
            {
                if (ChunkManager.ChunkExists(s))
                {
                    visibleChunks.Add(ChunkManager.GetChunkWithKey(s));
                }
            }
            currentChunk = newChunk;
        }

        if (chunkCreationSet.Count > 0)
        {
            // The downside to this is we will only build one per frame
            var pos = chunkCreationSet.Dequeue();
            var newChunkZ = Instantiate(PrefabManager.GetPrefab(PrefabType.Chunk), pos, new Quaternion());
            var chunk = (Chunk)newChunkZ.GetComponent(typeof(Chunk));
            ChunkManager.AddChunk(chunk);
            visibleChunks.Add(chunk);
        }

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
