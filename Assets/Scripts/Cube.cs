using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    public class Cube
    {
        public GameObject gameObject { get; private set; }

        public int VerticalPosition => (int)gameObject.transform.position.y;

        public Cube(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }



    }
}   