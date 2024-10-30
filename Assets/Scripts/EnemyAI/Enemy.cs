using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public Vector3 bulletDirection;
    public bool bulletTrigger;

    public float dodingRange = 1;
    public bool dodgingObstacle = false;
    public List<Vector3> dodgeObstaclePath;

    public Player player;

    private float refreshPlayerTargetCooldown = 2;
    private float lastRefresh = 0;

    public override void StartEntity()
    {
        base.StartEntity();

        player = FindClosestPlayer();
    }

    private Player FindClosestPlayer()
    {
        lastRefresh = Time.time;
        Player[] players = Object.FindObjectsOfType<Player>();

        Player closestPlayer = null;
        float closestDistance = float.MaxValue;

        foreach (Player player in players)
        {
            float distance = Vector3.Distance(this.transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    public override void UpdateEntity()
    {
        base.UpdateEntity();

        if (Time.time - lastRefresh > refreshPlayerTargetCooldown)
        {
            this.player = FindClosestPlayer();
        }
    }

    public void WalkToPlayer()
    {
        if (player == null) return;

        Vector3 raycastDirection = (player.transform.position - transform.position).normalized;
        float step = moveSpeed * Time.deltaTime;
        RaycastHit2D[] raysLong = Physics2D.RaycastAll(transform.position, raycastDirection, Vector3.Distance(transform.position, player.transform.position));
        if (!RaycastContainsWall(raysLong))
        {
            dodgingObstacle = false;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
        }
        else
        {
            if (dodgingObstacle)
            {
                if (dodgeObstaclePath.Count == 0)
                {
                    dodgingObstacle = false;
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
                }
                else if (Vector3.Distance(transform.position, dodgeObstaclePath[0]) <= 0.71f)
                {
                    dodgeObstaclePath.RemoveAt(0);
                }
                if (dodgeObstaclePath.Count > 0)
                {
                    transform.position = Vector2.MoveTowards(transform.position, dodgeObstaclePath[0], step);
                }
            }
            else
            {
                RaycastHit2D[] raysShort = Physics2D.RaycastAll(transform.position, raycastDirection, dodingRange);
                if (RaycastContainsWall(raysShort))
                {
                    dodgingObstacle = true;
                    PathFinding pathFinder = GetComponent<PathFinding>();
                    dodgeObstaclePath = pathFinder.FindPathDepthFirstObstacle(transform.position, player.transform.position);
                    if (dodgeObstaclePath.Count > 0) transform.position = Vector2.MoveTowards(transform.position, dodgeObstaclePath[0], step);
                }
                else
                {
                    dodgingObstacle = false;
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
                }
            }
        }
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
}
