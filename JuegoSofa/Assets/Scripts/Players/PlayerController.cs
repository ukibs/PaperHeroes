using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class PlayerController : BaseController
{
    //
    public enum ArrowStatus
    {
        Manual,
        Locked,

        Count
    }
    //
    public float movementSpeed = 3;
    public Transform flecha;
    public Transform circuloMarcador;
    public GameObject proyectilPrefab;
    //public float fireRate = 0.1f;
    public AbilityScriptableObject[] abilities;
    //
    [HideInInspector] public int controllerIndex;
    [HideInInspector] public PlayerHUD playerHUD;
    //[HideInInspector] public bool isDead = false;
    //
    [HideInInspector] public float attackMultiplier = 1;
    [HideInInspector] public float speedMultiplier = 1;
    //
    //private SpriteController spriteController;
    //private HealthController healthController;
    //private CharacterController characterController;
    //private LevelManager levelManager;
    //private float attackCooldown = 0;
    private float[] abilityCooldowns;
    // Para fijado de objetivo
    private ArrowStatus arrowStatus = ArrowStatus.Locked;
    private Transform lockedObjective;
    private EnemiesManager enemiesManager;
    //
    private Transform arrowTransform;
    private Transform circleTransform;
    private Vector3 arrowTransformInitialScale;
    private Vector3 circleTransformInitialScale;


    // Start is called before the first frame update
    protected override void Start()
    {
        spriteController = GetComponentInChildren<SpriteController>();
        healthController = GetComponent<HealthController>();
        characterController = GetComponent<CharacterController>();
        levelManager = FindObjectOfType<LevelManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        //
        InitiateAbilityCooldowns();
        //
        arrowTransform = flecha.GetChild(0);
        circleTransform = circuloMarcador.GetChild(0);
        arrowTransformInitialScale = arrowTransform.localScale;
        circleTransformInitialScale = circleTransform.localScale;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float dt = Time.deltaTime;

        var gamepad = Gamepad.all[controllerIndex];
        if (gamepad == null)
            return; // No gamepad connected.
        //
        if (!isDead)
        {
            //
            UpdateArrow(dt, gamepad);
            //
            UpdateMovement(dt, gamepad);
            //
            UpdateAbilities(dt, gamepad);
            //
            CheckHealth();
        }        
    }

    private void OnEnable()
    {
        //playerHUD.Initiate(abilities);
    }

    public void InitiateHUD()
    {
        playerHUD.Initiate(abilities);
    }

    void InitiateAbilityCooldowns()
    {
        abilityCooldowns = new float[abilities.Length];
        for(int i = 0; i < abilityCooldowns.Length; i++)
        {
            //
            if(abilities[i] != null)
                abilityCooldowns[i] = abilities[i].cooldown;
            playerHUD.abilityCooldowns[i].fillAmount = 0;
        }
    }

    void UpdateArrow(float dt, Gamepad gamepad)
    {
        //
        Vector2 direction = gamepad.rightStick.ReadValue();
        //
        if(direction.sqrMagnitude > 0.25f && arrowStatus == ArrowStatus.Locked)
        {
            lockedObjective = enemiesManager.GetNearestEnemyTransform(transform.position);
        }
        else if (gamepad.rightStickButton.wasPressedThisFrame)
        {
            if(arrowStatus == ArrowStatus.Locked)
            {
                arrowStatus = ArrowStatus.Manual;
                lockedObjective = null;
                circuloMarcador.gameObject.SetActive(false);
            }
            else
            {
                // TODO: Función para fijar objetivo
                lockedObjective = enemiesManager.GetNearestEnemyTransform(transform.position);
                arrowStatus = ArrowStatus.Locked;
                circuloMarcador.gameObject.SetActive(true);
            }
        }
        //
        if(arrowStatus == ArrowStatus.Locked)
        {
            //
            if(lockedObjective != null)
            {
                circuloMarcador.transform.position = lockedObjective.position;
                // TODO: Revisar como gestionamos esto
                circuloMarcador.transform.position = new Vector3(circuloMarcador.transform.position.x,
                                                            0, circuloMarcador.transform.position.z);
                //
                flecha.LookAt(circuloMarcador);
            }
            else //Que busque otro
            {
                //arrowStatus = ArrowStatus.Manual;
                lockedObjective = enemiesManager.GetNearestEnemyTransform(transform.position);
                if(lockedObjective)
                    circuloMarcador.gameObject.SetActive(true);
                else
                    circuloMarcador.gameObject.SetActive(false);
                //if(lockedObjective == null)
                //{
                //    arrowStatus = ArrowStatus.Manual;
                //    circuloMarcador.gameObject.SetActive(false);
                //}
            }
            //
            arrowTransform.localScale = arrowTransformInitialScale + (arrowTransformInitialScale * 0.1f) * Mathf.Sin((Time.time) * 10);
            circleTransform.localScale = circleTransformInitialScale + (circleTransformInitialScale * 0.1f) * Mathf.Sin((Time.time) * 10);
        }
        else
        {            
            // Control para que no se vaya pa otro lado al soltar
            if (direction.sqrMagnitude > Mathf.Pow(0.2f, 2))
                flecha.LookAt(transform.position + new Vector3(direction.x, 0, direction.y));
        }        
    }

    void UpdateMovement(float dt, Gamepad gamepad)
    {
        // Movimiento
        Vector2 move = gamepad.leftStick.ReadValue();
        //
        if (move.magnitude > 0.1f)
            spriteController.status = SpriteController.Status.Moving;
        else
            spriteController.status = SpriteController.Status.Idle;
        // 'Move' code here
        //transform.Translate(((Vector3.forward * move.y * dt) + (Vector3.right * move.x * dt)) * movementSpeed);
        //characterController.Move(((Vector3.forward * move.y * dt) + (Vector3.right * move.x * dt)) * movementSpeed * speedMultiplier);
        characterController.Move(((CameraPivotController.Instance.transform.forward * move.y * dt) + 
            (CameraPivotController.Instance.transform.right * move.x * dt)) * 
            movementSpeed * speedMultiplier);
    }

    void UpdateAbilities(float dt, Gamepad gamepad)
    {
        //
        for (int i = 0; i < abilities.Length; i++)
        {
            //
            if (abilities[i] == null) continue;
            //
            if (abilityCooldowns[i] < abilities[i].cooldown)
            {
                abilityCooldowns[i] += dt;
                playerHUD.abilityCooldowns[i].fillAmount = 1 - (abilityCooldowns[i] / abilities[i].cooldown);
            }
                
            //
            ButtonControl buttonToCheck = null;
            switch (i)
            {
                case 0: buttonToCheck = gamepad.leftTrigger; break;
                case 1: buttonToCheck = gamepad.rightTrigger; break;
                case 2: buttonToCheck = gamepad.leftShoulder; break;
                case 3: buttonToCheck = gamepad.rightShoulder; break;
            }
            //
            ButtonControl altButtonToCheck = null;
            switch (i)
            {
                case 0: altButtonToCheck = gamepad.yButton; break;
                case 1: altButtonToCheck = gamepad.bButton; break;
                case 2: altButtonToCheck = gamepad.xButton; break;
                case 3: altButtonToCheck = gamepad.aButton; break;
            }
            //
            if ((buttonToCheck.isPressed || altButtonToCheck.isPressed) && abilityCooldowns[i] >= abilities[i].cooldown)
            {
                switch (abilities[i].type)
                {
                    case AbilityScriptableObject.Type.Spawn:
                        GameObject spawnedAbility = Instantiate(abilities[i].prefab, transform.position, flecha.rotation);
                        spawnedAbility.transform.eulerAngles = new Vector3(0, spawnedAbility.transform.eulerAngles.y, 0);
                        //
                        BaseAbility baseAbility = spawnedAbility.GetComponent<BaseAbility>();
                        if (baseAbility)
                        {
                            baseAbility.attackMultiplier = attackMultiplier;
                            baseAbility.owner = Owner.Player;
                            baseAbility.user = this;
                            if(lockedObjective)
                                baseAbility.destination = lockedObjective.position;
                        }
                        else
                        {
                            Debug.LogError("This ability has no base ability attached");
                        }
                        // Le asignamos la layer de player attack
                        spawnedAbility.layer = 10;
                        break;
                }
                //
                abilityCooldowns[i] = 0;
            }
        }
    }

    //void UpdateAttack(float dt, Gamepad gamepad)
    //{
    //    //
    //    if(attackCooldown < fireRate)
    //        attackCooldown += dt;
    //    // Gatillo derecho
    //    //if (gamepad.rightTrigger.wasPressedThisFrame)
    //    if (gamepad.rightTrigger.isPressed && attackCooldown >= fireRate)
    //    {
    //        // 'Use' code here
    //        Instantiate(proyectilPrefab, transform.position, flecha.rotation);
    //        attackCooldown = 0;
    //    }
    //}

    void CheckHealth()
    {
        if (healthController.CurrentHealth == 0)
        {
            Die();
        }
        else if(healthController.CurrentHealth > 0 && isDead)
        {
            Revive();
        }
    }

    void Die()
    {
        isDead = true;
        spriteController.status = SpriteController.Status.Dead;
        //
        //GameObject deathSprite = Instantiate(spriteController.gameObject, transform.position, Quaternion.identity);
        //spriteController.transform.localScale = 
        //    new Vector3(spriteController.transform.localScale.x / 4, 
        //                spriteController.transform.localScale.y, 
        //                spriteController.transform.localScale.z);
        //spriteController.transform.Rotate(Vector3.forward, 90);
        spriteController.transform.Rotate(Vector3.right, 90);
        spriteController.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
        spriteController.transform.localPosition = new Vector3(0, -0.5f, 0);
        //deathSprite.name = gameObject.name + "_Dead";
        //flecha.gameObject.SetActive(false);
        circuloMarcador.gameObject.SetActive(false);
        //
        levelManager.NewDeath();
    }

    void Revive()
    {
        isDead = true;
        spriteController.status = SpriteController.Status.Dead;
        //
        spriteController.transform.Rotate(Vector3.right, -90);
        //spriteController.transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
        spriteController.transform.localPosition = new Vector3(0, 0, 0);
        //deathSprite.name = gameObject.name + "_Dead";
        //flecha.gameObject.SetActive(true);
    }
}
