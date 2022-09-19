using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public enum PanelStatus
    {
        PendingConfirmation,
        Active,
        Ready,

        Count
    }

    public enum AbilitySelectionStatus
    {
        Hidden,
        ShowingAbilities
    }

    public int playerIndex = 0;
    public GameObject confirmationPanel;
    public GameObject activeGroup;

    public RawImage skin;
    public TMP_Text healthText;
    public TMP_Text attackMultiplierText;
    public TMP_Text speedMultiplierText;

    public List<RawImage> abilityIcons;
    public List<GameObject> abilityIconBackgrounds;

    public GameObject availableAbilitiesGroup;
    public RectTransform availableAbilitiesContent;
    public GameObject abilityOptionPrefab;
    public RectTransform selectionIndicator;

    public TMP_Text cooldownText;
    public TMP_Text descriptionText;

    public TMP_Text confirmationText;
    public TMP_Text readyText;

    [HideInInspector] public PanelStatus panelStatus;
    private AbilitySelectionStatus abilitySelectionStatus;

    private bool characterChangeAllowed = true;
    private CharacterSelector characterSelector;
    private int currentCharacterIndex = 0;
    private int abilityToChangeIndex = -1;
    private int abilitySelectionIndex = -1;
    private int[] selectedAbilitiesIndexes;
    private bool abilityChangeAllowed = true;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        characterSelector = FindObjectOfType<CharacterSelector>();
        activeGroup.SetActive(false);
        UpdateCharacter(0);
    }

    private void OnEnable()
    {
        characterSelector = FindObjectOfType<CharacterSelector>();
        ResetPanel();
        InitializeCharacter();
        InitializeAbilityIcons();
        InitializeAvailableAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        switch (panelStatus)
        {
            case PanelStatus.PendingConfirmation:
                if (Gamepad.all[playerIndex].startButton.wasPressedThisFrame)
                {
                    confirmationPanel.SetActive(false);
                    panelStatus = PanelStatus.Active;
                    activeGroup.SetActive(true);
                }
                break;
            case PanelStatus.Active:
                UpdateCharacterOptions();
                if (Gamepad.all[playerIndex].startButton.wasPressedThisFrame && abilitySelectionStatus == AbilitySelectionStatus.Hidden)
                {
                    ConfirmCharacterSelection();
                    panelStatus = PanelStatus.Ready;
                    confirmationText.gameObject.SetActive(false);
                    readyText.gameObject.SetActive(true);
                }
                break;
            case PanelStatus.Ready:
                if (Gamepad.all[playerIndex].startButton.wasPressedThisFrame)
                {
                    panelStatus = PanelStatus.Active;
                    confirmationText.gameObject.SetActive(true);
                    readyText.gameObject.SetActive(false);
                }
                break;
        }        
    }

    void UpdateCharacterOptions()
    {
        //
        Vector2 move = Gamepad.all[playerIndex].leftStick.ReadValue();
        //
        if (abilitySelectionStatus == AbilitySelectionStatus.Hidden)
        {
            // Seleccion de personaje
            
            if (move.x < 0 && characterChangeAllowed)
            {
                UpdateCharacter(-1);
            }
            else if (move.x > 0 && characterChangeAllowed)
            {
                UpdateCharacter(1);
            }
            else if (move.x == 0)
            {
                characterChangeAllowed = true;
            }

            // Selección de habilidades
            CheckAbilityButton(Gamepad.all[playerIndex].leftTrigger, 0);
            CheckAbilityButton(Gamepad.all[playerIndex].rightTrigger, 1);
            CheckAbilityButton(Gamepad.all[playerIndex].leftShoulder, 2);
            CheckAbilityButton(Gamepad.all[playerIndex].rightShoulder, 3);
        }
        //
        else if(abilitySelectionStatus == AbilitySelectionStatus.ShowingAbilities)
        {
            // Desplazamiento
            if (move.y > 0 && abilityChangeAllowed)
            {
                UpdateAbilitySelection(-1);
            }
            else if (move.y < 0 && abilityChangeAllowed)
            {
                UpdateAbilitySelection(1);
            }
            else if (move.y == 0)
            {
                abilityChangeAllowed = true;
            }
            //
            if (Gamepad.all[playerIndex].aButton.wasPressedThisFrame)
            {
                SelectAbility();
            }
        }
    }

    void SelectAbility()
    {
        //
        int alreadySelectedAbilityIndex = CheckIfAbilityAlreadySelected();
        if (alreadySelectedAbilityIndex == -1)
        {
            selectedAbilitiesIndexes[abilityToChangeIndex] = abilitySelectionIndex;
            abilityIcons[abilityToChangeIndex].texture = characterSelector.availableAbilities[abilitySelectionIndex].icon;
            // Por si estaba desactivado de amntes
            abilityIcons[abilityToChangeIndex].gameObject.SetActive(true);
        }
        else
        {
            // TODO: Intercambio de psoiciones
            int aux = selectedAbilitiesIndexes[abilityToChangeIndex];
            //
            selectedAbilitiesIndexes[abilityToChangeIndex] = abilitySelectionIndex;
            abilityIcons[abilityToChangeIndex].texture = characterSelector.availableAbilities[abilitySelectionIndex].icon;
            // Por si estaba desactivado de amntes
            abilityIcons[abilityToChangeIndex].gameObject.SetActive(true);
            //
            selectedAbilitiesIndexes[alreadySelectedAbilityIndex] = aux;
            if(aux > -1)
            {
                abilityIcons[alreadySelectedAbilityIndex].texture = characterSelector.availableAbilities[aux].icon;
            }
            else
            {
                abilityIcons[alreadySelectedAbilityIndex].gameObject.SetActive(false);
            }
        }
        //
        abilityIconBackgrounds[abilityToChangeIndex].SetActive(false);        
        availableAbilitiesGroup.SetActive(false);
        abilitySelectionStatus = AbilitySelectionStatus.Hidden;
        
        confirmationText.gameObject.SetActive(true);
    }

    int  CheckIfAbilityAlreadySelected()
    {
        //
        for(int i = 0; i < selectedAbilitiesIndexes.Length; i++)
        {
            if (selectedAbilitiesIndexes[i] == abilitySelectionIndex) return i;
        }
        //
        return -1;
    }

    void CheckAbilityButton(ButtonControl buttonToCheck, int abilityIndex)
    {
        if (buttonToCheck.wasPressedThisFrame/* && abilitySelectionStatus == AbilitySelectionStatus.Hidden*/)
        {
            abilitySelectionStatus = AbilitySelectionStatus.ShowingAbilities;
            abilityToChangeIndex = abilityIndex;
            abilitySelectionIndex = /*0*/ selectedAbilitiesIndexes[abilityIndex];
            selectionIndicator.anchoredPosition = new Vector2(0, -abilitySelectionIndex * 50);
            abilityIconBackgrounds[abilityToChangeIndex].SetActive(true);
            availableAbilitiesGroup.SetActive(true);
            confirmationText.gameObject.SetActive(false);
        }
    }

    void UpdateAbilitySelection(int direction)
    {
        //
        abilitySelectionIndex += direction;
        if (abilitySelectionIndex >= characterSelector.availableAbilities.Count)
            abilitySelectionIndex = 0;
        if (abilitySelectionIndex < 0)
            abilitySelectionIndex = characterSelector.availableAbilities.Count - 1;
        // TODO: Que se mueva bien el selector
        selectionIndicator.anchoredPosition = new Vector2(0, -abilitySelectionIndex * 50);
        //if(abilitySelectionIndex < characterSelector.availableAbilities.Count - 3)
        //    availableAbilitiesContent.anchoredPosition = new Vector2(0, abilitySelectionIndex * 50);
        availableAbilitiesContent.anchoredPosition = 
            new Vector2(0, Mathf.Min(abilitySelectionIndex, characterSelector.availableAbilities.Count - 4) * 50);
        abilityChangeAllowed = false;
        //
        cooldownText.text = "Cooldown: " + characterSelector.availableAbilities[abilitySelectionIndex].cooldown;
        descriptionText.text = characterSelector.availableAbilities[abilitySelectionIndex].description;
    }

    void UpdateCharacter(int direction)
    {
        //
        currentCharacterIndex += direction;
        if (currentCharacterIndex >= characterSelector.characterTemplates.Count)
            currentCharacterIndex = 0;
        if (currentCharacterIndex < 0)
            currentCharacterIndex = characterSelector.characterTemplates.Count - 1;
        //
        skin.texture = characterSelector.characterTemplates[currentCharacterIndex].skin;
        healthText.text = "Health - " + characterSelector.characterTemplates[currentCharacterIndex].health;
        attackMultiplierText.text = "Attac X - " + characterSelector.characterTemplates[currentCharacterIndex].attackMultiplier;
        speedMultiplierText.text = "Speed X - " + characterSelector.characterTemplates[currentCharacterIndex].speedMultiplier;
        //
        characterChangeAllowed = false;
    }

    void InitializeCharacter()
    {
        if (GameManager.Instance && GameManager.Instance.playerDatas[playerIndex] != null)
        {
            // TODO: A ver como sacamos esto
            // currentCharacterIndex = characterSelector.characterTemplates.IndexOf(GameManager.Instance.playerDatas[playerIndex].characterData);
            currentCharacterIndex = GameManager.Instance.playerDatas[playerIndex].characterIndex;
            // 
            skin.texture = characterSelector.characterTemplates[currentCharacterIndex].skin;
            healthText.text = "Health - " + characterSelector.characterTemplates[currentCharacterIndex].health;
            attackMultiplierText.text = "Attack X - " + characterSelector.characterTemplates[currentCharacterIndex].attackMultiplier;
            speedMultiplierText.text = "Speed X - " + characterSelector.characterTemplates[currentCharacterIndex].speedMultiplier;
        }
    }
    void InitializeAbilityIcons()
    {
        Debug.Log(characterSelector + ", " + abilityIcons);
        //
        selectedAbilitiesIndexes = new int[4];
        for (int i = 0; i < selectedAbilitiesIndexes.Length; i++)
            selectedAbilitiesIndexes[i] = -1;
                
        //
        if (GameManager.Instance && GameManager.Instance.playerDatas[playerIndex] != null)
        {
            //
            for (int i = 0; i < abilityIcons.Count; i++)
            {
                //if (i < GameManager.Instance.playerDatas[playerIndex].abilitiesDatas.Count)
                if (GameManager.Instance.playerDatas[playerIndex].abilitiesDatas[i])
                {
                    // TODO: Revisar esto
                    abilityIcons[i].texture = GameManager.Instance.playerDatas[playerIndex].abilitiesDatas[i].icon;
                    selectedAbilitiesIndexes[i] =
                        GameManager.Instance.availableAbilities.IndexOf(GameManager.Instance.playerDatas[playerIndex].abilitiesDatas[i]);
                }
                else
                {
                    abilityIcons[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            //
            for (int i = 0;/* i < characterSelector.availableAbilities.Count && */i < abilityIcons.Count; i++)
            {
                if(i < characterSelector.availableAbilities.Count)
                //if (characterSelector.availableAbilities[i])
                {
                    abilityIcons[i].texture = characterSelector.availableAbilities[i].icon;
                    selectedAbilitiesIndexes[i] = i;
                }
                else
                {
                    abilityIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void InitializeAvailableAbilities()
    {        
        //
        for(int i = 0; i < characterSelector.availableAbilities.Count; i++)
        {
            GameObject newabilityOption = Instantiate(abilityOptionPrefab, availableAbilitiesContent);
            //
            RawImage rawImage = newabilityOption.GetComponentInChildren<RawImage>();
            rawImage.texture = characterSelector.availableAbilities[i].icon;
            //
            TMP_Text optionText = newabilityOption.GetComponentInChildren<TMP_Text>();
            optionText.text = characterSelector.availableAbilities[i].name;
            //
            RectTransform rt = newabilityOption.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, i * -50);
            //
            newabilityOption.SetActive(true);
        }
        //
        availableAbilitiesContent.sizeDelta = new Vector2(200, characterSelector.availableAbilities.Count * 50);
        //
        availableAbilitiesGroup.SetActive(false);
    }

    public void ResetPanel()
    {
        panelStatus = PanelStatus.PendingConfirmation;
        confirmationPanel.SetActive(true);
    }

    public void ConfirmCharacterSelection()
    {
        // TODO: Hacer que meta nulls o habilidades no usables
        // OJO Aqui!
        List<AbilityScriptableObject> abilitiesDatas = new List<AbilityScriptableObject>(4);
        //
        for(int i = 0; i < 4; i++)
        {
            if (selectedAbilitiesIndexes[i] >= 0)
                abilitiesDatas.Add(characterSelector.availableAbilities[selectedAbilitiesIndexes[i]]);
            else
                abilitiesDatas.Add(null);
        }
        //
        //if (selectedAbilitiesIndexes[0] >= 0) abilitiesDatas.Add(characterSelector.availableAbilities[selectedAbilitiesIndexes[0]]);
        //if (selectedAbilitiesIndexes[1] >= 0) abilitiesDatas.Add(characterSelector.availableAbilities[selectedAbilitiesIndexes[1]]);
        //if (selectedAbilitiesIndexes[2] >= 0) abilitiesDatas.Add(characterSelector.availableAbilities[selectedAbilitiesIndexes[2]]);
        //if (selectedAbilitiesIndexes[3] >= 0) abilitiesDatas.Add(characterSelector.availableAbilities[selectedAbilitiesIndexes[3]]);
        gameManager.playerDatas[playerIndex] = 
            new PlayerData(characterSelector.characterTemplates[currentCharacterIndex], abilitiesDatas, currentCharacterIndex);
    }
}
