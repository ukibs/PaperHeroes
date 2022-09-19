using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public List<PlayerPanel> playerPanels;
    public List<CharacterDataScriptableObject> characterTemplates;
    public List<AbilityScriptableObject> availableAbilities;

    //private bool noGamepads = false;

    private int potentialPlayers = 0;

    //private int confirmedPlayers = 0;
    //private Canvas canvas;

    private void Awake()
    {
        //
        if (GameManager.Instance)
        {
            availableAbilities = GameManager.Instance.availableAbilities;
            Debug.Log(GameManager.Instance.currentSetting);
            //
            if (GameManager.Instance.currentSetting)
            {
                characterTemplates = new List<CharacterDataScriptableObject>(GameManager.Instance.currentSetting.characters);
                //availableAbilities = new List<AbilityScriptableObject>(GameManager.Instance.currentSetting.abilities);
            }
        }

        //
        GenerateCharactersFromModFolder();
    }

    // Start is called before the first frame update
    void Start()
    {
        //canvas = FindObjectOfType<Canvas>();
        //playerPanels = new List<GameObject>(4);

        //
        for(int i = 0; i < playerPanels.Count; i++)
        {
            playerPanels[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckReadyPlayers();
        //
        if (Gamepad.all[0].selectButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //
        if(Gamepad.all.Count > potentialPlayers)
        {
            Debug.Log("Adding potential player: " + potentialPlayers + ", gamepads: " + Gamepad.all.Count);
            playerPanels[potentialPlayers].gameObject.SetActive(true);
            potentialPlayers++;
        }
        else if(Gamepad.all.Count < potentialPlayers)
        {
            potentialPlayers--;
            Debug.Log("Removing potential player: " + potentialPlayers + ", gamepads: " + Gamepad.all.Count);
            playerPanels[potentialPlayers].ResetPanel();
            playerPanels[potentialPlayers].gameObject.SetActive(false);
            
        }
    }

    //
    void CheckReadyPlayers()
    {
        //
        if (potentialPlayers == 0)
            return;
        //
        int readyPlayers = 0;
        for(int i = 0; i < potentialPlayers; i++)
        {
            if (playerPanels[i].panelStatus == PlayerPanel.PanelStatus.Ready)
                readyPlayers++;
        }
        //
        if (readyPlayers == potentialPlayers)
            SceneManager.LoadScene("Map");
    }

    void GenerateCharactersFromModFolder()
    {
        //
        if (!GameManager.Instance.modsMode)
            return;
        //
        try
        {
            string mainPath = Application.persistentDataPath;
            string characterPath = "/Mods/Characters";
            string[] characterFiles = Directory.GetFiles(mainPath + characterPath, "*.json");
            //Debug.Log(characterFiles.Length);
            //
            for (int i = 0; i < characterFiles.Length; i++)
            {
                string jsonString = File.ReadAllText(characterFiles[i]);
                //
                //Debug.Log(jsonString);
                CharacterData newcharacterData = JsonUtility.FromJson<CharacterData>(jsonString);
                //Debug.Log("Json serialized");
                //
                byte[] fileData = File.ReadAllBytes(mainPath + characterPath + "/" + newcharacterData.skinName);
                Texture2D skin = new Texture2D(2, 2);
                skin.LoadImage(fileData);
                newcharacterData.skin = skin;

                CharacterDataScriptableObject characterDataScriptableObject = new CharacterDataScriptableObject(newcharacterData);
                characterTemplates.Add(characterDataScriptableObject);
                //Debug.Log(newcharacterData.name);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Character parsing error: " + e);
        }
    }
}
