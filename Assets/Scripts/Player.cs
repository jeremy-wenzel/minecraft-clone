using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public const int TRANS_SPEED = 10;
        public const int SPRINT_SPEED = 2;
        public const int FORCE_MULTIPLIER = 500;

        public Camera camera;

        private bool isPlayerJumping = false;

        private void Update()
        {
            HandleAction();
            SetTranslation();
            MaintainUpgrightRotation();
        }

        private void HandleAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.distance < 3f)
                {
                    Debug.Log("Looking at " + hit.transform.tag);
                    Cube cube = hit.transform.gameObject.GetComponent<Cube>();
                    if (cube == null)
                    {
                        Debug.LogError("gameobject {hit.transform.tag} is not cube type");
                        return;
                    }
                    cube.MineCube();
                }
            }
        }

        /// <summary>
        /// Maintins the upright orientation of the player
        /// </summary>
        private void MaintainUpgrightRotation()
        {
            Vector3 eulerAngles = this.transform.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;
            this.transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// Sets the translation of the parent object.
        /// </summary>
        private void SetTranslation()
        {
            // TODO: This should go somewhere else
            bool isRestart = Input.GetKey(KeyCode.P);
            if (isRestart)
            {
                this.transform.SetPositionAndRotation(new Vector3(0, 8, 0), new Quaternion());
                return;
            }

            // TODO: This should go somewhere else
            bool exit = Input.GetKey(KeyCode.Escape);
            if (exit)
            {
                Application.Quit();
            }

            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isJumping = Input.GetKey(KeyCode.Space);
            float xTranslate = Input.GetAxis("Horizontal");
            float zTranslate = Input.GetAxis("Vertical");
            Vector3 trans = new Vector3(xTranslate, 0, zTranslate) * TRANS_SPEED * Time.deltaTime;
            if (isSprinting)
            {
                trans *= SPRINT_SPEED;
            }

            if (!isPlayerJumping && isJumping)
            {
                isPlayerJumping = true;
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.AddRelativeForce(Vector3.up * FORCE_MULTIPLIER);
            }

            gameObject.transform.Translate(trans);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // TODO: I don't like this but I am not sure of a better way.
            isPlayerJumping = false;
        }
    }
}
