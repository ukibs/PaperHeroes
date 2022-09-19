using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Owner para todos
// 
public enum Owner
{
    Invalid = -1,
    Player,
    Enemy,

    Count
}

public class HealthController : MonoBehaviour
{
    

    public int maxHealth = 10;
    public Texture2D greenTexture;
    public Texture2D redTexture;
    public Texture2D whiteTexture;  // Para aplicarle colores por código
    public AudioClip deathClip;

    [HideInInspector] public List<AlteredState> alteredStates;
    [HideInInspector] public EnemiesManager enemiesManager;     //Para bosses

    private int currentHealth = -1;     // -1 para marcarlo como no inicializado
    private PlayerController playerController;
    private PlayersManager playersManager;
    private AudioManager audioManager;
    private SpriteController spriteController;
    private Owner owner = Owner.Invalid;

    private float invulnerableTimeAfterHit = 1;
    private float currentInvulnerableTimeAfterHit;

    private int bossPhase = 0;

    public int CurrentHealth { get { return currentHealth; } }
    public Owner OwnerCharacter { get { return owner; } }

    // Start is called before the first frame update
    void Start()
    {
        //
        playerController = GetComponent<PlayerController>();
        playersManager = FindObjectOfType<PlayersManager>();
        audioManager = FindObjectOfType<AudioManager>();
        spriteController = GetComponentInChildren<SpriteController>();
        //
        GetOwner();
        //
        switch (owner)
        {
            case Owner.Player: currentHealth = maxHealth; break;
            // Toque extra para eveitar "0 jugadores"
            case Owner.Enemy: 
                // TODO: Revisar que no se descojone por esto
                maxHealth = maxHealth * Mathf.Max(1, playersManager.PlayerControllerList.Count);
                currentHealth = maxHealth;
                break;
        }
        //
        alteredStates = new List<AlteredState>(3);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        UpdateAlteredStates(dt);
        //
        if (currentInvulnerableTimeAfterHit >= 0)
            currentInvulnerableTimeAfterHit -= dt;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log(gameObject.name + "collided with " + hit.collider.name);
        //ProyectilControl proyectilControl = hit.collider.GetComponent<ProyectilControl>();
        //if (proyectilControl)
        //{
        //    proyectilControl.impactedHealthController = this;
        //    int totalDamage = (int)(proyectilControl.damage * proyectilControl.attackMultiplier);
        //    ReceiveAttack(totalDamage, proyectilControl.alteredState, proyectilControl.alteredStateDuration);
        //    Destroy(proyectilControl.gameObject);
        //}
    }

    private void OnGUI()
    {
        //
        if (playerController)
        {
            playerController.playerHUD.playerHealthBar.fillAmount = (float)currentHealth / (float)maxHealth;
            playerController.playerHUD.playerIcon.color = spriteController.SpriteColor;
        }
        else if(enemiesManager != null)
        {
            enemiesManager.bossHealthBar.sizeDelta = new Vector2((float)currentHealth / (float)maxHealth * 1000f, 50);
        }
        else
        {
            //Debug.Log("Drawing normal enemy health");
            //
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            //
            if (currentHealth < maxHealth)
            {
                GUI.DrawTexture(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 20, 20, 6), redTexture);

                GUI.DrawTexture(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 20,
                    (float)currentHealth / (float)maxHealth * 20, 6), greenTexture);
            }
        }
        
    }

    public void ReceiveAttack(int damage, AlteredState.Type alteredState, float stateDuration)
    {
        //
        if (currentInvulnerableTimeAfterHit >= 0)
            return;
        //
        switch (owner)
        {
            // El daño recibido por los jugadores depende de la cantidad de jugadores activos
            //case Owner.Player: damage *= playersManager.PlayerControllerList.Count; break;
            case Owner.Player: 
                damage += (int)(damage * 0.1f * (playersManager.PlayerControllerList.Count - 1));
                currentInvulnerableTimeAfterHit = invulnerableTimeAfterHit;
                LevelManager.Instance.ShowDamageOverlay();
                break;
            case Owner.Enemy:
                // De momento nada especial
                break;
        }
        //
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        //Debug.Log("Suffered " + damage + " damage");
        spriteController.ShowDamage();
        //
        if(alteredState != AlteredState.Type.None && stateDuration > 0)
        {
            alteredStates.Add(new AlteredState(alteredState, stateDuration));
        }
        //
        //if(currentHealth == 0 && enemiesManager != null)
        //{
        //    enemiesManager.BossDeath();
        //}
    }

    public void Restore()
    {
        currentHealth = maxHealth;
    }

    public void Restore(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void GetOwner()
    {
        if (playerController)
            owner = Owner.Player;
        //
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController)
            owner = Owner.Enemy;
    }

    void UpdateAlteredStates(float dt)
    {
        for(int i = 0; i < alteredStates.Count; i++)
        {
            alteredStates[i].remainingTime -= dt;
            if(alteredStates[i].remainingTime <= 0)
            {
                alteredStates.RemoveAt(i);
                i--;
            }
        }
    }

    //
    public void NextBossPhase(int newMaxHealth, int bossPhaseIndex)
    {
        maxHealth = newMaxHealth * Mathf.Max(1, playersManager.PlayerControllerList.Count);
        currentHealth = maxHealth;
        bossPhase=bossPhaseIndex;
        enemiesManager.UpdateBossHealthBar(bossPhaseIndex);
    }
}

public class AlteredState
{
    public enum Type
    {
        None = -1,

        Paralized,
        Bubbled,

        Count
    }

    public Type type;
    public float remainingTime;

    public AlteredState(Type type, float duration)
    {
        this.type = type;
        remainingTime = duration;
    }
}