using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // The target to keep the camera on
    public const int ROT_SPEED = 10;

    public GameObject playerObject;

    private bool _isPlayerJumping;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetRotations();
    }

    /// <summary>
    /// Sets the rotation of the camera and the player object
    /// </summary>
    private void SetRotations()
    {
        // how can we dynamically change this so that it could get from a controller
        // Manager?
        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");
        Vector3 rot = new Vector3(xRot, yRot, 0) * ROT_SPEED * Time.deltaTime;

        // Lock the X. By setting to 0 the rotation doesn't do anything
        float totalX = this.transform.eulerAngles.x + rot.x;
        if (totalX > 90 && totalX < 270)
        {
            rot.x = 0;
        }

        // Set Child X to rot.x
        Vector3 currentAngles = this.transform.eulerAngles;
        currentAngles.x += rot.x;
        this.transform.eulerAngles = currentAngles;

        // Set parent Y to rot.y
        Transform parent = transform.parent;
        currentAngles = parent.eulerAngles;
        currentAngles.y += rot.y;
        parent.eulerAngles = currentAngles;
    }

    
}
