using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    private int currWave;
    private int waveValue;

    public int waveCount;

    public int costScale = 10;

    public float spawnInterval;

    private float spawnTimer;
    private int waveKills;
    private int enemyCount;

    private float spawnOffset = 300f;

    private Camera cam;

    private TMP_Text waveText;

    private Image waveImage;

    enum WaveState
    {
        WAITING,
        SPAWNING,
        DONE
    }

    private List<GameObject> enemiesToSpawn = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    private WaveState waveState;

    private void Start()
    {
        PlayerController.instance.onKill += onKill;
        cam = Camera.main;
        waveState = WaveState.WAITING;
        currWave = 1;
        waveText = PlayerController.instance.waveText;
        waveImage = PlayerController.instance.waveImage;
        GenerateWave();
    }

    private void FixedUpdate()
    {
        if (waveState == WaveState.SPAWNING)
        {
            // Clean up enemiesToSpawn by removing nulls
            spawnedEnemies.RemoveAll(x => x == null);

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
                spawnTimer -= Time.fixedDeltaTime;
            }

            if (WaveOver())
            {
                waveState = WaveState.DONE;
            }
        }
        if (waveState == WaveState.DONE)
        {
            if (currWave < waveCount)
            {
                currWave++;
                waveState = WaveState.WAITING;
                enemyCount = 0;
                waveKills = 0;
                GenerateWave();
            }
            else
            {
                // Level Over!
                PlayerController.instance.LoadLevelSelect();
            }
        }
    }

    private void LateUpdate()
    {
        waveText.text = "Wave " + currWave;
        waveImage.fillAmount = waveKills / (float)enemyCount;
    }

    public bool WaveOver()
    {
        return waveKills >= enemyCount;
    }

    private void onKill()
    {
        waveKills++;
    }

    public void GenerateWave()
    {
        waveValue = currWave * costScale;
        GenerateEnemies();

        waveState = WaveState.SPAWNING;
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

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0)
        {
            int randEnemyId = UnityEngine.Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                waveValue -= randEnemyCost;
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
        enemyCount = enemiesToSpawn.Count;
    }

    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int cost;
    }
}
