using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState {
        Title,
        Combat,
        Upgrade,
        GameOver
    }

    public static GameManager instance;
    public GameState gameState;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        gameState = GameState.Combat;
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
