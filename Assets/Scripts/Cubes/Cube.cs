using UnityEngine;

namespace Assets.Scripts
{
    public class Cube : MonoBehaviour
    {
        public AudioClip BreakSound;
        public AudioClip PlaceSound;

        public int X => (int)gameObject.transform.position.x;
        public int Y => (int)gameObject.transform.position.y;
        public int Z => (int)gameObject.transform.position.z;

        public Chunk ParentChunk { get; private set; }

        public bool WasPlaced { get; private set; }

        /// <summary>
        /// Mines the cube
        /// </summary>
        public virtual void MineCube()
        {
            ParentChunk.MineCube(this);
        }

        /// <summary>
        /// Deactivates the cube and sets the parent object to null
        /// </summary>
        public void DeactivateCube()
        {
            ParentChunk = null;
            SetVisibility(false);
        }

        public void Darken()
        {
            Material buildingMaterial = gameObject.GetComponent<Renderer>().material;
            buildingMaterial.SetColor("Test", new Color(0, 0, 0, 0));
        }

        public virtual void AddCube(Vector3 surfaceNormal, GameObject prefab)
        {
            ParentChunk.CreateNewCube(new Vector3(X + surfaceNormal.x, Y + surfaceNormal.y, Z + surfaceNormal.z), prefab);
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }

        /// <summary>
        /// Activate the gameobject
        /// </summary>
        /// <param name="parentChunk"></param>
        public void Spawn(Chunk parentChunk, bool wasPlaced)
        {
            this.ParentChunk = parentChunk;
            // TODO: Should we check if the object is already active so we are not reactivating?
            SetVisibility(true);
            WasPlaced = wasPlaced;
        }

        public void SetVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public virtual bool ShouldBeUsedForDeterminingColumnHeight()
        {
            return true;
        }
    }
}   