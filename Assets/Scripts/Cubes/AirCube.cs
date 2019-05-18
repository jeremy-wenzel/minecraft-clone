using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class AirCube : Cube
    {
        public override void MineCube()
        {
            // don't do anything
        }

        public override bool ShouldBeUsedForDeterminingColumnHeight()
        {
            return false;
        }
    }
}
