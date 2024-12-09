using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    private HashSet<Vector3> visited;
    private float size = 0;
    private float stepSize = 0.25f;

    [Header("Debug Settings")]
    [SerializeField] private LineRenderer debugLineRenderer;

    private void Start()
    {
        Collider2D collider = GetComponentInChildren<Collider2D>();
        if (collider != null)
        {
            size = collider.bounds.size.x;
        }

        // Debug
        debugLineRenderer = GameObject.Find("DebugLineRenderer")?.GetComponent<LineRenderer>();
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
            this.priority = priority;
        }

        public int CompareTo(GridTile other)
        {
            if (this.priority < other.priority) return -1;
            else if (this.priority > other.priority) return 1;
            else return 0;
        }
    }

    public List<Vector3> FindDepthFirstPath(Vector3 from, Vector3 to)
    {
        visited = new HashSet<Vector3>();
        GridTile fromGT = new GridTile(from, null, Vector3.Distance(from, to));
        GridTile GT = DepthFirstHelper(fromGT, to);
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

    private GridTile DepthFirstHelper(GridTile from, Vector3 to, int counter=0)
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
            GridTile solution = DepthFirstHelper(gridTile, to, counter+1);
            if (solution != null)
            {
                return solution;
            }
        }
        return null;
    }

    /// <summary>
    /// Find a depth first path untill the final position is visible (no walls in between).
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Vector3> FindPathDepthFirstVisibility(Vector3 from, Vector3 to)
    {
        visited = new HashSet<Vector3>();
        GridTile fromGT = new GridTile(from, null, Vector3.Distance(from, to));
        GridTile GT = DepthFirstVisibilityHelper(fromGT, to);
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

    private GridTile DepthFirstVisibilityHelper(GridTile from, Vector3 to, int counter = 0)
    {
        if (counter > 100)
        {
            Debug.Log("Did not find path!");
            return null;
        }

        List<Vector3> neighbours = GetNeighbours(from.location);
        List<GridTile> activeTiles = new List<GridTile>();
        foreach (Vector3 neighbour in neighbours)
        {
            GridTile gridTile = new GridTile(neighbour, from, Vector3.Distance(neighbour, to));
            if (!IsWallInBetween(gridTile.location, to) || Vector3.Distance(gridTile.location, to) < 0.4f * size)
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
            GridTile solution = DepthFirstVisibilityHelper(gridTile, to, counter + 1);
            if (solution != null)
            {
                return solution;
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

    public void DisplayRoute(List<Vector3> route)
    {
        if (debugLineRenderer != null)
        {
            debugLineRenderer.positionCount = route.Count;
            debugLineRenderer.SetPositions(route.ToArray());
        }
    }

    public List<Vector3> SmoothRoute(List<Vector3> route)
    {
        List<Vector3> newRoute = new List<Vector3>();
        if (route.Count <= 2) return route;

        newRoute.Add(route[0]);
        int i = 0;
        while (i < route.Count - 2)
        {
            int j = i + 2;

            while (j < route.Count && !IsWallInBetween(route[i], route[j]))
            {
                j++;
            }
            newRoute.Add(route[j - 1]);
            i = j - 1;
        }

        return newRoute;
    }

    public bool IsWallInBetween(Vector3 from, Vector3 to)
    {
        Vector3 raycastDirection = (to - from).normalized;
        RaycastHit2D[] rays = Physics2D.RaycastAll(from, raycastDirection, Vector3.Distance(from, to));
        if (RaycastContainsWall(rays)) return true;

        // Perpendicular vectors
        Vector3 perpendicular1 = new Vector3(-raycastDirection.y, raycastDirection.x, raycastDirection.z);
        Vector3 perpendicular2 = new Vector3(raycastDirection.y, -raycastDirection.x, raycastDirection.z);

        Vector3 newFrom1 = from + (size * perpendicular1);
        raycastDirection = (to - newFrom1).normalized;
        rays = Physics2D.RaycastAll(newFrom1, raycastDirection, Vector3.Distance(newFrom1, to));
        if (RaycastContainsWall(rays)) return true;

        Vector3 newFrom2 = from + (size * perpendicular2);
        raycastDirection = (to - newFrom2).normalized;
        rays = Physics2D.RaycastAll(newFrom2, raycastDirection, Vector3.Distance(newFrom2, to));
        if (RaycastContainsWall(rays)) return true;

        return false;
    }

    public List<Vector3> GetNeighbours(Vector3 location)
    {
        List<Vector3> neighbours = new List<Vector3>();

        Vector3 right = location + new Vector3(stepSize, 0, 0);
        if (!LocationHasWall(right) && !visited.Contains(right))
        {
            neighbours.Add(right);
        }
        Vector3 left = location + new Vector3(-stepSize, 0, 0);
        if (!LocationHasWall(left) && !visited.Contains(left))
        {
            neighbours.Add(left);
        }
        Vector3 up = location + new Vector3(0, stepSize, 0);
        if (!LocationHasWall(up) && !visited.Contains(up))
        {
            neighbours.Add(up);
        }
        Vector3 down = location + new Vector3(0, -stepSize, 0);
        if (!LocationHasWall(down) && !visited.Contains(down))
        {
            neighbours.Add(down);
        }
        return neighbours;
    }

    public bool LocationHasWall(Vector3 location)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(location, 0.35f * size);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall")) return true;
        }
        return false;
        
        /*TileBase wall = walls.GetTile(Vector3Int.FloorToInt(location));
        if(wall == null)
        {
            return false;
        }
        return true;*/
    }
}

