using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public float maxXyDistance = 40;
    public GameObject[] treePrefabs;
    public int numTrees = 40;
    public GameObject[] bushPrefabs;
    public int numBushes = 60;
    public GameObject[] grassPrefabs;
    public int numGrass = 5000;

    //
    private float levelRadius = 50f;

    // Start is called before the first frame update
    void Start()
    {
        //
        CheckGameManagerAndGetTerrainData();
        //
        SpawnGroup(treePrefabs, numTrees);
        SpawnGroup(bushPrefabs, numBushes);
        SpawnGroup(grassPrefabs, numGrass);

        SpawnTreeCircle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckGameManagerAndGetTerrainData()
    {
        if (GameManager.Instance && GameManager.Instance.currentTerrainSet)
        {
            maxXyDistance = GameManager.Instance.currentTerrainSet.maxXyDistance;
            treePrefabs = GameManager.Instance.currentTerrainSet.treePrefabs;
            numTrees = GameManager.Instance.currentTerrainSet.numTrees;
            bushPrefabs = GameManager.Instance.currentTerrainSet.bushPrefabs;
            numBushes = GameManager.Instance.currentTerrainSet.numBushes;
            grassPrefabs = GameManager.Instance.currentTerrainSet.grassPrefabs;
            numGrass = GameManager.Instance.currentTerrainSet.numGrass;
        }
    }

    void SpawnGroup(GameObject[] group, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            int objectToSpawn = Random.Range(0, group.Length);
            SpawnTerrainPiece(group[objectToSpawn]);
        }
    }

    void SpawnTerrainPiece(GameObject prefab)
    {
        //Instantiate(prefab, new Vector3(Random.Range(-maxXyDistance, maxXyDistance), 0, 
        //    Random.Range(-maxXyDistance, maxXyDistance)), Quaternion.identity);
        //float circumference = levelRadius * 2 * Mathf.PI;
        float angle = Random.Range(0f, 360f);
        //float radius = Random.Range(0f, levelRadius);
        float radius = Mathf.Sqrt(Random.Range(0f, 1f)) * levelRadius;
        float xPosition = radius * Mathf.Cos(angle);
        float zPosition = radius * Mathf.Sin(angle);
        Instantiate(prefab, new Vector3(xPosition, 0, zPosition), Quaternion.identity);
    }

    void SpawnTreeCircle()
    {
        //float radius = 50;
        int treeRows = 3;
        for(int i = 0; i < treeRows; i++)
        {
            float rowRadius = levelRadius + i;
            float circumference = rowRadius * 2 * Mathf.PI;
            //
            for (int j = 0; j < circumference; j++)
            {
                int treeIndex = Random.Range(0, treePrefabs.Length);
                float angle = j * (360f / circumference);
                float xPosition = rowRadius * Mathf.Cos(angle);
                float zPosition = rowRadius * Mathf.Sin(angle);
                Instantiate(treePrefabs[treeIndex], new Vector3(xPosition, 0, zPosition), Quaternion.identity);
            }
        }
        
    }
}
