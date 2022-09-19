using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{
    #region Public Attributes

    [Header("Worlds Template")]
    public SettingSO templateForWorlds;

    [Header("Canvas elements - Worlds")]
    public GameObject worldsGroup;
    public GameObject worldElementsGroup;
    public GameObject newWorldPopup;
    public TMP_InputField worldInputField;
    public GameObject worldReferencePrefab;
    public RectTransform worldReferenceContainer;

    [Header("Canvas elements - World elements")]
    public TMP_Text worldNameTitle;
    public GameObject characterReferencePrefab;
    public RectTransform characterReferenceContainer;

    [Header("Canvas elements - Character edition")]
    public GameObject characterEditionPopup;
    public RawImage characterImage;
    public TMP_InputField characterNameField;
    public TMP_InputField characterHealthField;
    public TMP_InputField characterAttackField;
    public TMP_InputField characterSpeedField;

    #endregion

    #region Private Attributes

    private string currentWorldName = "";

    private string currentCharacterInEditionPath = "";
    private string currentCharacterInEditionImagePath = "";

    #endregion

    // Start is called before the first frame update
    void Start()
    {        
        //
        CheckAndCreateWorldForlder();
        //
        UpdateListedWorlds();
        //
        worldReferencePrefab.SetActive(false);
        characterReferencePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckAndCreateWorldForlder()
    {
        string root = UnityEngine.Application.persistentDataPath;
        if(!Directory.Exists(root + "\\Worlds"))
        {
            Directory.CreateDirectory(root + "\\Worlds");
        }
    }

    void UpdateListedWorlds()
    {
        // Sacamos los mundos que pueda haber creados
        string root = UnityEngine.Application.persistentDataPath;
        string[] subdirectoryEntries = Directory.GetDirectories(root + "\\Worlds");
        //
        for(int i = 0; i < subdirectoryEntries.Length; i++)
        {
            Debug.Log(subdirectoryEntries[i]);
            //
            string[] splittedEntry = subdirectoryEntries[i].Split('\\');
            string worldName = splittedEntry[splittedEntry.Length - 1];
            Debug.Log(worldName);
            //
            GameObject worldReference = Instantiate(worldReferencePrefab, worldReferenceContainer);
            RectTransform wrrt = worldReference.GetComponent<RectTransform>();
            wrrt.anchoredPosition = new Vector2(0, 200 * i);
            worldReference.SetActive(true);
            //
            UnityEngine.UI.Button[] worldButtons = worldReference.GetComponentsInChildren<UnityEngine.UI.Button>();
            worldButtons[0].onClick.AddListener(delegate { SelectWorld(worldName); });
            //
            TMP_Text worldNameField = worldReference.GetComponentInChildren<TMP_Text>();
            worldNameField.text = worldName;
        }
    }

    //
    public void CreateWorld()
    {
        //
        string newWorldName = worldInputField.text;
        // TODO: Validación mejor hecha
        if (newWorldName.Equals("")) return;
        // Creamos directorio de mundo
        string root = UnityEngine.Application.persistentDataPath;
        if (!Directory.Exists(root + "\\Worlds\\" + newWorldName))
        {
            // Creamos el mundo
            Directory.CreateDirectory(root + "\\Worlds\\" + newWorldName);
            // Los personajes
            Directory.CreateDirectory(root + "\\Worlds\\" + newWorldName + "\\Characters");
            //
            for(int i = 0; i < templateForWorlds.characters.Length; i++)
            {
                // Guardamos la imagen del personaje
                byte[] bytes = templateForWorlds.characters[i].skin.EncodeToPNG();
                File.WriteAllBytes(root + "\\Worlds\\" + newWorldName + "\\Characters\\" + templateForWorlds.characters[i].skin.name + ".png", bytes);
                templateForWorlds.characters[i].skinName = templateForWorlds.characters[i].skin.name + ".png";
                // Y el archivo con los atributos en json
                FileStream characterFile = File.Create(root + "\\Worlds\\" + newWorldName + "\\Characters\\" + templateForWorlds.characters[i].name + ".json");
                string characterData = JsonUtility.ToJson(templateForWorlds.characters[i]);
                bytes = Encoding.UTF8.GetBytes(characterData);
                characterFile.Write(bytes, 0, bytes.Length);
                characterFile.Close();
            }
        }
        else
        {
            Debug.Log("World already exists");
        }

        // Y mas cosas

        //
        newWorldPopup.SetActive(false);
        //
        UpdateListedWorlds();
    }

    #region Navigation Methods

    public void SelectWorld(string worldName)
    {
        currentWorldName = worldName;
        worldNameTitle.text = currentWorldName;
        worldElementsGroup.SetActive(true);
        worldsGroup.SetActive(false);
        //
        UpdateCharacterReferences();
    }

    public void ReturnToWorldsScreen()
    {
        worldElementsGroup.SetActive(false);
        worldsGroup.SetActive(true);
        currentWorldName = "";
    }

    #endregion

    #region Character Methods

    void UpdateCharacterReferences()
    {
        // Sacamos los mundos que pueda haber creados
        string root = UnityEngine.Application.persistentDataPath + "\\Worlds" + "\\" + currentWorldName + "\\Characters";
        //string[] subdirectoryEntries = Directory.GetDirectories(root + "\\Worlds");
        DirectoryInfo d = new DirectoryInfo(root); //Assuming Test is your Folder
        FileInfo[] files = d.GetFiles("*.json"); //Getting Text files

        //string mainPath = Application.persistentDataPath;
        //string characterPath = "/Mods/Characters";
        string[] characterFiles = Directory.GetFiles(root, "*.json");

        for (int i = 0; i < characterFiles.Length; i++)
        {
            //
            Debug.Log(characterFiles[i]);
            string jsonString = File.ReadAllText(characterFiles[i]);
            CharacterData newcharacterData = JsonUtility.FromJson<CharacterData>(jsonString);
            //
            GameObject characterReference = Instantiate(characterReferencePrefab, characterReferenceContainer);
            RectTransform crrt = characterReference.GetComponent<RectTransform>();
            crrt.anchoredPosition = new Vector2(0, 200 * -i);
            characterReference.SetActive(true);
            //
            UnityEngine.UI.Button[] worldButtons = characterReference.GetComponentsInChildren<UnityEngine.UI.Button>();
            // Boton de guardar
            int currentIndex = i;
            worldButtons[0].onClick.AddListener(delegate { LoadCharacterData(characterFiles[currentIndex]); });
            //
            TMP_Text worldNameField = characterReference.GetComponentInChildren<TMP_Text>();
            string[] slicedPath = characterFiles[i].Split('\\');
            string characterName = slicedPath[slicedPath.Length - 1].Split('.')[0];
            worldNameField.text = characterName;
        }
    }

    public void LoadCharacterData(string path)
    {
        //
        currentCharacterInEditionPath = path;        
        //
        string jsonString = File.ReadAllText(path);
        CharacterData newcharacterData = JsonUtility.FromJson<CharacterData>(jsonString);
        //
        string[] slicedPath = path.Split('\\');
        string characterName = slicedPath[slicedPath.Length - 1].Split('.')[0];
        //
        characterNameField.text = characterName;
        characterHealthField.text = newcharacterData.health.ToString();
        characterAttackField.text = newcharacterData.attackMultiplier.ToString();
        characterSpeedField.text = newcharacterData.speedMultiplier.ToString();
        //
        slicedPath[slicedPath.Length - 1] = newcharacterData.skinName;
        string convergedPath = string.Join("\\", slicedPath);
        // Guardamos ruta de la imagen para por si la cambiamos por otra
        currentCharacterInEditionImagePath = convergedPath;
        byte[] fileData = File.ReadAllBytes(convergedPath);
        Texture2D skin = new Texture2D(2, 2);
        skin.LoadImage(fileData);
        characterImage.texture = skin;
        //
        characterEditionPopup.SetActive(true);
    }

    public void SelectImageToSet()
    {
        string[] allowedFiles = new string[1];
        allowedFiles[0] = "image/png";
        NativeFilePicker.PickFile(ChangeCharacterImage, allowedFiles);
    }

    public void ChangeCharacterImage(string path)
    {
        Debug.Log(path);
        string root = UnityEngine.Application.persistentDataPath + "\\Worlds" + "\\" + currentWorldName + "\\Characters";
        //
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D skin = new Texture2D(2, 2);
        skin.LoadImage(fileData);
        characterImage.texture = skin;
        //
        //File.Copy(path, root + "\\ImageName");
    }

    public void SaveCharacter()
    {
        //
        try
        {
            //
            CharacterData characterData = new CharacterData();
            characterData.health = int.Parse(characterHealthField.text);
            characterData.attackMultiplier = float.Parse(characterAttackField.text);
            characterData.speedMultiplier = float.Parse(characterSpeedField.text);
            //
            string[] slicedPath = currentCharacterInEditionPath.Split('\\');
            string characterName = slicedPath[slicedPath.Length - 1].Split('.')[0];
            // Si no conidne el nombre
            if (!characterName.Equals(characterNameField.text))
            {
                // Nos cargamos el anterior fichero
                File.Delete(currentCharacterInEditionPath);
                // Y creamos el nuevo

            }
            //
            // Guardamos la imagen del personaje
            //byte[] bytes = templateForWorlds.characters[i].skin.EncodeToPNG();
            //File.WriteAllBytes(root + "\\Worlds\\" + newWorldName + "\\Characters\\" + templateForWorlds.characters[i].skin.name + ".png", bytes);
            //templateForWorlds.characters[i].skinName = templateForWorlds.characters[i].skin.name + ".png";
            //// Y el archivo con los atributos en json
            //FileStream characterFile = File.Create(root + "\\Worlds\\" + newWorldName + "\\Characters\\" + templateForWorlds.characters[i].name + ".json");
            //string characterData = JsonUtility.ToJson(templateForWorlds.characters[i]);
            //bytes = Encoding.UTF8.GetBytes(characterData);
            //characterFile.Write(bytes, 0, bytes.Length);
            //characterFile.Close();
            //
            characterEditionPopup.SetActive(false);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }        
    }

    #endregion
}
