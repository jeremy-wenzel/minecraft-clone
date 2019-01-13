using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // The target to keep the camera on
    public const int ROT_SPEED = 10;
    public const int TRANS_SPEED = 10;

    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // how can we dynamically change this so that it could get from a controller
        // Manager?
        float yRot = Input.GetAxis("Mouse X"); 
        float xRot = Input.GetAxis("Mouse Y");
        Vector3 rot = new Vector3(xRot, yRot, 0) * ROT_SPEED * Time.deltaTime;

        float xTranslate = Input.GetAxis("Horizontal");
        float zTranslate = Input.GetAxis("Vertical");
        Vector3 trans = new Vector3(xTranslate, 0, zTranslate) * TRANS_SPEED * Time.deltaTime;

        // Lock the X. By setting to 0 the rotation doesn't do anything
        float totalX = gameObject.transform.eulerAngles.x + rot.x;
        if (totalX > 90 && totalX < 270)
        {
            rot.x = 0;
        }

        Vector3 currentAngles = gameObject.transform.eulerAngles + rot;
        currentAngles.z = 0;
        gameObject.transform.eulerAngles = currentAngles;
        gameObject.transform.Translate(trans);
    }
}
