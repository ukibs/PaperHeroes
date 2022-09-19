using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Setting", menuName = "ScriptableObjects/Setting", order = 1)]
public class SettingSO : ScriptableObject
{
    public CharacterDataScriptableObject[] characters;
    public AbilityScriptableObject[] abilities;
}
