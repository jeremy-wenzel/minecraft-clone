using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Scripts
{
    public class Cube : MonoBehaviour
    {
        public int Y => (int)gameObject.transform.position.y;

        private Chunk ParentChunk;

        /// <summary>
        /// Removes the Cube from the Chunk and disables it's gameobject
        /// </summary>
        public void DeleteFromChunk()
        {
            ParentChunk.DeleteCube(this);
            ParentChunk = null;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Activate the gameobject
        /// </summary>
        /// <param name="parentChunk"></param>
        public void Spawn(Chunk parentChunk)
        {
            this.ParentChunk = parentChunk;
            // TODO: Should we check if the object is already active so we are not reactivating?
            this.gameObject.SetActive(true);
        }
    }
}   