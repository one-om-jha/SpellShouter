using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private string buffer = "";
    private string input = "";

    // Text
    [SerializeField]
    private TMP_Text bufferText;

    private Camera cam;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        // get input from keyboard
        if (Input.inputString != input)
        {
            input = Input.inputString;
            buffer += input;
            cam.GetComponent<CameraManager>().StartShake(0.1f, 0.05f);
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
            cam.GetComponent<CameraManager>().StartShake(0.2f, 0.2f);
        }

        // update text
        bufferText.text = buffer;
    }
}
