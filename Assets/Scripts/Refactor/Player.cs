using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    // TEXT INPUT
    private string buffer = "";
    private string input = "";

    // GAME VALUES
    public int health = 3;

    // REFERENCES
    public TMP_Text bufferText;
    public GameObject slash;
    public Image healthImage;

    private Camera cam;
    private SpriteRenderer spriteRenderer;
    private GameManager gm;

    // EVENTS
    public event Action onInput;
    public event Action onAttack;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        gm = GameManager.instance;
    }

    private void Update()
    {
        if (gm.gameState == GameManager.GameState.Combat)
        {
            // get keyboard input
            if (Input.inputString != input)
            {
                input = Input.inputString;
                buffer += input;
                if (onInput != null)
                {
                    onInput();
                    // Fire Input Event
                }
            }

            // handle attack with return
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (onAttack != null)
                {
                    onAttack();
                    // Fire Attack Event
                }
                buffer = "";
                bufferText.text = buffer;
            }

            // update buffer
            buffer = buffer.ToLower();
            UpdateUI();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Instantiate(slash, transform.position, Quaternion.identity);
            health -= 1;
            gm.hitStop();
            gm.combo = 0;
            gm.UpdateUI();
            StartCoroutine(Flash());
            if (health <= 0)
            {
                Die();
            }
            Destroy(other.gameObject);
        }
    }

    IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        gm.gameState = GameManager.GameState.GameOver;
        Debug.Log("You died");
    }

    private void UpdateUI()
    {
        bufferText.text = buffer;
        healthImage.rectTransform.sizeDelta = new Vector2(
            health * 80,
            healthImage.rectTransform.sizeDelta.y
        );
    }

    public string GetBuffer()
    {
        return buffer;
    }

    public void backspace()
    {
        if (buffer.Length - 2 >= 0)
        {
            buffer = buffer.Substring(0, buffer.Length - 2);
            UpdateUI();
        }
    }
}
