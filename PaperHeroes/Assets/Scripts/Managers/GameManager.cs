using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public PlayerData[] playerDatas;
    public List<AbilityScriptableObject> availableAbilities;
    public bool modsMode = false;
    public SettingSO[] availableSettings;

    // IMPORTANTE: Servirá para guardar diferentes partidas/settings
    [HideInInspector] public SettingSO currentSetting;

    [HideInInspector] public LevelEnemiesScriptableObject currentLevelData;
    [HideInInspector] public AbilityScriptableObject rewardAbility;
    [HideInInspector] public string currentLevelName;

    [HideInInspector] public bool levelCompleted = false;

    [HideInInspector] public TerrainSetScriptableObject currentTerrainSet;

    private Dictionary<string,int> scores;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            playerDatas = new PlayerData[4];
            scores = new Dictionary<string, int>(10);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public AbilityScriptableObject CheckReward()
    {
        if(levelCompleted && rewardAbility != null && !availableAbilities.Contains(rewardAbility))
        {
            levelCompleted = false;
            return rewardAbility;
        }

        return null;
    }

    public int GetMaxScore(LevelIndicator levelIndicator)
    {
        if (scores.ContainsKey(levelIndicator.levelData.name))
        {
            return scores[levelIndicator.levelData.name];
        }
        else
        {
            scores.Add(levelIndicator.levelData.name, 0);
        }
        return 0;
    }

    public void SetMaxScore(string levelName, int newScore)
    {
        if (scores[levelName] < newScore)
            scores[levelName] = newScore;
    }

    public void SetMaxScore(int newScore)
    {
        if (scores[currentLevelName] < newScore)
            scores[currentLevelName] = newScore;
    }
}

public class PlayerData
{
    public CharacterDataScriptableObject characterData;
    public List<AbilityScriptableObject> abilitiesDatas;
    public int characterIndex;  // Para el selector de personajes

    public PlayerData(CharacterDataScriptableObject characterData, List<AbilityScriptableObject> abilitiesDatas, int characterIndex)
    {
        this.characterData = characterData;
        this.abilitiesDatas = abilitiesDatas;
        this.characterIndex = characterIndex;
    }
}