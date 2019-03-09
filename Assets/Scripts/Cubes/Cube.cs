using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Scripts
{
    public class Cube : MonoBehaviour
    {
        public int Y => (int)gameObject.transform.position.y;

        private Chunk ParentChunk;

        public void DeleteFromChunk()
        {
            ParentChunk.DeleteCube(this);
            ParentChunk = null;
            gameObject.SetActive(false);
        }

        public Tuple<int, int> GetCoordinates()
        {
            int x = (int)gameObject.transform.position.x;
            int z = (int)gameObject.transform.position.z;
            return new Tuple<int, int>(x, z);
        }

        public void Spawn(Chunk parentChunk)
        {
            this.ParentChunk = parentChunk;
            this.gameObject.SetActive(true);
        }
    }
}   