using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourCollection", menuName = "ScriptableObjects/BehaviourCollection", order = 1)]
public class BehaviourCollectionSO : ScriptableObject
{
    public Behaviour[] behaviours;
}
