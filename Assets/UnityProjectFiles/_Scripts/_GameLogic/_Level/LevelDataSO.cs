using System.Collections.Generic;
using UnityEngine;
using RunnerGame.Level; // FIX: This links the script directly to see LevelChunk!

[CreateAssetMenu(fileName = "NewLevelData", menuName = "RunnerGame/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Tooltip("The exact sequence of chunks for this specific level")]
    public List<LevelChunk> chunksOrder = new List<LevelChunk>();
}