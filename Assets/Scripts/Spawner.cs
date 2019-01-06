using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(PrefabManager.GetPrefab(PrefabType.CHUNK), new Vector3(10, 0, 0), new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
