using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "ScriptableObjects/TerrainData", order = 1)]
public class TerrainSetScriptableObject : ScriptableObject
{
    public float maxXyDistance = 40;
    public GameObject[] treePrefabs;
    public int numTrees = 40;
    public GameObject[] bushPrefabs;
    public int numBushes = 60;
    public GameObject[] grassPrefabs;
    public int numGrass = 5000;
}
