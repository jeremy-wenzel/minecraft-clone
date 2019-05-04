using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public const int TRANS_SPEED = 10;
        public const int SPRINT_SPEED = 2;
        public const int FORCE_MULTIPLIER = 500;

        public Camera camera;
        public GameObject inventoryGameObject;
        private Inventory inventory;
        private AudioSource audioSource;

        private bool isPlayerJumping = false;
        private bool isInWater = false;

        private static readonly Vector3 WATER_GRAVITY = new Vector3(0, -1.0f, 0);
        private static readonly Vector3 NORMAL_GRAVITY = new Vector3(0, -9.81f, 0);

        private void Start()
        {
            inventory = new Inventory();
            audioSource = this.GetComponent<AudioSource>();
            SetCurrentInventoryItem();
        }

        private void Update()
        {
            if (!PauseMenuScript.GamePaused)
            {
                HandleAction();
                SetTranslation();
                MaintainUpgrightRotation();
            }
        }

        private void HandleAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.distance < 5f)
                {
                    Cube cube = hit.transform.gameObject.GetComponent<Cube>();
                    if (cube == null)
                    {
                        Debug.LogError("gameobject {hit.transform.tag} is not cube type");
                        return;
                    }

                    // This isn't great. could potentially abstrac this out.
                    audioSource.clip = cube.BreakSound;
                    audioSource.Play();
                    cube.MineCube();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.distance < 5f)
                {
                    Cube cube = hit.transform.gameObject.GetComponent<Cube>();
                    if (cube == null)
                    {
                        Debug.LogError("gameobject {hit.transform.tag} is not cube type");
                        return;
                    }

                    cube.AddCube(hit.normal, inventory.GetCurrentItem());
                    audioSource.clip = inventory.GetCurrentItem().GetComponent<Cube>().PlaceSound;
                    audioSource.Play();
                }
            }

            else if (Input.mouseScrollDelta.y != 0)
            {
                Destroy(inventoryGameObject.transform.GetChild(0).gameObject);
                SetCurrentInventoryItem();
            }
        }

        private void SetCurrentInventoryItem()
        {
            GameObject nextInventoryObject;
            if (Input.mouseScrollDelta.y > 0)
            {
                nextInventoryObject = inventory.GetNextItem();
            }
            else
            {
                nextInventoryObject = inventory.GetPreviousItem();
            }

            var newObject = Instantiate(nextInventoryObject, inventoryGameObject.transform.position, nextInventoryObject.GetComponent<InventoryObject>().GetRotation());
            newObject.transform.localScale = newObject.GetComponent<InventoryObject>().GetScale();
            Destroy(newObject.GetComponent<BoxCollider>());
            newObject.transform.SetParent(inventoryGameObject.transform);
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

            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isJumping = Input.GetKey(KeyCode.Space);
            float xTranslate = Input.GetAxis("Horizontal");
            float zTranslate = Input.GetAxis("Vertical");
            Vector3 trans = new Vector3(xTranslate, 0, zTranslate) * TRANS_SPEED * Time.deltaTime;
            if (isInWater && Physics.gravity == NORMAL_GRAVITY)
            {
                trans *= .1f;
                Physics.gravity = WATER_GRAVITY;
                
            }
            else if (!isInWater && Physics.gravity == WATER_GRAVITY)
            {
                Physics.gravity = NORMAL_GRAVITY;
            }

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Water")
            {
                isInWater = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Water")
            {
                isInWater = false;
            }
        }
    }
}
