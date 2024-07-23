using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private int gridWidth = 3;
    [SerializeField] private int gridHeight = 3;
    [SerializeField] private float cellWidth = 1f;
    [SerializeField] private float cellHeight = 1f;

    [SerializeField] public Vector2 gridOrigin = new Vector2(-8.5f, -4.5f); // Grid origin in the scene

    [SerializeField] public bool generatePath;
    [SerializeField] public bool visualiseGrid;

    public bool pathGenerated;

    public Dictionary<Vector2, Cell> cells;

    [SerializeField] public List<Vector2> cellsToSearch;
    [SerializeField] public List<Vector2> searchedCells;
    [SerializeField] public List<Vector2> finalPath;

    public List<GameObject> obstacles = new List<GameObject>();


    /*  private void Awake()
      {
          GenerateGrid();
          FindPath(new Vector2(0, 1), new Vector2(5, 7));
          pathGenerated = true;
      } */



    /* private void Update()
       {

           if (generatePath && !pathGenerated)
           {
               GenerateGrid();
             //FindPath(new Vector2(0, 1), new Vector2(5, 7));
            // FindPath(new Vector2(0, 1), targetGridPos);
             pathGenerated = true;
           }
           else if (!generatePath)
           {
               pathGenerated = false;
           }
       }  */

    private void Awake()
    {
        GenerateGrid();
    }


    public void FindPath(Vector2 startPos, Vector2 endPos)
    {
        searchedCells = new List<Vector2>();
        cellsToSearch = new List<Vector2> { startPos };
        finalPath = new List<Vector2>();

        Cell startCell = cells[startPos];
        startCell.gCost = 0;
        startCell.hCost = GetDistance(startPos, endPos);
        startCell.fCost = GetDistance(startPos, endPos);

        while (cellsToSearch.Count > 0)
        {
            Vector2 cellToSearch = cellsToSearch[0];

            foreach (Vector2 pos in cellsToSearch)
            {
                Cell c = cells[pos];
                if (c.fCost < cells[cellToSearch].fCost ||
                    c.fCost == cells[cellToSearch].fCost && c.hCost == cells[cellToSearch].hCost)
                {
                    cellToSearch = pos;
                }
            }

            cellsToSearch.Remove(cellToSearch);
            searchedCells.Add(cellToSearch);

            if (cellToSearch == endPos)
            {
                Cell pathCell = cells[endPos];

                while (pathCell.gridPosition != startPos)
                {
                    finalPath.Add(pathCell.gridPosition);
                    pathCell = cells[pathCell.connection];
                }

                finalPath.Add(startPos);

                return;
            }

            SearchCellNeighbors(cellToSearch, endPos);
        }
    }

    private void SearchCellNeighbors(Vector2 cellPos, Vector2 endPos)
    {
        for (float x = cellPos.x - cellWidth; x <= cellWidth + cellPos.x; x += cellWidth)
        {
            for (float y = cellPos.y - cellHeight; y <= cellHeight + cellPos.y; y += cellHeight)
            {
                Vector2 neighborPos = new Vector2(x, y);
                if (cells.TryGetValue(neighborPos, out Cell c) && !searchedCells.Contains(neighborPos) && !cells[neighborPos].isWall)
                {
                    int gCostToNeighbor = cells[cellPos].gCost + GetDistance(cellPos, neighborPos);

                    if (gCostToNeighbor < cells[neighborPos].gCost)
                    {
                        Cell neighborNode = cells[neighborPos];

                        neighborNode.connection = cellPos;
                        neighborNode.gCost = gCostToNeighbor;
                        neighborNode.hCost = GetDistance(neighborPos, endPos);
                        neighborNode.fCost = neighborNode.gCost + neighborNode.hCost;

                        if (!cellsToSearch.Contains(neighborPos))
                        {
                            cellsToSearch.Add(neighborPos);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        Vector2Int dist = new Vector2Int(Mathf.Abs((int)pos1.x - (int)pos2.x), Mathf.Abs((int)pos1.y - (int)pos2.y));
        int lowest = Mathf.Min(dist.x, dist.y);
        int highest = Mathf.Max(dist.x, dist.y);

        int horizontalMovesRequired = highest - lowest;

        return lowest * 14 + horizontalMovesRequired * 10;
    }

    private void OnDrawGizmos()
    {
         if (!visualiseGrid || cells == null)
         {
             return;
         } 

        foreach (KeyValuePair<Vector2, Cell> kvp in cells)
        {
            if (!kvp.Value.isWall)
            {
                //Gizmos.color = Color.white;
                Color newColor = Color.white;
                newColor.a = 0.5f;
                Gizmos.color = newColor;
            }
            else
            {
                Gizmos.color = Color.black;
              /*  Color newColor = Color.black;
                newColor.a = 0.5f;
                Gizmos.color = newColor; */
            }

            if (finalPath.Contains(kvp.Key))
            {
                //Gizmos.color = Color.magenta;
                Color newColor = Color.magenta;
                newColor.a = 0.5f;
                Gizmos.color = newColor;
            }

            Gizmos.DrawCube(kvp.Key + (Vector2)transform.position, new Vector3(cellWidth, cellHeight));
        }
    } 

    


public void GenerateGrid()
    {
        cells = new Dictionary<Vector2, Cell>();

        for (float x = 0; x < gridWidth; x += cellWidth)
        {
            for (float y = 0; y < gridHeight; y += cellHeight)
            {
                Vector2 gridPosition = new Vector2(x, y);
                Vector2 realWorldPosition = GetRealWorldPosition(gridPosition);
                cells.Add(gridPosition, new Cell(gridPosition, realWorldPosition));
               // Debug.Log($"Grid position: {gridPosition}, Real-world position: {realWorldPosition}");
            }
        }

        

        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePosition = new Vector2(obstacle.transform.position.x, obstacle.transform.position.y);

            foreach (KeyValuePair<Vector2, Cell> entry in cells)
            {
                if (entry.Value.realWorldPosition == obstaclePosition)
                {
                    entry.Value.isWall = true;

                    Debug.Log("we found a wall");

                    break; // Exit the inner loop once a match is found
                }
            }
        }
    }

    
    public Vector2 GetRealWorldPosition(Vector2 gridPosition)
    {
        float realX = gridOrigin.x + (gridPosition.x * cellWidth);
        float realY = gridOrigin.y + (gridPosition.y * cellHeight);
        return new Vector2(realX, realY);
    }

    public Vector2 GetGridPositionFromWorldPosition(Vector2 worldPosition)
    {
        float gridX = (worldPosition.x - gridOrigin.x) / cellWidth;
        float gridY = (worldPosition.y - gridOrigin.y) / cellHeight;
        return new Vector2(Mathf.Floor(gridX), Mathf.Floor(gridY));
    }

    private void MarkWalls()
    {
        foreach (GameObject obj in obstacles)
        {
            Vector2 objPosition = new Vector2(obj.transform.position.x, obj.transform.position.y);

            foreach (KeyValuePair<Vector2, Cell> entry in cells)
            {
                if (entry.Value.realWorldPosition == objPosition)
                {
                    entry.Value.isWall = true;
                    break; // Exit the inner loop once a match is found
                }
            }
        }
    }

    public class Cell
    {
        public Vector2 gridPosition;
        public Vector2 realWorldPosition;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = int.MaxValue;
        public Vector2 connection;
        public bool isWall;

        public bool isTarget; // Added this line

        /*  public Cell(Vector2 pos)
          {
              gridPosition = pos;
          } */

        public Cell(Vector2 gridPos, Vector2 realWorldPos)
        {
            gridPosition = gridPos;
            realWorldPosition = realWorldPos;
        }
    }
}






