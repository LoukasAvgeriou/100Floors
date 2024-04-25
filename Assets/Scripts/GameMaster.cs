using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    #region Singleton

    public static GameMaster Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public Transform[] enemySpawns;

    private float nextSpawn = 1.5f;
    public int currentEnemies = 0;
    public int maxEnemiesToField = 5;

    public float minEnemyRespawnTimer = 1f;
    public float maxEnemyRespawnTimer = 2f;

    public bool mouseMovement = true;

    private void Update()
    {
        if (Time.time >= nextSpawn && currentEnemies < maxEnemiesToField)
        {   // It's time to spawn a new enemy and we have room to add more.
            nextSpawn = Time.time + Random.Range(minEnemyRespawnTimer, maxEnemyRespawnTimer); // Set the next spawn time to a random time between the min and max.

            currentEnemies++; // Add 1 to the current enemies alive.

            //spawn random enemy to random spawn point
            GameObject enemy1 = ObjectPooler.SharedInstance.GetPooledObject(RandomEnemy());
            if (enemy1 != null)
            {
                Transform enemy1Spawn = enemySpawns[Random.Range(0, enemySpawns.Length)];

                enemy1.transform.position = enemy1Spawn.position;
                enemy1.transform.rotation = enemy1Spawn.rotation;
                enemy1.SetActive(true);
            }
        }
    }

    private string RandomEnemy()
    {
        int random = Random.Range(1, 100); // Pick a random number for us to use.

        if (random <= 50)
        {   //  chance to spawn ranged enemy.
            return "rangedEnemy";
        }
        else
        {   //  chance to spawn sword enemy.
            return "mainEnemy";
        }
    }

    public void KillPlayer(GameObject player)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Player playerScript = player.GetComponent<Player>();

        if (!playerScript.inAttack && !playerScript.inDefence)
        {
            player.SetActive(false);
            SceneManager.LoadScene("EndMenu");
        }

    }

    public void KillEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        currentEnemies--;

        ScreenShakeController.instance.StartShake(.1f, .1f);
    }

    public void KillmainEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        currentEnemies--;

        ScreenShakeController.instance.StartShake(.1f, .1f);
    }

    public void BreakArrow(GameObject arrow)
    {
        arrow.SetActive(false);
    }

    public void KillRangedEnemy()
    {

    }
}
