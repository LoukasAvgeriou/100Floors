using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSceneController : MonoBehaviour
{
    // Assign your prefab in the inspector
   // public GameObject prefab;

    // List of transforms where the prefab will be spawned
    public List<Transform> spawnPoints;

    // Number of cycles
    public int numberOfCycles = 3;

    // Delay between spawns
    public float delayBetweenSpawns = 2f;

    void Start()
    {
        // Start the spawning process
        StartCoroutine(SpawnPrefabs());
    }

    IEnumerator SpawnPrefabs()
    {
        // Loop through the number of cycles
        for (int cycle = 0; cycle < numberOfCycles; cycle++)
        {
            // Loop through each spawn point
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                // Spawn the prefab at the current spawn point
                GameObject drone = ObjectPooler.SharedInstance.GetPooledObject("drone");
                if (drone != null)
                {
                    drone.transform.position = spawnPoints[i].transform.position;
                    drone.transform.rotation = spawnPoints[i].transform.rotation;
                    drone.SetActive(true);
                }

                // Wait for the specified delay before spawning the next prefab
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
    }


}
