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

    //enemies need to kill so you finish the level
    public int enemiesToKill = 20;

    private bool levelCompleted = false;

    public UpgradesControllerSO upgradeControllerSO;

    private void Update()
    {
        if (Time.time >= nextSpawn && currentEnemies < maxEnemiesToField && !levelCompleted)
        {   // It's time to spawn a new enemy and we have room to add more.
            nextSpawn = Time.time + Random.Range(minEnemyRespawnTimer, maxEnemyRespawnTimer); // Set the next spawn time to a random time between the min and max.

            currentEnemies++; // Add 1 to the current enemies alive.

            //spawn random enemy to random spawn point
            GameObject spawnedEnemy = ObjectPooler.SharedInstance.GetPooledObject(RandomEnemy());
            if (spawnedEnemy != null)
            {
                Transform enemy1Spawn = enemySpawns[Random.Range(0, enemySpawns.Length)];

                spawnedEnemy.transform.position = enemy1Spawn.position;
                spawnedEnemy.transform.rotation = enemy1Spawn.rotation;
                spawnedEnemy.SetActive(true);
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
            return "basicEnemy";
        }
    }

    public void CheckIfLevelCompleted()
    {
        if (enemiesToKill <= 0)
        {
            levelCompleted = true;
        }

        if (currentEnemies <= 0 && enemiesToKill <= 0)
        {
            SceneManager.LoadScene("UpdateSelection");
           
        }
    }

    public void KillPlayer(GameObject player)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Player playerScript = player.GetComponent<Player>();

        if (upgradeControllerSO.secondLife)
        {
            upgradeControllerSO.secondLife = false;
        } 
        else
        {
            player.SetActive(false);
            SceneManager.LoadScene("EndMenu");
        }
    }

    public void KillEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        currentEnemies--;
        enemiesToKill--;

        CheckIfLevelCompleted();

        //ScreenShakeController.instance.StartShake(.1f, .1f);
    }

    public void KillBoss()
    {
        StartCoroutine(LoadFinalScene(2));
    }

    IEnumerator LoadFinalScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("WinMenu");
    }

}
