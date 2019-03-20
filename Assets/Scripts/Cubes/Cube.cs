using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Scripts
{
    public class Cube : MonoBehaviour
    {
        public int X => (int)gameObject.transform.position.x;
        public int Y => (int)gameObject.transform.position.y;
        public int Z => (int)gameObject.transform.position.z;

        private Chunk ParentChunk;

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

        /// <summary>
        /// Activate the gameobject
        /// </summary>
        /// <param name="parentChunk"></param>
        public void Spawn(Chunk parentChunk)
        {
            this.ParentChunk = parentChunk;
            // TODO: Should we check if the object is already active so we are not reactivating?
            SetVisibility(true);
        }

        public void SetVisibility(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}   