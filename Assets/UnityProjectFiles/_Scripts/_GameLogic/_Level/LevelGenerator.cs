using UnityEngine;

namespace RunnerGame.Level
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Data Management Links")]
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private GameLevelsDatabaseSO levelsDatabase;

        [Header("Spawn Anchor")]
        [SerializeField] private Transform spawnStartPoint; 

        private float nextSpawnZ = 0f;

        private void Awake()
        {
            if (spawnStartPoint != null)
            {
                nextSpawnZ = spawnStartPoint.position.z;
            }

            BuildActivePlayerLevel();
        }

        private void BuildActivePlayerLevel()
        {
            if (playerData == null || levelsDatabase == null)
            {
                Debug.LogError("LevelGenerator: Missing PlayerData or LevelsDatabase references!");
                return;
            }

            // 1. Fetch the exact level data based on the player's progression index
            LevelDataSO activeLevelData = levelsDatabase.GetLevelData(playerData.currentLevelIndex);

            if (activeLevelData == null || activeLevelData.chunksOrder.Count == 0)
            {
                Debug.LogError($"LevelGenerator: Level Data for index {playerData.currentLevelIndex} is empty or null!");
                return;
            }

            Debug.Log($"<color=green>LevelGenerator: Successfully loading and generating Level {playerData.currentLevelIndex + 1}</color>");

            // 2. Stitch the chunks together perfectly for this specific level
            for (int i = 0; i < activeLevelData.chunksOrder.Count; i++)
            {
                LevelChunk chunkPrefab = activeLevelData.chunksOrder[i];

                if (chunkPrefab == null) continue;

                Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
                LevelChunk spawnedChunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity, transform);
                
                nextSpawnZ += spawnedChunk.ChunkLength;
            }
        }
    }
}