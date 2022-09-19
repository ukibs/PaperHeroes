using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcMovement : BaseAbility
{
    public float targetlessReach = 10;
    public float duration = 3;
    public float maxHeight = 3;
    public GameObject spawnOnDestinationPrefab;

    private float currentTime = 0;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        //
        initialPosition = transform.position;
        //
        if (destination == Vector3.zero)
            destination = transform.position + (transform.forward * targetlessReach);
        //
        if (spawnOnDestinationPrefab)
        {
            GameObject spawnOnDestination = Instantiate(spawnOnDestinationPrefab, destination, Quaternion.identity);
            BaseAbility baseAbility = spawnOnDestination.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.attackMultiplier = attackMultiplier;
                baseAbility.owner = owner;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        currentTime += dt;
        // Horizontal 
        transform.position = Vector3.Lerp(initialPosition, destination, currentTime / duration);
        // Vertical
        float currentHeight = Mathf.Sin(Mathf.PI * currentTime / duration) * maxHeight;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        //
        if(currentTime >= duration)
        {            
            Destroy(gameObject);
        }
    }
}
