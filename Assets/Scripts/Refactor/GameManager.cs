using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Title,
        Combat,
        Upgrade,
        GameOver
    }

    public static GameManager instance;
    public GameState gameState;

    // GAME VALUES
    public int score;
    public int combo;
    public int wave;
    public int levelThreshold;

    public List<GameObject> upgrades = new List<GameObject>();
    public List<Upgrades> currentUpgrades = new List<Upgrades>();

    // EVENTS
    public event Action onKill;

    // REFERENCES
    public TMP_Text scoreText;
    public Animator upgradeAnimator;
    public RectTransform upgradePanel;
    public TMP_Text upgradeText;

    // UPGRADE REFERENCES
    public Item item1Button;
    public Item item2Button;

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
        score = 0;
        combo = 0;
        levelThreshold = 100;
        gameState = GameState.Combat;
    }

    private void Update()
    {
        if (gameState == GameState.Combat)
        {
            if (score > levelThreshold)
            {
                levelThreshold *= 3;
                Upgrade();
            }
        }
    }

    public void Upgrade()
    {
        if (upgrades.Count > 0)
        {
            gameState = GameState.Upgrade;
            LeanTween.moveY(upgradePanel, 0, 0.25f).setEase(LeanTweenType.easeOutBack);
            // select random upgrade
            int random1 = UnityEngine.Random.Range(0, upgrades.Count);
            int random2 = UnityEngine.Random.Range(0, upgrades.Count);

            // set upgrade text
            item1Button.item = random1;
            item2Button.item = random2;
            item1Button.UpdateItem();
            item2Button.UpdateItem();
        }
    }

    public void UpdateUI()
    {
        scoreText.text = "Score " + score + "\nCombo x" + combo;
        upgradeText.text = "";
        for (int i = 0; i < currentUpgrades.Count; i++)
        {
            upgradeText.text += currentUpgrades[i].upgradeName + "\n";
        }
    }

    public void SpawnUpgrade(int itemNumber)
    {
        if (upgrades.Count > 0)
        {
            // spawn upgrade
            GameObject upgrade = Instantiate(upgrades[itemNumber]);
            upgrades.RemoveAt(itemNumber);
            upgrade.transform.position = new Vector3(
                UnityEngine.Random.Range(-2.5f, 2.5f),
                UnityEngine.Random.Range(2.5f, 5f),
                0
            );
            upgrade.transform.SetParent(transform);

            currentUpgrades.Add(upgrade.GetComponent<Upgrades>());
            UpdateUI();

            gameState = GameState.Combat;
            LeanTween.moveY(upgradePanel, 1080, 0.25f).setEase(LeanTweenType.easeOutBack);
        }
    }

    public void markKill()
    {
        score += combo * 100;
        combo++;
        onKill();
        UpdateUI();
    }

    public void hitStop()
    {
        StartCoroutine(hitStopCoroutine());
    }

    IEnumerator hitStopCoroutine()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }
}
