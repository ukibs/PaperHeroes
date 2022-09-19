using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterDataScriptableObject : ScriptableObject
{
    public Texture2D skin = null;
    public int health = 100;
    public float attackMultiplier = 1;
    public float speedMultiplier = 1;
    public string skinName = "";        // Este para archivos cargados de mods

    public CharacterDataScriptableObject(CharacterData characterData)   // TODO: Revisar queja de Unity
    {
        this.skin = characterData.skin;
        this.health = characterData.health;
        this.attackMultiplier = characterData.attackMultiplier;
        this.speedMultiplier = characterData.speedMultiplier;
    }
}

// Variante para cargarlo de mods
public class CharacterData
{
    public Texture2D skin = null;
    public int health = 100;
    public float attackMultiplier = 1;
    public float speedMultiplier = 1;
    public string skinName = "";        // Este para archivos cargados de mods
}