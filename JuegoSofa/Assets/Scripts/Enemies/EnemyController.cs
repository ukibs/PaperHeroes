using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class EnemyController : BaseController
{
    //
    public float rateOfFire = 1;
    //public float movementSpeed = 1;
    public GameObject proyectilePrefab;
    public int score = 1;
    public Behaviour[] behaviours;

    //
    protected PlayersManager playersManager;
    //private HealthController healthController;
    //private float attackCooldown = 0;
    //private SpriteController spriteController;
    //private CharacterController characterController;
    //private LevelManager levelManager;
    protected EnemiesManager enemiesManager;
    //[HideInInspector] public bool isDead = false;
    protected Behaviour[] currentBehaviours;

    // Start is called before the first frame update
    protected override void Start()
    {
        spriteController = GetComponentInChildren<SpriteController>();
        healthController = GetComponent<HealthController>();
        playersManager = FindObjectOfType<PlayersManager>();
        characterController = GetComponent<CharacterController>();
        levelManager = FindObjectOfType<LevelManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        //StartCoroutine(WaitAndShoot());
        currentBehaviours = behaviours;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //
        float dt = Time.deltaTime;
        UpdateBehaviour(dt);
        // Controlamos la muerte aqui
        if (healthController.CurrentHealth == 0 && !isDead)
        {
            EnemyDeath();
        }
    }

    protected void LateUpdate()
    {
        if (movementStatus == MovementStatus.ByAbility) return;
        // Chequeo extra por el jodido boss
        //if (transform.position.y > 0)
        if (!characterController.isGrounded)
            //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            characterController.Move(Vector3.down);
    }

    protected void UpdateBehaviour(float dt)
    {
        // Si está burbujeado que no se mueva
        if (healthController.alteredStates.Count > 0 &&
            (healthController.alteredStates.FindAll(item => item.type == AlteredState.Type.Bubbled).Count > 0
            || healthController.alteredStates.FindAll(item => item.type == AlteredState.Type.Paralized).Count > 0)
            )
            return;
        // TODO: Revisar condición
        if (playersManager.PlayerControllerList.Count == 0 || playersManager.AlivePlayers() == 0) return;
        //
        if (movementStatus == MovementStatus.ByAbility) return;
        //
        bool actionExecuted = false;
        //
        spriteController.status = SpriteController.Status.Idle;
        //
        int playerIndex = playersManager.GetNearestPlayerIndex(transform.position);
        Vector3 playerDirAndDis = playersManager.PlayerControllerList[playerIndex].transform.position - transform.position;
        //
        //if (transform.position.y > 0)
        //    playerDirAndDis.y = -1;
        //else
            playerDirAndDis.y = 0;
        //
        for (int i = 0; i < currentBehaviours.Length; i++)
        {
            actionExecuted = false;
            //
            switch (currentBehaviours[i].type)
            {
                //
                case Behaviour.Type.Attack:
                    if (currentBehaviours[i].minDistance > playerDirAndDis.magnitude)
                    {
                        currentBehaviours[i].cooldown += dt;
                        if(currentBehaviours[i].cooldown >= currentBehaviours[i].rateOfFire)
                        {
                            //Instantiate(proyectilePrefab, transform.position, Quaternion.LookRotation(playerDirAndDis));
                            Shoot(playerDirAndDis, currentBehaviours[i]);
                            currentBehaviours[i].cooldown = 0;
                            actionExecuted = true;
                        }
                    }
                    break;
                //
                case Behaviour.Type.GetAwayFromPlayer:
                    if (currentBehaviours[i].minDistance > playerDirAndDis.magnitude)
                    {
                        //transform.position -= playerDirAndDis.normalized * dt * currentBehaviours[i].amount;
                        characterController.Move(-playerDirAndDis.normalized * dt * currentBehaviours[i].amount);
                        spriteController.status = SpriteController.Status.Moving;
                        actionExecuted = true;
                    }
                    break;
                //
                case Behaviour.Type.EncirclePlayer:
                    if (currentBehaviours[i].minDistance > playerDirAndDis.magnitude)
                    {
                        //Vector3 newDirection = new Vector3(playerDirAndDis.normalized.z, playerDirAndDis.normalized.y, playerDirAndDis.normalized.x);
                        Vector3 newDirection = Quaternion.AngleAxis(currentBehaviours[i].angle, Vector3.up) * playerDirAndDis;
                        characterController.Move(newDirection.normalized * dt * currentBehaviours[i].amount);
                        spriteController.status = SpriteController.Status.Moving;
                        actionExecuted = true;
                    }
                    break;
                //
                case Behaviour.Type.ApproachPlayer:
                    if(currentBehaviours[i].minDistance == 0 || currentBehaviours[i].minDistance > playerDirAndDis.magnitude)
                    {
                        //transform.position += playerDirAndDis.normalized * dt * currentBehaviours[i].amount;
                        characterController.Move(playerDirAndDis.normalized * dt * currentBehaviours[i].amount);
                        spriteController.status = SpriteController.Status.Moving;
                        actionExecuted = true;
                    }                    
                    break;
                // TODO: Probablemente sustituya al ataque
                case Behaviour.Type.UseAbility:
                    if (currentBehaviours[i].minDistance > playerDirAndDis.magnitude)
                    {
                        //attackCooldown += dt;
                        currentBehaviours[i].cooldown += dt;
                        if (currentBehaviours[i].cooldown >= currentBehaviours[i].rateOfFire)
                        {
                            //Debug.Log(gameObject.name + " using ability " + currentBehaviours[i].prefab.name);
                            //Instantiate(proyectilePrefab, transform.position, Quaternion.LookRotation(playerDirAndDis));
                            UseAbility(playerDirAndDis, currentBehaviours[i]);
                            currentBehaviours[i].cooldown = 0;
                            actionExecuted = true;
                        }
                    }
                    break;
            }
            //
            if (actionExecuted == true && currentBehaviours[i].stopHere)
                return;
        }
    }

    protected void Shoot(Vector3 playerDirAndDis, Behaviour behaviour)
    {
        //
        if (!proyectilePrefab)
            return;

        //Vector3 playerDirection = playersManager.PlayerControllerList[0].transform.position - transform.position;
        for(int i = 0; i < behaviour.amount; i++)
        {
            GameObject newProyectile;
            if(behaviour.prefab)
                newProyectile = Instantiate(behaviour.prefab, transform.position, Quaternion.LookRotation(playerDirAndDis));
            else
                newProyectile = Instantiate(proyectilePrefab, transform.position, Quaternion.LookRotation(playerDirAndDis));

            newProyectile.transform.Rotate(Vector3.up * (behaviour.angle * i - 
                (behaviour.angle * (behaviour.amount - 1) / 2) ) );
            newProyectile.transform.eulerAngles = new Vector3(0, newProyectile.transform.eulerAngles.y, 0);
            // Le asginamos la layer de proyectil enemigo
            newProyectile.layer = 11;
            //
            BaseAbility baseAbility = newProyectile.GetComponent<BaseAbility>();
            if (baseAbility)
            {
                baseAbility.owner = Owner.Enemy;
                baseAbility.user = this;
            }
            //
            Debug.Log(name + " shot " + newProyectile.name);
        }        
    }

    protected void UseAbility(Vector3 playerDirAndDis, Behaviour behaviour)
    {
        //
        if (!behaviour.prefab)
            return;

        //Vector3 playerDirection = playersManager.PlayerControllerList[0].transform.position - transform.position;
        GameObject spawnedAbility = Instantiate(behaviour.prefab, transform.position, Quaternion.LookRotation(playerDirAndDis));
        // Le asginamos la layer de proyectil enemigo
        spawnedAbility.layer = 11;
        //
        BaseAbility baseAbility = spawnedAbility.GetComponent<BaseAbility>();
        if (baseAbility)
        {
            baseAbility.owner = Owner.Enemy;
            baseAbility.user = this;
            //baseAbility.
            // De momento mitad de daño base
            baseAbility.attackMultiplier = 0.5f;
        }                
    }

    //IEnumerator WaitAndShoot()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(rateOfFire);
    //        Shoot();
    //    }
    //}

    protected void EnemyDeath()
    {
        isDead = true;
        //
        spriteController.status = SpriteController.Status.Dead;
        //
        GameObject deathSprite = Instantiate(spriteController.gameObject, transform.position, Quaternion.identity);
        //deathSprite.transform.localScale = new Vector3(deathSprite.transform.localScale.x / 5, deathSprite.transform.localScale.y, deathSprite.transform.localScale.z);
        deathSprite.transform.Rotate(Vector3.forward, 90);
        deathSprite.transform.Rotate(Vector3.up, 90);
        deathSprite.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
        deathSprite.name = gameObject.name + "_Dead";
        //
        levelManager.NewDeath(score);
        //
        if (healthController.deathClip)
            AudioManager.Instance.PlaySound(healthController.deathClip);
        //
        enemiesManager.enemyControllers.Remove(this);
        // TODO: Hcaer el pigtaur como boss
        //
        Destroy(gameObject);
    }

}

[Serializable]
public class Behaviour
{
    public enum Type
    {
        ApproachPlayer,
        Attack,
        GetAwayFromPlayer,
        EncirclePlayer,
        UseAbility,

        Count
    }

    [SerializeField] public Type type;
    public float minDistance = 10;
    public float amount = 1;
    public float rateOfFire = 1;
    public float angle = 0;
    public bool stopHere = true;
    public GameObject prefab;

    [HideInInspector] public float cooldown = 0;
}