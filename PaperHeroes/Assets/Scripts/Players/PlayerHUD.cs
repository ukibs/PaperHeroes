using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public RawImage playerIcon;
    public Image playerHealthBar;
    public GameObject[] playerAbilities;

    private RawImage[] abilityIcons;
    [HideInInspector] public Image[] abilityCooldowns;

    // Start is called before the first frame update
    void Start()
    {
        if (abilityIcons == null)  GetAbilitiesIconsAndCooldowns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetAbilitiesIconsAndCooldowns()
    {
        abilityIcons = new RawImage[playerAbilities.Length];
        abilityCooldowns = new Image[playerAbilities.Length];
        //
        for(int i = 0; i < playerAbilities.Length; i++)
        {
            abilityIcons[i] = playerAbilities[i].GetComponentInChildren<RawImage>();
            abilityCooldowns[i] = playerAbilities[i].GetComponentInChildren<Image>();
        }
    }

    public void Initiate(AbilityScriptableObject[] abilities, Texture2D icon = null)
    {
        //
        if(icon != null)
            playerIcon.texture = icon;
        //
        if (abilityIcons == null) GetAbilitiesIconsAndCooldowns();
        //
        for(int i = 0; i < playerAbilities.Length; i++)
        {
            //
            if(/*i < abilities.Length*/ abilities[i] != null)
            {
                abilityIcons[i].texture = abilities[i].icon;
                playerAbilities[i].SetActive(true);
            }
            else
            {
                playerAbilities[i].SetActive(false);
            }
        }
    }
}
