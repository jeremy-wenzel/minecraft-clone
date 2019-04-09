using UnityEngine;
using System.Collections;

public class SunMovement : MonoBehaviour
{
    private float currentTime = 0;
    private const float speed = .5f;
    private float startY = 100;
    private float startX = 100;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * speed;
        this.transform.position = new Vector3(Mathf.Cos(currentTime) * startX, Mathf.Sin(currentTime) * startY, 0);

        this.transform.LookAt(Vector3.zero);
    }
}
