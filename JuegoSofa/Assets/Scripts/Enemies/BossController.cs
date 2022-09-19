using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    public BossPhase[] bossPhases;

    private int currentPhaseIndex = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentPhaseIndex = bossPhases.Length - 1;
        currentBehaviours = bossPhases[currentPhaseIndex].behaviourCollection.behaviours;        
        healthController.NextBossPhase(bossPhases[currentPhaseIndex].health, currentPhaseIndex);
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
            CheckNextPhaseOrDie();
        }
    }

    void CheckNextPhaseOrDie()
    {
        if(currentPhaseIndex > 0)
        {
            //
            if (bossPhases[currentPhaseIndex].abilityOnPhaseEnd)
            {
                int playerIndex = playersManager.GetNearestPlayerIndex(transform.position);
                Vector3 playerDirAndDis = playersManager.PlayerControllerList[playerIndex].transform.position - transform.position;
                playerDirAndDis.y = 0;
                UseEndPhaseAbility(playerDirAndDis, bossPhases[currentPhaseIndex].abilityOnPhaseEnd);
            }
            //
            LevelManager.Instance.UpdateScore(bossPhases[currentPhaseIndex].score);
            //
            currentPhaseIndex--;
            currentBehaviours = bossPhases[currentPhaseIndex].behaviourCollection.behaviours;
            healthController.NextBossPhase(bossPhases[currentPhaseIndex].health, currentPhaseIndex);
            
        }
        else
        {
            EnemyDeath();
            enemiesManager.BossDeath();
        }
    }

    void UseEndPhaseAbility(Vector3 playerDirAndDis, AbilityScriptableObject abilityScriptableObject)
    {
        //Vector3 playerDirection = playersManager.PlayerControllerList[0].transform.position - transform.position;
        GameObject spawnedAbility = Instantiate(abilityScriptableObject.prefab, transform.position, Quaternion.LookRotation(playerDirAndDis));
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

    // Clase para fases de boss ---------------------------------------------------------
    [Serializable]
    public class BossPhase
    {
        public BehaviourCollectionSO behaviourCollection;
        public int health = 1000;
        public AbilityScriptableObject abilityOnPhaseEnd;
        public int score = 500;
    }
}
