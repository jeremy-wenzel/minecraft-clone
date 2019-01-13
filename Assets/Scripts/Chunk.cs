using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 10;

    public float startX;
    public float startZ;

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
                Instantiate(PrefabManager.GetPrefab(PrefabType.CUBE)).transform.SetPositionAndRotation(pos, new Quaternion());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsPositionInChunk(Vector3 pos)
    {
        return pos.x > startX && pos.x < startX + CHUNK_SIZE && pos.z > startZ && pos.z < startZ + CHUNK_SIZE;
    }

    public string GetKey()
    {
        return GetKey(gameObject.transform.position);
    }

    public static string GetKey(Vector3 position)
    {
        float x = position.x / CHUNK_SIZE;
        float z = position.z / CHUNK_SIZE;

        Debug.Log($"{x.ToString("f0")} {z.ToString("f0")}");

        return $"{x.ToString("f0")} {z.ToString("f0")}";
    }
}
