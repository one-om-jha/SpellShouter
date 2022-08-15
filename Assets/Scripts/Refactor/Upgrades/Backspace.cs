using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backspace : Upgrades
{
    private void Update() {
        if (GameManager.instance.gameState == GameManager.GameState.Combat) {
            if (Input.GetKeyDown(KeyCode.Backspace)) {
                Player.instance.backspace();
            }
        }
    }
}
