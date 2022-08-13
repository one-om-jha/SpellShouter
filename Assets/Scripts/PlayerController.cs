using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public string buffer = "";
    private string input = "";

    private int health = 3;

    // Text
    [SerializeField]
    private TMP_Text bufferText;

    [SerializeField]
    private Image healthImage;

    private Camera cam;

    public event Action onInput;
    public event Action onAttack;
    public event Action onKill;

    [SerializeField]
    private GameObject slash;

    private bool suspended = false;

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
    }

    private void Update()
    {
        if (!suspended)
        {
            // get input from keyboard
            if (Input.inputString != input)
            {
                input = Input.inputString;
                buffer += input;
                if (onInput != null)
                {
                    onInput();
                }
            }

            // handle deleting input
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                buffer = buffer.Substring(0, buffer.Length - 2);
            }

            // handle enter
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bufferText.text = buffer;
                buffer = "";
                if (onAttack != null)
                {
                    onAttack();
                }
            }

            buffer = buffer.ToLower();
            // update text
            bufferText.text = buffer;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        healthImage.rectTransform.sizeDelta = new Vector2(
            health * 100,
            healthImage.rectTransform.sizeDelta.y
        );
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            health--;
            Instantiate(slash, transform.position, Quaternion.identity);
            hitStop();
            if (health == 0)
            {
                Die();
            }
            Destroy(other.gameObject);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void hitStop()
    {
        StartCoroutine(HitStop());
    }

    IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }

    public void Suspend()
    {
        suspended = true;
        bufferText.enabled = false;
    }

    public void Resume()
    {
        suspended = false;
        bufferText.enabled = true;
    }

    public void markKill()
    {
        onKill();
    }
}
