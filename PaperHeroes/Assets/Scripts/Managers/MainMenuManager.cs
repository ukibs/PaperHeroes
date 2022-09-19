using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Groups")]
    public GameObject mainGroup;
    public GameObject modsGroup;
    public GameObject settingGroup;
    public RectTransform selectedOptionMarker;
    [Header("Buttons")]
    public RectTransform[] mainButtons;
    public RectTransform[] modButtons;
    public RectTransform[] settingButtons;

    private int menuIndex = 0;
    private int buttonIndex = 0;

    private bool stickMovementAllowed = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Que haya un gamepad presente para esto
        if(Gamepad.all.Count > 0)
        {
            if (Gamepad.all[0].aButton.wasPressedThisFrame)
            {
                SelectOption();
            }
            //
            Vector2 move = Gamepad.all[0].leftStick.ReadValue();
            // Desplazamiento
            if (move.y > 0 && stickMovementAllowed)
            {
                UpdateMenuSelection(-1);
            }
            else if (move.y < 0 && stickMovementAllowed)
            {
                UpdateMenuSelection(1);
            }
            else if (move.y == 0)
            {
                stickMovementAllowed = true;
            }
        }
    }

    #region Mods Mode Methods

    public void GoToNormalMode()
    {
        //
        GameManager.Instance.modsMode = false;
        //
        SceneManager.LoadScene("Character Selection");
    }

    public void SelectSettingAndGoToNormalMode()
    {
        //
        GameManager.Instance.modsMode = false;
        //
        GameManager.Instance.currentSetting = GameManager.Instance.availableSettings[buttonIndex];
        //
        SceneManager.LoadScene("Character Selection");
    }

    public void GoToSettingsButtons()
    {
        //
        mainGroup.SetActive(false);
        settingGroup.SetActive(true);
        menuIndex = 2;
    }

    public void GoToModsMode()
    {
        if(!Directory.Exists(Application.persistentDataPath  + "/Mods"))
        {
            PrepareModsMode();
        }
        ////
        //string path = Application.persistentDataPath + "/Mods/";
        //Debug.Log(path);
        //System.Diagnostics.Process.Start(@path);

        ////
        //GameManager.Instance.modsMode = true;

        //
        mainGroup.SetActive(false);
        modsGroup.SetActive(true);
        menuIndex = 1;
    }

    void PrepareModsMode()
    {
        // Creamos la carpeta principal
        Directory.CreateDirectory(Application.persistentDataPath + "/Mods");
        // La de personajes ---------------------------------------------------------------------
        Directory.CreateDirectory(Application.persistentDataPath + "/Mods/Characters");
        // Plantilla de perosnaje
        // Imagen
        File.Copy(Application.streamingAssetsPath + "/Images/Characters/CharTemplate.png", Application.persistentDataPath + "/Mods/Characters" + "/CharTemplate.png");
        //
        CharacterData characterData = new CharacterData();
        characterData.skinName = "CharTemplate.png";
        string jsonClass = JsonUtility.ToJson(characterData);
        FileStream fileStream = File.Create(Application.persistentDataPath + "/Mods/Characters/CharTemplate.json");
        fileStream.Close();
        StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "/Mods/Characters/CharTemplate.json");
        streamWriter.WriteLine(jsonClass);
        streamWriter.Close();
        //
        //System.Diagnostics.Process.Start("explorer.exe", @"/select,"+ Application.persistentDataPath + "/Mods");
    }

    //void GenerateCharactersFromModFolder()
    //{
    //    //
    //    try
    //    {
    //        string mainPath = Application.persistentDataPath;
    //        string characterPath = "/Mods/Characters";
    //        string[] characterFiles = Directory.GetFiles(mainPath + characterPath, "*.json");
    //        //Debug.Log(characterFiles.Length);
    //        //
    //        for (int i = 0; i < characterFiles.Length; i++)
    //        {
    //            string jsonString = File.ReadAllText(characterFiles[i]);
    //            //
    //            //Debug.Log(jsonString);
    //            CharacterData newcharacterData = JsonUtility.FromJson<CharacterData>(jsonString);
    //            //Debug.Log("Json serialized");
    //            //
    //            byte[] fileData = File.ReadAllBytes(mainPath + characterPath + "/" + newcharacterData.skinName);
    //            Texture2D skin = new Texture2D(2, 2);
    //            skin.LoadImage(fileData);
    //            newcharacterData.skin = skin;

    //            CharacterDataScriptableObject characterDataScriptableObject = new CharacterDataScriptableObject(newcharacterData);
    //            //characterTemplates.Add(characterDataScriptableObject);
    //            //Debug.Log(newcharacterData.name);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Character parsing error: " + e);
    //    }
    //}

    public void OpenModsFolder()
    {
        //
        string path = Application.persistentDataPath + "/Mods/";
        Debug.Log(path);
        System.Diagnostics.Process.Start(@path);
    }

    public void StartModsMode()
    {
        GameManager.Instance.modsMode = true;
        SceneManager.LoadScene("Character Selection");
    }

    #endregion

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ReturnToMainGroup()
    {
        mainGroup.SetActive(true);
        modsGroup.SetActive(false);
        menuIndex = 0;
    }

    void UpdateMenuSelection(int direction)
    {
        //
        stickMovementAllowed = false;
        //
        buttonIndex += direction;
        if (buttonIndex < 0)
            buttonIndex = 2;
        if (buttonIndex > 2)
            buttonIndex = 0;
        //
        switch (menuIndex)
        {
            //
            case 0:
                selectedOptionMarker.anchoredPosition = mainButtons[buttonIndex].anchoredPosition;
                break;
            //
            case 1:
                selectedOptionMarker.anchoredPosition = modButtons[buttonIndex].anchoredPosition;
                break;
            //
            case 2:
                selectedOptionMarker.anchoredPosition = settingButtons[buttonIndex].anchoredPosition;
                break;
        }
    }

    void SelectOption()
    {
        switch (menuIndex)
        {
            // Main menu
            case 0:
                switch (buttonIndex)
                {
                    case 0: GoToSettingsButtons(); break;
                    case 1: GoToModsMode(); break;
                    case 2: QuitApplication(); break;
                }
                break;
            // Mod menu
            case 1:
                switch (buttonIndex)
                {
                    case 0: StartModsMode(); break;
                    case 1: OpenModsFolder(); break;
                    case 2: ReturnToMainGroup(); break;
                }
                break;
            // Setting selection
            case 2:
                switch (buttonIndex)
                {
                    case 0: SelectSettingAndGoToNormalMode(); break;
                    case 1: SelectSettingAndGoToNormalMode(); break;
                    case 2: ReturnToMainGroup(); break;
                }
                break;
        }
    }
}
