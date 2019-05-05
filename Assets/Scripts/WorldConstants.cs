using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class WorldConstants
    {
        public const int CHUNK_SIZE = 8;
        public const float SCALE_FACTOR = 20f;
        public const float WORLD_SCALE = 5f;
        public const float STEEPNESS_SCALE = 200f;
        public const int OFFSET = 1000;
        public const int SNOW_MAX_Y = 8;

        private WorldConstants()
        {
            
        }
    }
}
