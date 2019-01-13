using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    sealed class ChunkManager
    {
        private static Dictionary<string, Chunk> _totalChunks = new Dictionary<string, Chunk>();

        private ChunkManager()
        {

        }

        public static bool ChunkExists(Chunk chunk)
        {
            return ChunkExists(chunk.GetKey());
        }

        public static bool ChunkExists(string key)
        {
            return _totalChunks.ContainsKey(key);
        }

        public static Chunk GetChunkWithKey(string key)
        {
            if (!ChunkExists(key))
            {
                return null;
            }

            return _totalChunks[key];
        }

        public static void AddChunk(Chunk chunk)
        {
            _totalChunks.Add(chunk.GetKey(), chunk);
        }
    }
}
