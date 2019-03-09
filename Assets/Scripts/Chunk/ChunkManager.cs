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

        /// <summary>
        /// Does the Chunk currently exist?
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public static bool ChunkExists(Chunk chunk)
        {
            return ChunkExists(chunk.GetKey());
        }

        /// <summary>
        /// Does the Chunk currently exists?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ChunkExists(string key)
        {
            return totalChunks.ContainsKey(key);
        }

        /// <summary>
        /// Get the Chunk from the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Chunk GetChunkWithKey(string key)
        {
            if (!ChunkExists(key))
            {
                Debug.LogWarning($"Chunk does not exist for {key}");
                return null;
            }

            return totalChunks[key];
        }

        /// <summary>
        /// Adds the Chunk to the manager
        /// </summary>
        /// <param name="chunk"></param>
        public static void AddChunk(Chunk chunk)
        {
            totalChunks.Add(chunk.GetKey(), chunk);
        }

        /// <summary>
        /// Removes the Chunk from the manager
        /// </summary>
        /// <param name="chunk"></param>
        public static void RemoveChunk(Chunk chunk)
        {
            if (totalChunks.ContainsKey(chunk.GetKey()))
            {
                totalChunks.Remove(chunk.GetKey());
            }
        }
    }
}
