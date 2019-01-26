using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 50;

    public float startX;
    public float startZ;

    private const float _scaleFactor = 20f;
    private const float _worldScale = 5f;
    private const float _steepnessScale = 200f;
    private const int _offset = 1000;
    private static readonly Perlin perlin = new Perlin();

    // Start is called before the first frame update
    void Start()
    {
        startX = gameObject.transform.position.x;
        startZ = gameObject.transform.position.z;
        for (int i = 0; i < CHUNK_SIZE; ++i)
        {
            for (int j = 0; j < CHUNK_SIZE; ++j)
            {
                float newX = (startX + i + _offset) / _scaleFactor;
                float newZ = (startZ + j + _offset) / _scaleFactor;

                // this essentially allows us to generate the steepness. Dividing by _worldScale
                // allows us to have plains and montains because the steepness spans over a longer distance
                float steepnessY = perlin.DoPerlin(newX / _worldScale, newZ / _worldScale) * _steepnessScale;

                float totalY =  perlin.DoPerlin(newX, newZ) * steepnessY;
                Debug.Log($"Y value = {steepnessY}, Total Y = {(int)totalY}");
                Vector3 pos = new Vector3(startX + i, (int)totalY, startZ + j);
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
