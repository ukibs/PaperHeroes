using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : BaseAbility
{
    public enum EffectTime
    {
        OnCreation,
        Sustained,
        PerTicks,

        Count
    }

    public enum EffectTipe
    {
        Damage,
        Healing,

        Count
    }

    //public enum Owner
    //{
    //    Invalid = -1,
    //    Player,
    //    Enemy,

    //    Count
    //}

    public EffectTime effectTime;
    public EffectTipe effectType;

    public float lifeTime;
    public float range = 5;
    public int amount = 5;
    public int strength = 1;
    public float timeBetweenTicks = 1;

    public AlteredState.Type alteredState = AlteredState.Type.None;
    public float alteredStateDuration;

    //public Owner owner;

    // Start is called before the first frame update
    void Start()
    {
        //
        Destroy(gameObject, lifeTime);
        //
        if(effectTime == EffectTime.OnCreation)
        {
            ApplyEffect();
        }
        //
        if(effectTime == EffectTime.PerTicks)
        {
            StartCoroutine(TickBucle());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (effectTime == EffectTime.Sustained)
        {
            ApplyEffect();
        }
    }

    void ApplyEffect()
    {
        //
        RaycastHit[] hitInfo = Physics.SphereCastAll(transform.position, range, transform.forward, 0.1f);
        //
        if(hitInfo.Length > 0)
        {
            //
            for(int i = 0; i < hitInfo.Length; i++)
            {
                //
                HealthController healthController = hitInfo[i].collider.GetComponent<HealthController>();
                if (healthController)
                {
                    //
                    int totalAmount = (int)(attackMultiplier * amount);
                    //
                    switch (effectType)
                    {
                        case EffectTipe.Damage: 
                            if(owner != healthController.OwnerCharacter)
                            {                                
                                healthController.ReceiveAttack(totalAmount, alteredState, alteredStateDuration);
                                
                            }                                
                            break;
                        case EffectTipe.Healing:                             
                            if (owner == healthController.OwnerCharacter)
                            {
                                healthController.Restore(totalAmount);
                            }
                            break;
                    }                    
                }

                //
                ProyectilControl proyectilControl = hitInfo[i].collider.GetComponent<ProyectilControl>();
                //Debug.Log(gameObject.name + " collided with " + proyectilControl);
                //
                if (proyectilControl && proyectilControl.owner != owner && strength >= proyectilControl.strength)
                {
                    Debug.Log(gameObject.name + " destroyed " + proyectilControl.name + " - " + proyectilControl.owner + ", " + owner);
                    Destroy(proyectilControl.gameObject);
                }
            }
        }
    }

    IEnumerator TickBucle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenTicks);
            ApplyEffect();
        }
    }
}
