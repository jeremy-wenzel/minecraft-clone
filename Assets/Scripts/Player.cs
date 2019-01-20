using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public const int TRANS_SPEED = 10;
        public const int SPRINT_SPEED = 2;
        public const int FORCE_MULTIPLIER = 100;

        private bool _isPlayerJumping = false;

        private void Update()
        {
            SetTranslation();
            MaintainUpgrightRotation();
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
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isJumping = Input.GetKey(KeyCode.Space);
            float xTranslate = Input.GetAxis("Horizontal");
            float zTranslate = Input.GetAxis("Vertical");
            Vector3 trans = new Vector3(xTranslate, 0, zTranslate) * TRANS_SPEED * Time.deltaTime;
            if (isSprinting)
            {
                trans *= SPRINT_SPEED;
            }

            if (!_isPlayerJumping && isJumping)
            {
                _isPlayerJumping = true;
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.AddRelativeForce(Vector3.up * FORCE_MULTIPLIER);
            }

            gameObject.transform.Translate(trans);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // TODO: I don't like this but I am not sure of a better way.
            _isPlayerJumping = false;
        }
    }
}
