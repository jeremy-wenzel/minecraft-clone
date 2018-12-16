using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject gameObject;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
