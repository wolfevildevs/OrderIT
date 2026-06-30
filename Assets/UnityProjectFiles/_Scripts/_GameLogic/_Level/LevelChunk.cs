using UnityEngine;

namespace RunnerGame.Level
{
    public class LevelChunk : MonoBehaviour
    {
        [Header("Chunk Dimensions")]
        [Tooltip("The exact physical length of this chunk prefab along the Z axis")]
        [SerializeField] private float chunkLength = 30f;

        // Public property to read the length safely from the generator
        public float ChunkLength => chunkLength;
    }
}