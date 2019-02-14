using Assets.Scripts;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;

    private const int INIT_SIZE = 10;
    private Chunk currentChunk = null;
    private const int BUILD_WIDTH = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = -INIT_SIZE; i <= INIT_SIZE; i++)
        {
            for (int j = -INIT_SIZE; j <= INIT_SIZE; j++)
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
            Chunk newChunk = ChunkManager.GetChunkWithKey(Chunk.GetKey(player.transform.position));

            if (newChunk == null)
            {
                Debug.Log($"NewChunk null, player position = {player.transform.position}");
                return;
            }

            float xDiff = newChunk.StartX - currentChunk.StartX;
            float zDiff = newChunk.StartZ - currentChunk.StartZ;


            if (Mathf.Abs(xDiff) > 0)
            {
                float xOffset = (xDiff > 0 ? 1 : -1) * Chunk.CHUNK_SIZE * BUILD_WIDTH;
                for (float offset = -Chunk.CHUNK_SIZE * BUILD_WIDTH; offset < Chunk.CHUNK_SIZE * BUILD_WIDTH; offset += Chunk.CHUNK_SIZE)
                {
                    Vector3 pos = new Vector3(currentChunk.StartX + xOffset, 0, currentChunk.StartZ + offset);
                    if (!ChunkManager.ChunkExists(Chunk.GetKey(pos)))
                    {
                        var newChunkZ = Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), pos, new Quaternion());
                        ChunkManager.AddChunk((Chunk)newChunkZ.GetComponent(typeof(Chunk)));
                    }
                }
            }
            else if (Mathf.Abs(zDiff) > 0)
            {
                float zOffset = (zDiff > 0 ? 1 : -1) * Chunk.CHUNK_SIZE * BUILD_WIDTH;
                for (float offset = -Chunk.CHUNK_SIZE * BUILD_WIDTH; offset < Chunk.CHUNK_SIZE * BUILD_WIDTH; offset += Chunk.CHUNK_SIZE)
                {
                    Vector3 pos = new Vector3(currentChunk.StartX + offset, 0, currentChunk.StartZ + zOffset);
                    if (!ChunkManager.ChunkExists(Chunk.GetKey(pos)))
                    {
                        var newChunkZ = Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), pos, new Quaternion());
                        ChunkManager.AddChunk((Chunk)newChunkZ.GetComponent(typeof(Chunk)));
                    }
                }
            }

            currentChunk = newChunk;
        }
    }
}
