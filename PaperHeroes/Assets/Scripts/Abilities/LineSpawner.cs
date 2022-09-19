using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : BaseAbility
{
    public GameObject spawnablePrefab;
    public float distanceBetweenSpawns = 1;
    public float timeBetweenSpawns = 0.5f;
    public int totalSpawns = 10;

    private int currentSpawn = 0;
    private float currentTimeBetweenSpawns = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeBetweenSpawns += Time.deltaTime;
        //
        if(currentTimeBetweenSpawns >= timeBetweenSpawns)
        {
            GameObject spawnedAbility = Instantiate(spawnablePrefab, 
                transform.position + (transform.forward * distanceBetweenSpawns * currentSpawn), Quaternion.identity);
            //
            BaseAbility baseAbility = spawnedAbility.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.attackMultiplier = attackMultiplier;
                baseAbility.owner = owner;
            }
            
            //
            currentTimeBetweenSpawns = 0;
            currentSpawn++;
            //
            if(currentSpawn >= totalSpawns)
            {
                Destroy(gameObject);
            }
        }
    }
}
