using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    // The target to keep the camera on
    public const int ROT_SPEED = 10;
    public const int TRANS_SPEED = 10;
    public const int SPRINT_SPEED = 2;

    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetRotations();
        SetTranslation();
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
        currentAngles = playerObject.transform.eulerAngles;
        currentAngles.y += rot.y;
        playerObject.transform.eulerAngles = currentAngles;
    }

    /// <summary>
    /// Sets the translation of the parent object.
    /// </summary>
    private void SetTranslation()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float xTranslate = Input.GetAxis("Horizontal");
        float zTranslate = Input.GetAxis("Vertical");
        Vector3 trans = new Vector3(xTranslate, 0, zTranslate) * TRANS_SPEED * Time.deltaTime;
        if (isSprinting)
        {
            trans *= SPRINT_SPEED;
        }

        playerObject.transform.Translate(trans);
    }
}
