using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveMachine : MonoBehaviour
{
    public static WaveMachine instance;

    // GAME VALUES
    public List<EnemyTier> enemyTiers = new List<EnemyTier>();

    // WAVE VALUES
    public int currWave;
    public int waveCount;
    public int scale = 10;
    private int waveValue;
    public float spawnInterval = 2;
    private float spawnTimer;

    private int waveKills;
    private int enemyCount;

    // SPAWN VALUES
    private float spawnOffset = 100f;
    private Camera cam;
    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    // REFERENCES
    private GameManager gm;
    public TMP_Text waveText;
    public Image waveImage;
    public GameObject waveComplete;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        cam = Camera.main;
        gm = GameManager.instance;
        currWave = 0;
        gm.onKill += onKill;
        GenerateWave();
    }

    private void GenerateWave()
    {
        currWave++;
        gm.wave++;
        waveValue = currWave * scale;
        // spawn interval decreases as currWave goes up
        spawnInterval = 1.5f - (currWave * 0.1f);
        waveKills = 0;
        GenerateEnemies();
        Debug.Log("Wave " + currWave + " generated with " + enemyCount + " enemies");
    }

    private void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0)
        {
            int randEnemyId = UnityEngine.Random.Range(0, enemyTiers[0].enemies.Count);
            int randEnemyCost = enemyTiers[0].enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                waveValue -= randEnemyCost;
                ;
                generatedEnemies.Add(enemyTiers[0].enemies[randEnemyId].enemyPrefab);
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
        enemyCount = enemiesToSpawn.Count;
    }

    public Vector3 GetSpawnLocation()
    {
        // Select randomly x or y axis
        int axis = UnityEngine.Random.Range(0, 2);
        // Select randomly positive or negative
        int sign = UnityEngine.Random.Range(0, 2) * 2 - 1;

        if (axis == 0)
        {
            // get random world location on x axis on width of screen
            float x = UnityEngine.Random.Range(
                cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x,
                cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x
            );
            return new Vector3(
                x,
                sign * cam.ScreenToWorldPoint(new Vector3(0, Screen.height + spawnOffset, 0)).y,
                0
            );
        }
        else
        {
            // get random world location on y axis on height of screen
            float y = UnityEngine.Random.Range(
                cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y,
                cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y
            );
            return new Vector3(
                sign * cam.ScreenToWorldPoint(new Vector3(Screen.width + spawnOffset, 0, 0)).x,
                y,
                0
            );
        }
    }

    private void Update()
    {
        if (gm.gameState == GameManager.GameState.Combat)
        {
            // spawn enemies
            if (spawnTimer <= 0)
            {
                if (enemiesToSpawn.Count > 0)
                {
                    spawnedEnemies.Add(
                        Instantiate(enemiesToSpawn[0], GetSpawnLocation(), Quaternion.identity)
                    );
                    enemiesToSpawn.RemoveAt(0);
                    spawnTimer = spawnInterval;
                }
            }
            else
            {
                spawnTimer -= Time.deltaTime;
            }

            if (WaveOver())
            {
                GenerateWave();
                Instantiate(waveComplete, Vector3.zero, Quaternion.identity);
            }
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        waveText.text = "Wave " + currWave;
        waveImage.fillAmount = (float)waveKills / enemyCount;
    }

    private bool WaveOver()
    {
        return waveKills >= enemyCount;
    }

    private void onKill()
    {
        waveKills++;
    }

    // CLASSES
    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int cost;
    }

    [System.Serializable]
    public class EnemyTier
    {
        public List<Enemy> enemies = new List<Enemy>();
    }
}
