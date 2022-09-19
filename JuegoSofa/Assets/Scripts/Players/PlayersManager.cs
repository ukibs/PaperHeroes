using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour
{
    //
    public GameObject playerPrefab;
    public GameObject[] playerPrefabs;
    public int maxPlayers = 4;
    public PlayerHUD[] playerHUDs;
    public GameObject noGamepadsPanel;

    //
    //private int currentPlayers = 0;
    private List<PlayerController> playerControllers;
    private bool noGamepads = false;
    private GameManager gameManager;

    private static PlayersManager instance;

    public static PlayersManager Instance { get { return instance; } }

    public List<PlayerController> PlayerControllerList { 
        get { 
            return playerControllers; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        //
        instance = this;
        //
        gameManager = FindObjectOfType<GameManager>();
        //
        playerControllers = new List<PlayerController>(maxPlayers);
        //
        for (int i = 0; i < playerHUDs.Length; i++)
        {
            playerHUDs[i].gameObject.SetActive(false);
        }
        //
        InitiatePlayers();
        
    }

    private void Update()
    {
        if (noGamepads)
        {
            if(Gamepad.all.Count > 0)
            {
                if (Gamepad.all[0].aButton.IsPressed())
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if(Gamepad.all.Count > playerControllers.Count)
        //{
        //    PlayerController playerController = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        //    playerController.controllerIndex = playerControllers.Count;
        //    playerController.playerHUD = playerHUDs[playerControllers.Count];
        //    playerHUDs[playerControllers.Count].gameObject.SetActive(true);
        //    playerController.InitiateHUD();
        //    //currentPlayers++;
        //    playerControllers.Add(playerController);
        //}
        //else if(Gamepad.all.Count < playerControllers.Count)
        //{
        //    PlayerController playerToRemove = playerControllers[playerControllers.Count - 1];
        //    playerHUDs[playerControllers.Count - 1].gameObject.SetActive(false);
        //    playerControllers.Remove(playerToRemove);
        //    Destroy(playerToRemove.gameObject);
        //    //currentPlayers--;
        //}
    }

    public void InitiatePlayers()
    {
        //
        if (Gamepad.all.Count > 0)
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                PlayerController playerController = Instantiate(playerPrefabs[i], new Vector3(0,0,-15), Quaternion.identity).GetComponent<PlayerController>();
                //PlayerController playerController = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
                playerController.controllerIndex = playerControllers.Count;
                //
                if (gameManager)
                {
                    // Habilidades
                    playerController.abilities = gameManager.playerDatas[i].abilitiesDatas.ToArray();
                    // Skin
                    MeshRenderer meshRenderer = playerController.GetComponentInChildren<MeshRenderer>();
                    // Material para que cada prefab tenga el suyo
                    Material newMaterial = new Material(meshRenderer.material);
                    meshRenderer.material = newMaterial;
                    meshRenderer.material.mainTexture = gameManager.playerDatas[i].characterData.skin;
                    // Icono
                    playerHUDs[playerControllers.Count].playerIcon.texture = gameManager.playerDatas[i].characterData.skin;
                    // Stats
                    playerController.attackMultiplier = gameManager.playerDatas[i].characterData.attackMultiplier;
                    playerController.speedMultiplier = gameManager.playerDatas[i].characterData.speedMultiplier;
                    // Vida
                    HealthController healthController = playerController.GetComponent<HealthController>();
                    healthController.maxHealth = gameManager.playerDatas[i].characterData.health;
                }
                // TODO: Coger transform del hijo para cambio de tamaño
                //
                SpriteRenderer flechaSprite = playerController.flecha.GetComponentInChildren<SpriteRenderer>();
                switch (i)
                {
                    case 0: flechaSprite.color = Color.blue; break;
                    case 1: flechaSprite.color = Color.red; break;
                    case 2: flechaSprite.color = Color.green; break;
                    case 3: flechaSprite.color = Color.yellow; break;
                }
                //
                SpriteRenderer circuloSprite = playerController.circuloMarcador.GetComponentInChildren<SpriteRenderer>();
                switch (i)
                {
                    case 0: circuloSprite.color = Color.blue; break;
                    case 1: circuloSprite.color = Color.red; break;
                    case 2: circuloSprite.color = Color.green; break;
                    case 3: circuloSprite.color = Color.yellow; break;
                }
                //
                playerController.playerHUD = playerHUDs[playerControllers.Count];
                playerHUDs[playerControllers.Count].gameObject.SetActive(true);
                playerController.InitiateHUD();
                //currentPlayers++;
                playerControllers.Add(playerController);
            }
        }
        else
        {
            noGamepadsPanel.SetActive(true);
            noGamepads = true;
        }
    }

    public int AlivePlayers()
    {
        int alivePlayers = 0;
        for(int i = 0; i < playerControllers.Count; i++)
        {
            if (!playerControllers[i].isDead)
                alivePlayers++;
        }
        return alivePlayers;
    }

    public int GetNearestPlayerIndex(Vector3 position)
    {
        // Si solo es uno ya
        if (playerControllers.Count == 1)
            return 0;
        //
        int nearestPlayerIndex = -1;
        float nearestPlayerDistance = Mathf.Infinity;
        for (int i = 0; i < playerControllers.Count; i++)
        {
            //
            if (playerControllers[i].isDead)
                continue;
            // Chequeo de player mas cernacon
            float nextPlayerDistance = (playerControllers[i].transform.position - position).sqrMagnitude;
            //
            if (nextPlayerDistance < nearestPlayerDistance)
            {
                nearestPlayerIndex = i;
                nearestPlayerDistance = nextPlayerDistance;
            }
        }
        return nearestPlayerIndex;
    }
}
