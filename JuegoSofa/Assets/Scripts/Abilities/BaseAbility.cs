using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para meter cosas comunes a tpdas las habilidades
/// </summary>
public class BaseAbility : MonoBehaviour
{
    // Esto en rpincipio le llegará solo desde los jugadores
    // De momento
    [HideInInspector] public float attackMultiplier = 1f;
    [HideInInspector] public Owner owner;
    [HideInInspector] public BaseController user;
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public Transform objective;

    //
    public void GetDataFromPreviousAbility(BaseAbility previousAbility)
    {
        Debug.Log(previousAbility.owner + ", " + previousAbility.user);
        attackMultiplier = previousAbility.attackMultiplier;
        owner = previousAbility.owner;
        user = previousAbility.user;
        destination = previousAbility.destination;
        objective = previousAbility.objective;
        gameObject.layer = previousAbility.gameObject.layer;
    }
}
