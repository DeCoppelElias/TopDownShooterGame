using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    private Tilemap walls;
    private HashSet<Vector3> visited;

    private void Start()
    {
        walls = GameObject.Find("Walls").GetComponent<Tilemap>();
    }
    public class GridTile : IComparable<GridTile>
    {
        public GridTile previous;
        public Vector3 location;
        public float priority;

        public GridTile(Vector3 location, GridTile previous, float priority)
        {
            this.previous = previous;
            this.location = location;
            //this.location = Vector3Int.FloorToInt(location) + new Vector3(0.5f,0.5f,0);
            this.priority = priority;
        }

        public int CompareTo(GridTile other)
        {
            if (this.priority < other.priority) return -1;
            else if (this.priority > other.priority) return 1;
            else return 0;
        }
    }

    public List<Vector3> FindPathDepthFirst(Vector3 from, Vector3 to)
    {
        visited = new HashSet<Vector3>();
        GridTile fromGT = new GridTile(from, null, Vector3.Distance(from, to));
        GridTile GT = FindPathDepthFirstGridTile(fromGT, to);
        List<Vector3> result = new List<Vector3>();
        GridTile currentGT = GT;
        while (currentGT != null)
        {
            result.Add(currentGT.location);
            currentGT = currentGT.previous;
        }
        result.Reverse();
        return result;
    }

    private GridTile FindPathDepthFirstGridTile(GridTile from, Vector3 to, int counter=0)
    {
        if (counter > 100) return null;

        List<Vector3> neighbours = GetNeighbours(from.location);
        List<GridTile> activeTiles = new List<GridTile>();
        foreach (Vector3 neighbour in neighbours)
        {
            GridTile gridTile = new GridTile(neighbour, from, Vector3.Distance(neighbour, to));
            if (gridTile.location == to)
            {
                return gridTile;
            }
            else
            {
                activeTiles.Add(gridTile);
                visited.Add(neighbour);
            }
        }
        activeTiles.Sort();
        foreach (GridTile gridTile in activeTiles)
        {
            GridTile solution = FindPathDepthFirstGridTile(gridTile, to, counter+1);
            if (solution != null)
            {
                return solution;
            }
        }
        return null;
    }

    public List<Vector3> FindPathDepthFirstObstacle(Vector3 from, Vector3 to)
    {
        visited = new HashSet<Vector3>();
        GridTile fromGT = new GridTile(from, null, Vector3.Distance(from, to));
        GridTile GT = FindPathDepthFirstGridTileObstacle(fromGT, to);
        List<Vector3> result = new List<Vector3>();
        GridTile currentGT = GT;
        while (currentGT != null)
        {
            result.Add(currentGT.location);
            currentGT = currentGT.previous;
        }
        result.Reverse();
        return result;
    }

    private GridTile FindPathDepthFirstGridTileObstacle(GridTile from, Vector3 to, int counter = 0)
    {
        if (counter > 100) return null;

        List<Vector3> neighbours = GetNeighbours(from.location);
        List<GridTile> activeTiles = new List<GridTile>();
        foreach (Vector3 neighbour in neighbours)
        {
            GridTile gridTile = new GridTile(neighbour, from, Vector3.Distance(neighbour, to));
            activeTiles.Add(gridTile);
            visited.Add(neighbour);
        }
        activeTiles.Sort();
        foreach (GridTile gridTile in activeTiles)
        {
            Vector3 raycastDirection = (to - gridTile.location).normalized;
            RaycastHit2D[] rays = Physics2D.RaycastAll(gridTile.location, raycastDirection, Vector3.Distance(to, gridTile.location));
            if (!RaycastContainsWall(rays))
            {
                return gridTile;
            }
            else
            {
                GridTile solution = FindPathDepthFirstGridTileObstacle(gridTile, to, counter+1);
                if (solution != null)
                {
                    return solution;
                }
            }
        }
        return null;
    }

    public bool RaycastContainsWall(RaycastHit2D[] rays)
    {
        foreach (RaycastHit2D ray in rays)
        {
            if (ray.transform.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector3> GetNeighbours(Vector3 location)
    {
        List<Vector3> neighbours = new List<Vector3>();

        Vector3 right = location + new Vector3(1, 0, 0);
        if (!LocationHasWall(right) && !visited.Contains(right))
        {
            neighbours.Add(right);
        }
        Vector3 left = location + new Vector3(-1, 0, 0);
        if (!LocationHasWall(left) && !visited.Contains(left))
        {
            neighbours.Add(left);
        }
        Vector3 up = location + new Vector3(0, 1, 0);
        if (!LocationHasWall(up) && !visited.Contains(up))
        {
            neighbours.Add(up);
        }
        Vector3 down = location + new Vector3(0, -1, 0);
        if (!LocationHasWall(down) && !visited.Contains(down))
        {
            neighbours.Add(down);
        }
        /*Vector3 upRight = location + new Vector3(1, 1, 0);
        if (!LocationHasWall(upRight) && !visited.Contains(upRight))
        {
            neighbours.Add(upRight);
        }
        Vector3 upLeft = location + new Vector3(-1, 1, 0);
        if (!LocationHasWall(upLeft) && !visited.Contains(upLeft))
        {
            neighbours.Add(upLeft);
        }
        Vector3 downRight = location + new Vector3(1, -1, 0);
        if (!LocationHasWall(downRight) && !visited.Contains(downRight))
        {
            neighbours.Add(downRight);
        }
        Vector3 downLeft = location + new Vector3(-1, -1, 0);
        if (!LocationHasWall(downLeft) && !visited.Contains(downLeft))
        {
            neighbours.Add(downLeft);
        }*/
        return neighbours;
    }

    public bool LocationHasWall(Vector3 location)
    {
        TileBase wall = walls.GetTile(Vector3Int.FloorToInt(location));
        if(wall == null)
        {
            return false;
        }
        return true;
    }
}

