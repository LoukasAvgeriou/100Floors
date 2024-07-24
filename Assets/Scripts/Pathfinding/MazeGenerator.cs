using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject pathPrefab;
    public Vector3 startPosition = Vector3.zero; // Added startPosition variable
    private int[,] maze;
    private List<Vector2Int> stack = new List<Vector2Int>();

    void Start()
    {
        maze = new int[width, height];
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        // Start the maze generation from the top left corner
        Vector2Int startPos = new Vector2Int(0, 0);
        stack.Add(startPos);
        maze[startPos.x, startPos.y] = 1;

        while (stack.Count > 0)
        {
            Vector2Int current = stack[stack.Count - 1];
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(current, chosen);
                stack.Add(chosen);
                maze[chosen.x, chosen.y] = 1;
            }
            else
            {
                stack.RemoveAt(stack.Count - 1);
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.x > 1 && maze[cell.x - 2, cell.y] == 0)
            neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.x < width - 2 && maze[cell.x + 2, cell.y] == 0)
            neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.y > 1 && maze[cell.x, cell.y - 2] == 0)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
        if (cell.y < height - 2 && maze[cell.x, cell.y + 2] == 0)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

        return neighbors;
    }

    void RemoveWall(Vector2Int current, Vector2Int chosen)
    {
        Vector2Int wall = current + (chosen - current) / 2;
        maze[wall.x, wall.y] = 1;
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (maze[x, y] == 1) ? pathPrefab : wallPrefab;
                Instantiate(prefab, startPosition + new Vector3(x, y, 0), Quaternion.identity); // Use startPosition for offset
            }
        }
    }
}


