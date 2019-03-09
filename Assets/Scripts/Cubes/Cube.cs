using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Scripts
{
    public class Cube : MonoBehaviour
    {
        public int Y => (int)gameObject.transform.position.y;

        public Chunk ParentChunk { get; set; }

        public void DeleteFromChunk()
        {
            ParentChunk.DeleteCube(this);
        }

        public Tuple<int, int> GetCoordinates()
        {
            int x = (int)gameObject.transform.position.x;
            int z = (int)gameObject.transform.position.z;
            return new Tuple<int, int>(x, z);
        }
    }
}   