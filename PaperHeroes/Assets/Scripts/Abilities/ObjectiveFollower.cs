using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveFollower : BaseAbility
{
    //
    //public Transform objective;
    public float speed = 1;
    public bool explodesOnContact;
    public float duration = 10;
    public GameObject onDestroyPrefab;
    public bool getNearestObjective = false;
    //public Owner owner;

    // Start is called before the first frame update
    void Start()
    {
        //
        if(!objective)
            GetNearestObjective();
        //
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        if (objective)
        {
            transform.LookAt(objective);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else // Que busque otro
        {
            GetNearestObjective();
        }
    }

    private void OnDestroy()
    {
        if (onDestroyPrefab)
        {
            GameObject onDestroyEffect = Instantiate(onDestroyPrefab, transform.position, Quaternion.identity);
            BaseAbility baseAbility = onDestroyEffect.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.attackMultiplier = attackMultiplier;
                baseAbility.owner = owner;
            }
                
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.collider.name);
        if (explodesOnContact && collision.collider.transform == objective)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger with: " + other.name);
        if (explodesOnContact && other.transform == objective)
            Destroy(gameObject);
    }

    void GetNearestObjective()
    {
        HealthController[] healthControllers = FindObjectsOfType<HealthController>();
        float nearestDistance = Mathf.Infinity;
        float nextDistance;
        //
        for(int i = 0; i < healthControllers.Length; i++)
        {
            nextDistance = (healthControllers[i].transform.position - transform.position).sqrMagnitude;
            if(nextDistance < nearestDistance && healthControllers[i].OwnerCharacter != owner)
            {
                objective = healthControllers[i].transform;
                nearestDistance = nextDistance;
            }
        }
    }
}
