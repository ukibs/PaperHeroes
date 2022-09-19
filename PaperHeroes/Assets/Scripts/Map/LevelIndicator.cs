using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIndicator : MonoBehaviour
{
    public LevelEnemiesScriptableObject levelData;
    public AbilityScriptableObject rewardAbility;
    public TerrainSetScriptableObject terrainData;
    //
    public MeshRenderer indicatorMR;
    public Texture2D indicatorIcon;
    //
    [HideInInspector] public int maxScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        indicatorMR.material = new Material(indicatorMR.material);
        indicatorMR.material.mainTexture = indicatorIcon;
        //
        maxScore = GameManager.Instance.GetMaxScore(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
