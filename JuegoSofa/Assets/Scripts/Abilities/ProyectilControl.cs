using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilControl : BaseAbility
{
    //public enum Owner
    //{
    //    Invalid = -1,
    //    Player,
    //    Enemy,

    //    Count
    //}

    // Parametros
    [Header("Base Parameters")]
    public float lifeTime = 2;
    public float speed = 10;
    public int damage = 1;
    public int strength = 1;
    [Header("Altered State")]
    public AlteredState.Type alteredState = AlteredState.Type.None;
    public float alteredStateDuration = 0;
    [Header("On destruction effect")]
    public GameObject onDestroyPrefab;
    
    //[HideInInspector] public Owner owner;
    [HideInInspector] public HealthController impactedHealthController;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    //Colision
    private void OnCollisionEnter(Collision collision)
    {
        ManageImpact(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        ManageImpact(other);
    }

    void ManageImpact(Collider collider)
    {
        ProyectilControl proyectilControl = collider.GetComponent<ProyectilControl>();
        if (proyectilControl && proyectilControl.strength >= strength)
        {
            Destroy(gameObject);
            return;
        }

        //
        HealthController healthController = collider.GetComponent<HealthController>();
        if (healthController)
        {
            int totalDamage = (int)(damage * attackMultiplier);
            healthController.ReceiveAttack(totalDamage, alteredState, alteredStateDuration);
            // De momento guardamos el collider impactado solo aqui
            //impactedCollider = collision.collider;
            //impactedCharacterController = collision.collider.GetComponent<CharacterController>();
            impactedHealthController = healthController;
            Destroy(gameObject);
            return;
        }

        // Con colliders del escenario se destruye y ya
        if (!proyectilControl && !healthController)
        {
            Debug.Log(gameObject.name + " colliding with scenario");
            Destroy(gameObject);
        }
            
    }

    private void OnDestroy()
    {
        //
        Debug.Log(name + " destroyed");
        //
        if (onDestroyPrefab)
        {
            GameObject onDestroyEffect = Instantiate(onDestroyPrefab, transform.position, transform.rotation);
            //
            BaseAbility baseAbility = onDestroyEffect.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.attackMultiplier = attackMultiplier;
                baseAbility.owner = owner;
            }
                
            //
            //Debug.Log(gameObject.name + " - Impacted collider: " + impactedCollider);
            //Debug.Log(gameObject.name + " - Impacted health controller: " + impactedHealthController);
            if (impactedHealthController)
            {
                AttachedEffect attachedEffect = onDestroyEffect.GetComponent<AttachedEffect>();
                if (attachedEffect)
                {
                    attachedEffect.objective = impactedHealthController.transform;
                    //attachedEffect.transform.parent = impactedHealthController.transform;
                    //attachedEffect.transform.localPosition = Vector3.zero;
                    Debug.Log(attachedEffect.gameObject.name + " attached to " + impactedHealthController.name);
                }
            }            
        }
    }
}
