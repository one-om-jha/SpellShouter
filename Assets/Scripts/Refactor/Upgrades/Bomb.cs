using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Upgrades
{
    public string cast = "bomb";

    private void Start() {
        Player.instance.onAttack += Cast;
    }

    private void Cast()
    {
        if (Player.instance.GetBuffer().ToLower().Contains(cast))
        {
            StartCoroutine(BombCoroutine());
        }
    }

    IEnumerator BombCoroutine()
    {
        for (int i = 0; i < WaveMachine.instance.spawnedEnemies.Count; i++)
        {
            if (WaveMachine.instance.spawnedEnemies[i] != null)
            {
                WaveMachine.instance.spawnedEnemies[i].GetComponent<EnemyController>().state = EnemyController.State.Vulnerable;
            }
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
