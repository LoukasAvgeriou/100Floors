using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperimentalEnemy : MonoBehaviour
{
    public Pathfinding pathfinding; // Reference to the Pathfinding script
    public GameObject target; // Target to move towards
    public float speed = 2f; // Speed of movement
    public float respawnDelay = 2f; // Delay before respawning

    private List<Vector2> path; // The computed path
    private int currentPathIndex; // Current index in the path list

    public List<GameObject> spawnPoints = new List<GameObject>();

    public GameObject explosion;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        

        FindPath();
    }

    public void FindPath()
    {
        if (pathfinding == null || target == null)
        {
            Debug.LogError("Pathfinding script or target not assigned.");
            return;
        }

        Vector2 startPos = pathfinding.GetGridPositionFromWorldPosition(transform.position);
        Vector2 endPos = pathfinding.GetGridPositionFromWorldPosition(target.transform.position);

        // Find path and store it in the path variable
        pathfinding.FindPath(startPos, endPos);
        path = pathfinding.finalPath;

        // Reverse the path to start from the current position
        path.Reverse();

        // Start moving along the path
        if (path.Count > 0)
        {
            currentPathIndex = 0;
            StartCoroutine(MoveAlongPath());
        }
        else
        {
            Debug.Log("No path found.");

            gameObject.SetActive(false);

        }
    }

    private IEnumerator MoveAlongPath()
    {
        while (currentPathIndex < path.Count)
        {
            Vector2 targetPosition = path[currentPathIndex] + (Vector2)pathfinding.gridOrigin;

            // Move towards the target position
            while ((Vector2)transform.position != targetPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // Move to the next point in the path
            currentPathIndex++;
        }

        // Path completed
        Debug.Log("Path completed.");


        explosion.SetActive(true);
        gameObject.SetActive(false);
       

    }

    
    

}


