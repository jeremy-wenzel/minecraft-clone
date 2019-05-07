using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class InventoryObject : MonoBehaviour
    {
        public Vector3 Rotation;
        public Vector3 Scale;
        public bool CanMine;

        public Quaternion GetRotation()
        {
            if (Rotation == null)
            {
                Rotation = new Vector3(0, 0, 0);
            }
            return Quaternion.Euler(Rotation);
        }

        public Vector3 GetScale()
        {
            if (GetComponent<Cube>() != null)
            {
                Scale = new Vector3(.1f, .1f, .1f);
            }
            return Scale;
        }
    }
}
