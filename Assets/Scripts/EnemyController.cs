using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class EnemyController : MonoBehaviour
{
    public string health = "Apple";
    public float speed = 1f;

    private int damage = 0;

    [SerializeField]
    public GameObject slash;

    public enum State
    {
        Invisible,
        Visible,
        Vulnerable,
        Dead
    }

    public State state = State.Visible;

    private TMP_Text healthText;

    private VertexJitter jitter;

    private Vector3 targetPosition;

    private void Start()
    {
        Player.instance.onInput += playerInput;
        Player.instance.onAttack += playerAttack;
        healthText = GetComponentInChildren<TMP_Text>();
        jitter = GetComponentInChildren<VertexJitter>();

        targetPosition = Player.instance.transform.position;
    }

    private void playerInput()
    {
        string input = Player.instance.GetBuffer();
        health = health.ToLower();

        // Find longest length substring that is shared between health and input
        int maxLength = 0;
        for (int i = 0; i < health.Length; i++)
        {
            for (int j = 0; j < input.Length; j++)
            {
                if (health[i] == input[j])
                {
                    int length = 0;
                    for (int k = i, l = j; k < health.Length && l < input.Length; k++, l++)
                    {
                        if (health[k] == input[l])
                        {
                            length++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (length > maxLength)
                    {
                        maxLength = length;
                    }
                }
            }
        }
        damage = maxLength;
        UpdateText();
    }

    private void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.Combat)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (state == State.Visible)
            {
                if (damage >= health.Length)
                {
                    GameManager.instance.hitStop();
                    state = State.Vulnerable;
                    jitter.enabled = true;
                    healthText.color = Color.red;
                    GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    private void UpdateText()
    {
        if (healthText == null)
        {
            return;
        }

        for (int i = 0; i < damage; i++)
        {
            int charIndex = healthText.textInfo.characterInfo[i].index;
            int meshIndex = healthText.textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = healthText.textInfo.characterInfo[i].vertexIndex;

            Color32[] vertexColors = healthText.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = Color.red;
            vertexColors[vertexIndex + 1] = Color.red;
            vertexColors[vertexIndex + 2] = Color.red;
            vertexColors[vertexIndex + 3] = Color.red;
        }
        healthText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void playerAttack()
    {
        if (state == State.Vulnerable)
        {
            Die();
        }
    }

    private void Die()
    {
        state = State.Dead;
        GameObject child = Instantiate(slash, transform.position, Quaternion.identity);
        child.transform.parent = transform;
        GameManager.instance.markKill();
        Destroy(gameObject, 0.25f);
    }
}
