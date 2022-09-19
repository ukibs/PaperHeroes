using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public enum MovementStatus
    {
        Normal,
        ByAbility,

        Count
    }

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public MovementStatus movementStatus = MovementStatus.Normal;

    protected SpriteController spriteController;
    protected HealthController healthController;
    protected CharacterController characterController;
    protected LevelManager levelManager;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

}
