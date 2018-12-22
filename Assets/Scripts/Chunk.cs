using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private const int CHUNK_SIZE = 10;
    private float startX;
    private float startZ;
    public GameObject spawnObject;

    // Start is called before the first frame update
    void Start()
    {
        startX = gameObject.transform.position.x;
        startZ = gameObject.transform.position.z;
        for (int i = 0; i < CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < CHUNK_SIZE; ++j)
            {
                Vector3 pos = new Vector3(startX + i, 0, startZ + j);
                Instantiate(spawnObject).transform.SetPositionAndRotation(pos, new Quaternion());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
