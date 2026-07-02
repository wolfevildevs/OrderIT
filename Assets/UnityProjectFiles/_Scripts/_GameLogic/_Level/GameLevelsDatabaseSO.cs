using System.Collections.Generic;
using UnityEngine;

namespace RunnerGame.Level
{
    [CreateAssetMenu(fileName = "GameLevelsDatabase", menuName = "RunnerGame/Game Levels Database")]
    public class GameLevelsDatabaseSO : ScriptableObject
    {
        [Tooltip("The master list of all levels in the game, ordered sequentially.")]
        [SerializeField] private List<LevelDataSO> allLevels = new List<LevelDataSO>();

        public int TotalLevels => allLevels.Count;

        // Safely retrieve level data by index (handles out of bounds cleanly)
        public LevelDataSO GetLevelData(int levelIndex)
        {
            if (allLevels.Count == 0) return null;

            // Loop back if the player beats all levels (Infinite loops) or lock to last level
            int safeIndex = levelIndex % allLevels.Count;
            return allLevels[safeIndex];
        }

        /// <summary>
        /// Validates if a level index exists within the database collection boundaries.
        /// </summary>
        public bool HasLevel(int levelIndex)
        {
            if (allLevels == null || allLevels.Count == 0) return false;
            return levelIndex < allLevels.Count;
        }
        
    }
}