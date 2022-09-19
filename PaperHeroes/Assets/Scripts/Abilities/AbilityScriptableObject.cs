using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData", order = 1)]
public class AbilityScriptableObject : ScriptableObject
{
    public enum Type
    {
        Invalid = -1,

        Spawn,

        Count
    }

    public Type type;
    public GameObject prefab;
    public float cooldown = 0.1f;
    public Texture2D icon;
    public string description = "";
}
