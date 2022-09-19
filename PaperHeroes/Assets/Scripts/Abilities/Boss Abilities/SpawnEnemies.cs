using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public  CompleteWaveScriptableObject enemyWave;
    public bool givesPoints;
    // Start is called before the first frame update
    void Start()
    {
        //
        for(int i = 0; i < enemyWave.completeWave.enemies.Length; i++)
        {
            EnemiesManager.Instance.SpawnEnemies(enemyWave.completeWave.enemies[i], transform.position, EnemiesManager.SpawnMode.Circular, givesPoints);
        }
        //
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
