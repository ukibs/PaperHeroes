using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public enum GameState
    {
        Running,
        Victory,
        Defeat,

        Count
    }

    public GameState currentGameState = GameState.Running;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public AudioClip victoryClip;
    public AudioClip defeatClip;
    public TMP_Text scoreText;
    public RawImage damageOverlay;

    private static LevelManager instance;

    private GameManager gameManager;
    private PlayersManager playersManager;
    private EnemiesManager enemiesManager;
    private bool gameOver = false;
    private bool victory = false;
    private int currentScore = 0;
    private float roundTime = 30;
    private float overlayAlpha = 0;

    public static LevelManager Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        //
        instance = this;
        //
        playersManager = FindObjectOfType<PlayersManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        gameManager = FindObjectOfType<GameManager>();

        //
        //if (gameManager)
        //{
        //    level
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        if (gameOver)
        {
            if(Gamepad.all[0].startButton.isPressed)
            {
                if (!victory)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    SceneManager.LoadScene("Map");
                }
            }
            if (Gamepad.all[0].selectButton.isPressed)
            {
                SceneManager.LoadScene("Map");
            }
        }
        else
        {
            if(overlayAlpha > 0)
            {
                overlayAlpha -= dt;
                damageOverlay.color = new Color(1,1,1, overlayAlpha);
            }
        }
        //
        if(roundTime > 0)
        {
            roundTime -= Time.deltaTime;
        }
    }

    //
    public void NewDeath(int score = 0)
    {
        //
        currentScore += score;
        scoreText.text = currentScore.ToString();
        // Todos los jugadores muertos
        if (playersManager.AlivePlayers() == 0)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = "DEFEAT";
            AudioManager.Instance.PutGameOverClip(defeatClip, 1);
            gameOver = true;
            // TODO: Score de derrota
            gameManager.SetMaxScore(currentScore);
            return;
        }
        //    
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();
        if(AllEnemiesDead(enemyControllers))
        {
            //
            currentScore += (int)(roundTime * 10);
            scoreText.text = currentScore.ToString();
            //
            if (enemiesManager.LastRound)
            {
                gameOverPanel.SetActive(true);
                gameOverText.text = "VICTORY";
                AudioManager.Instance.PutGameOverClip(victoryClip, 0.5f);
                victory = true;
                gameOver = true;
                //
                if (gameManager)
                {
                    gameManager.levelCompleted = true;
                    // TODO: Score de vicotria
                    gameManager.SetMaxScore(currentScore);
                }
            }
            else
            {
                //Debug.Log("Enemies remaining: " + enemyControllers.Length);
                enemiesManager.NextWave();
                ResetTimer();
            }
        }           
        
    }

    // Llamada desde el boss cuando cambia de fase
    public void UpdateScore(int score = 0)
    {
        currentScore += score;        
        currentScore += (int)(roundTime * 10);
        scoreText.text = currentScore.ToString();
        ResetTimer();
    }

    //
    bool AllEnemiesDead(EnemyController[] enemyControllers)
    {
        for(int i = 0; i < enemyControllers.Length; i++)
        {
            if (!enemyControllers[i].isDead)
                return false;
        }
        return true;
    }

    //
    public void ResetTimer()
    {
        roundTime = 30;
    }

    //
    public void ShowDamageOverlay()
    {
        overlayAlpha = 1;
    }

}
