using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private string buffer = "";
    private string input = "";

    [SerializeField]
    private TMP_Text bufferText;

    private void Update() {
        // get input from keyboard
        if (Input.inputString != input)
        {
            input = Input.inputString;
            buffer += input;
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
        }

        // update text
        bufferText.text = buffer;
    }
}
