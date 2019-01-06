using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;

    private const int INIT_SIZE = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < INIT_SIZE; i++)
        {
            for (int j = 0; j < INIT_SIZE; j++)
            {
                Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), 
                    new Vector3(i * Chunk.CHUNK_SIZE, 0, j * Chunk.CHUNK_SIZE), 
                    new Quaternion());
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
