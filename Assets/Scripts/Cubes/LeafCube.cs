using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Cubes
{
    public class LeafCube : TreeCube
    {
        public override bool ShouldBeUsedForDeterminingColumnHeight()
        {
            return false;
        }
    }
}
