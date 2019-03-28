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
    // Start is called before the first frame update
    void Start()
    {
        for (int i = -BUILD_WIDTH; i < BUILD_WIDTH; i++)
        {
            for (int j = -BUILD_WIDTH; j < BUILD_WIDTH; j++)
            {
                // TODO: Refactor to a method
                var newChunk = Instantiate(PrefabManager.GetPrefab(PrefabType.Chunk), 
                    new Vector3(i * Chunk.CHUNK_SIZE, 0, j * Chunk.CHUNK_SIZE), 
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
            //Debug.Log("Resetting chunk");
            Chunk newChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(player.transform.position));

            if (newChunk == null)
            {
                Debug.LogWarning($"NewChunk null, player position = {player.transform.position} chunkKey = {Chunk.GetKey(player.transform.position)}");
                return;
            }

            float xDiff = newChunk.StartX - currentChunk.StartX;
            float zDiff = newChunk.StartZ - currentChunk.StartZ;

            HashSet<string> newChunks = new HashSet<string>();
            for (float xOffset = -Chunk.CHUNK_SIZE * BUILD_WIDTH; xOffset < Chunk.CHUNK_SIZE * BUILD_WIDTH; xOffset += Chunk.CHUNK_SIZE)
            {
                for (float zOffset = -Chunk.CHUNK_SIZE * BUILD_WIDTH; zOffset < Chunk.CHUNK_SIZE * BUILD_WIDTH; zOffset += Chunk.CHUNK_SIZE)
                {
                    Vector3 pos = new Vector3(newChunk.StartX + xOffset, 0, newChunk.StartZ + zOffset);
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
                        ChunkManager.RemoveChunk(c);
                        Destroy(c.gameObject);
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
            var pos = chunkCreationSet.Dequeue();
            var newChunkZ = Instantiate(PrefabManager.GetPrefab(PrefabType.Chunk), pos, new Quaternion());
            var chunk = (Chunk)newChunkZ.GetComponent(typeof(Chunk));
            ChunkManager.AddChunk(chunk);
        }
    }
}
