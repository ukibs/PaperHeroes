using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Control en el mapa por parte del jugador
/// </summary>
public class MapController : MonoBehaviour
{
    //
    public enum Status
    {
        Normal,
        ShowingReward,
        Count
    }
    //
    [Header("Parameters")]
    public float speed = 5;
    public GameObject levelInfoPanel;
    public TMP_Text levelNameText;
    public TMP_Text maxScoreText;
    [Header("Reward Objects")]
    public GameObject rewardPanel;
    public RawImage rewardIcon;
    public TMP_Text rewardText;
    [Header("Player Sprites")]
    public SpriteController[] spriteControllers;
    //
    [HideInInspector] public LevelEnemiesScriptableObject currentLevelData;
    //
    private GameManager gameManager;
    private LevelIndicator currentLevelIndicator;
    private Status status = Status.Normal;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager)
        {
            AbilityScriptableObject rewardAbility = gameManager.CheckReward();
            if(rewardAbility != null)
            {
                
                ShowRewardPanel(rewardAbility);
            }
            InitiateSpriteControllers();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
        switch (status)
        {
            case Status.Normal:
                UpdateNormalStatus();
                break;
            case Status.ShowingReward:
                // 
                if (Gamepad.all[0].aButton.wasPressedThisFrame)
                {
                    rewardPanel.SetActive(false);
                    status = Status.Normal;
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        currentLevelIndicator = other.GetComponent<LevelIndicator>();
        if (currentLevelIndicator)
        {
            currentLevelData = currentLevelIndicator.levelData;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentLevelIndicator = other.GetComponent<LevelIndicator>();
        if (currentLevelIndicator)
        {
            currentLevelData = null;
        }
    }

    void UpdateNormalStatus()
    {
        //
        if (currentLevelData)
        {
            levelInfoPanel.SetActive(true);
            levelNameText.text = currentLevelData.name;
            maxScoreText.text = "Max Score: " + currentLevelIndicator.maxScore;
        }
        else
        {
            levelInfoPanel.SetActive(false);
            //levelNameText.text = "";
        }
        //
        if (Gamepad.all.Count > 0)
        {
            // Movimiento
            Vector2 move = Gamepad.all[0].leftStick.ReadValue();
            transform.position += new Vector3(move.x, 0, move.y) * Time.deltaTime * speed;

            //
            if(move.magnitude > 0.1f)
            {
                UpdateSpriteControllers(SpriteController.Status.Moving);
            }
            else
            {
                UpdateSpriteControllers(SpriteController.Status.Idle);
            }

            // 
            if (Gamepad.all[0].aButton.wasPressedThisFrame && currentLevelData)
            {
                if (gameManager)
                {
                    gameManager.currentLevelData = currentLevelData;
                    gameManager.rewardAbility = currentLevelIndicator.rewardAbility;
                    gameManager.currentLevelName = currentLevelIndicator.levelData.name;
                    gameManager.currentTerrainSet = currentLevelIndicator.terrainData;
                }
                //
                SceneManager.LoadScene("PlayLevel");
            }

            //
            if (Gamepad.all[0].startButton.wasPressedThisFrame)
            {
                //
                SceneManager.LoadScene("Character Selection");
            }
        }
    }

    void InitiateSpriteControllers()
    {
        //Debug.Log("Player datas: " + gameManager.playerDatas.Length);
        for (int i = 0; i < spriteControllers.Length; i++)
        {
            if(gameManager.playerDatas[i] != null)
            {
                MeshRenderer meshRenderer = spriteControllers[i].GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = gameManager.playerDatas[i].characterData.skin;
            }
            else
            {
                spriteControllers[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateSpriteControllers(SpriteController.Status status)
    {
        for(int i = 0; i < spriteControllers.Length; i++)
        {
            spriteControllers[i].status = status;
        }
    }

    void ShowRewardPanel(AbilityScriptableObject rewardAbility)
    {
        GameManager.Instance.availableAbilities.Add(rewardAbility);
        status = Status.ShowingReward;
        rewardIcon.texture = rewardAbility.icon;
        rewardText.text = rewardAbility.name + " obtained!";
        rewardPanel.SetActive(true);
    }
}
