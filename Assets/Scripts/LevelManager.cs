using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int level = 1;

    public void LoadLevel()
    {
        PlayerController.instance.LoadLevel("Level" + level);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            LoadLevel();
        }
    }
}
