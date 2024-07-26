using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSceneController : MonoBehaviour
{
    public List<GameObject> drones; // List of drones
    public float spawnInterval = 5.0f; // Interval between spawns
    private int currentIndex = 0; // Current index in the list

    void Start()
    {
        StartCoroutine(SpawnDrones());
    }

    IEnumerator SpawnDrones()
    {
        while (currentIndex < drones.Count)
        {
            ActivateDrone();
            currentIndex++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // Do something when the list is over
        OnSpawningComplete();
    }

    void ActivateDrone()
    {
        if (currentIndex < drones.Count)
        {
            drones[currentIndex].SetActive(true);
        }
    }

    void OnSpawningComplete()
    {
        // Add your desired action here
        Debug.Log("All objects have been spawned!");
    }

}
