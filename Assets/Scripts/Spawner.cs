using Assets.Scripts;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;

    private const int INIT_SIZE = 3;
    private Chunk currentChunk = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < INIT_SIZE; i++)
        {
            for (int j = 0; j < INIT_SIZE; j++)
            {
                // TODO: Refactor to a method
                var newChunk = Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), 
                    new Vector3(i * Chunk.CHUNK_SIZE, 0, j * Chunk.CHUNK_SIZE), 
                    new Quaternion());
                ChunkManager.AddChunk((Chunk)newChunk.GetComponent(typeof(Chunk)));
            }
        }

        // TODO: Hacky, fix up
        currentChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(new Vector3(0, 0, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: THere is a bug in here where we are not setting the current chunk correctly which means
        // that we do this iteration
        if (!currentChunk.IsPositionInChunk(player.transform.position))
        {
            currentChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(player.transform.position));
            
            for(float xOffset = -Chunk.CHUNK_SIZE; xOffset <= Chunk.CHUNK_SIZE; xOffset += Chunk.CHUNK_SIZE)
            {
                for (float zOffset = -Chunk.CHUNK_SIZE; zOffset <= Chunk.CHUNK_SIZE; zOffset += Chunk.CHUNK_SIZE)
                {
                    Vector3 pos = new Vector3(currentChunk.startX + xOffset, 0, currentChunk.startZ + zOffset);
                    if (!ChunkManager.ChunkExists(Chunk.GetKey(pos)))
                    {
                        var newChunk = Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), pos, new Quaternion());
                        ChunkManager.AddChunk((Chunk)newChunk.GetComponent(typeof(Chunk)));
                    }
                }
            }
        }
    }
}
