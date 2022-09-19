using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesManager : MonoBehaviour
{
    public enum SpawnMode
    {
        Square,
        Circular,
        Boss,
        Count
    }
    //
    [Header("Waves")]
    public LevelEnemiesScriptableObject levelEnemiesScriptableObject;
    public CompleteWave[] waves;
    [Header("UI")]
    public TMP_Text waveText;
    public GameObject bossGroup;
    public TMP_Text bossName;
    public RectTransform bossHealthBar;
    public RawImage bossHealthBarImage;
    public RawImage bossHealthBarBackground;

    private static EnemiesManager instance;

    private GameManager gameManager;
    private int currentWave = 0;
    private float currentTimeToWave;
    private SpawnMode defaultSpawnMode = SpawnMode.Circular;
    //private bool activeBoss = false;

    private Color[] bossHealthBarColors = { Color.black, Color.red, Color.yellow, Color.green, Color.blue, Color.magenta };

    [HideInInspector] public List<EnemyController> enemyControllers;

    public static EnemiesManager Instance { get { return instance; } }

    public bool LastRound
    {
        get { return currentWave == waves.Length; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //
        instance = this;

        //
        gameManager = GameManager.Instance;

        //
        enemyControllers = new List<EnemyController>(100);

        //
        if (gameManager)
            levelEnemiesScriptableObject = gameManager.currentLevelData;

        //StartCoroutine(WaitAndSpawnEnemies());
        //SpawnWaveEnemies();

        // Si le setamos una lista de oleadas que use esa
        if (levelEnemiesScriptableObject)
        {
            if (!levelEnemiesScriptableObject.usePrefabOnes)
            {
                waves = levelEnemiesScriptableObject.waves;
            }
            else
            {
                GetPrefabWaves();
            }
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        //if(currentWave < waves.Length)
        if (currentWave == 0)
        {
            currentTimeToWave += dt;
            if (currentTimeToWave >= waves[currentWave].timeToStart)
            {
                NextWave();
                LevelManager.Instance.ResetTimer();
            }
        }        
    }

    void GetPrefabWaves()
    {
        waves = new CompleteWave[levelEnemiesScriptableObject.prefabWaves.Length];
        for(int i = 0; i < levelEnemiesScriptableObject.prefabWaves.Length; i++)
        {
            waves[i] = levelEnemiesScriptableObject.prefabWaves[i].completeWave;
        }
    }

    public void NextWave()
    {
        SpawnWaveEnemies();
        currentWave++;
        currentTimeToWave = 0;
        //
        if (currentWave == waves.Length)
            waveText.text = "LAST";
        else
            waveText.text = currentWave + "";
    }

    void SpawnWaveEnemies()
    {
        //
        for(int i = 0; i < waves[currentWave].enemies.Length; i++)
        {
            // Info de boss solo con el primer bicho de la ronda boss
            if (!waves[currentWave].isBoss || i!= 0)
            {
                SpawnEnemies(waves[currentWave].enemies[i], new Vector3(0, 0, 0), defaultSpawnMode);
            }
            else
            {
                // NOTA: Ahora mismo está preparado para un solo BOSS
                SpawnEnemies(waves[currentWave].enemies[i], new Vector3(0, 0, 0), SpawnMode.Boss);
                //activeBoss = true;
                bossGroup.SetActive(true);
                bossName.text = waves[currentWave].enemies[i].enemyPrefab.name;
            }                
        }
    }

    public void SpawnEnemies(EnemyWave enemyWave, Vector3 epicenter, SpawnMode spawnMode, bool givesPoints = true)
    {
        //
        for(int i = 0; i < enemyWave.numEnemies; i++)
        {
            //
            Vector3 nextPosition = new Vector3();
            //
            switch (spawnMode)
            {
                case SpawnMode.Square:
                    nextPosition = epicenter + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
                    break;
                case SpawnMode.Circular:
                    float angle = UnityEngine.Random.Range(0f, 360f);
                    float radius = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * 10f;  //Sacar publica de aqui
                    float xPosition = radius * Mathf.Cos(angle);
                    float zPosition = radius * Mathf.Sin(angle);
                    nextPosition = epicenter + new Vector3(xPosition, 0, zPosition);
                    break;
                case SpawnMode.Boss:
                    nextPosition = epicenter;
                    break;
            }
            //
            GameObject spawnedEnemy = Instantiate(enemyWave.enemyPrefab, nextPosition, Quaternion.identity);
            //
            EnemyController enemyController = spawnedEnemy.GetComponent<EnemyController>();
            //  Así la gente no farmea puntos de los bichos que vaya soltando un boss
            if (!givesPoints)
                enemyController.score = 0;
            //
            enemyControllers.Add(enemyController);
            // Si es boss le chufamos para mostrar la barra grande
            if(spawnMode == SpawnMode.Boss)
            {
                HealthController healthController = spawnedEnemy.GetComponent<HealthController>();
                healthController.enemiesManager = this;
            }
            
        }
    }

    public void UpdateBossHealthBar(int phaseIndex)
    {
        bossHealthBarImage.color = bossHealthBarColors[phaseIndex+1];
        bossHealthBarBackground.color = bossHealthBarColors[phaseIndex];
    }

    public void BossDeath()
    {
        bossGroup.SetActive(false);
    }

    //IEnumerator WaitAndSpawnEnemies()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);
    //        SpawnEnemies(2, new Vector3(0, 0, 10), 5);
    //    }        
    //}

    public Transform GetNearestEnemyTransform(Vector3 position)
    {
        //
        if (enemyControllers.Count == 0)
            return null;
        //
        int nearestEnemyIndex = -1;
        float nearestPlayerDistance = Mathf.Infinity;
        for (int i = 0; i < enemyControllers.Count; i++)
        {
            // Chequeo de player mas cernacon
            float nextPlayerDistance = (enemyControllers[i].transform.position - position).sqrMagnitude;
            //
            if (nextPlayerDistance < nearestPlayerDistance)
            {
                nearestEnemyIndex = i;
                nearestPlayerDistance = nextPlayerDistance;
            }
        }
        return enemyControllers[nearestEnemyIndex].transform;
    }
}

[Serializable]
public class CompleteWave
{
    public EnemyWave[] enemies;
    public float timeToStart;
    public bool isBoss = false;
}

[Serializable]
public class EnemyWave
{
    //
    public int numEnemies = 20;
    //
    public GameObject enemyPrefab;
}