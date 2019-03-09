using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    sealed class ChunkManager
    {
        private static Dictionary<string, Chunk> totalChunks = new Dictionary<string, Chunk>();

        private ChunkManager()
        {

        }

        public static bool ChunkExists(Chunk chunk)
        {
            return ChunkExists(chunk.GetKey());
        }

        public static bool ChunkExists(string key)
        {
            return totalChunks.ContainsKey(key);
        }

        public static Chunk GetChunkWithKey(string key)
        {
            if (!ChunkExists(key))
            {
                Debug.LogWarning($"Chunk does not exist for {key}");
                return null;
            }

            return totalChunks[key];
        }

        public static void AddChunk(Chunk chunk)
        {
            totalChunks.Add(chunk.GetKey(), chunk);
        }

        public static void DestroyChunk(Chunk chunk)
        {
            if (totalChunks.ContainsKey(chunk.GetKey()))
            {
                totalChunks.Remove(chunk.GetKey());
            }
        }
    }
}
