using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAndAreaMarker : BaseAbility
{
    public GameObject spawnablePrefab;
    public float timeToSpawn = 1;
    public float abilityRange = 1;

    private float currentTimeToSpawn = 0;
    private SpriteRenderer growingSpriteRenderer;
    private SpriteRenderer fixedSpriteRenderer;
    private Vector3 spriteInitialScale;

    // Start is called before the first frame update
    void Start()
    {
        growingSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[0];
        fixedSpriteRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        spriteInitialScale = growingSpriteRenderer.transform.localScale;
        //spriteRenderer.transform.localScale = spriteInitialScale * abilityRange;
        growingSpriteRenderer.transform.localScale = Vector3.zero;
        fixedSpriteRenderer.transform.localScale = spriteInitialScale * abilityRange;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeToSpawn += Time.deltaTime;
        //
        growingSpriteRenderer.transform.localScale = spriteInitialScale * (currentTimeToSpawn / timeToSpawn) * abilityRange;
        //
        if (currentTimeToSpawn >= timeToSpawn)
        {
            GameObject spawnedAbility = Instantiate(spawnablePrefab, transform.position, Quaternion.identity);
            //
            BaseAbility baseAbility = spawnedAbility.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.attackMultiplier = attackMultiplier;
                baseAbility.owner = owner;
            }
            
            //
            Destroy(gameObject);
        }
    }
}
