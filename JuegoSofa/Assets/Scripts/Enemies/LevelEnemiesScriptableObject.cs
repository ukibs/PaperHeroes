using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEnemiesData", menuName = "ScriptableObjects/LevelEnemiesData", order = 1)]
public class LevelEnemiesScriptableObject : ScriptableObject
{
    [Header("Waves")]
    public CompleteWave[] waves;
    public CompleteWaveScriptableObject[] prefabWaves;
    public bool usePrefabOnes = false;
}
