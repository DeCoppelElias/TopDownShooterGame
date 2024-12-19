using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    private float range;
    private SpriteRenderer enemySprite;

    private bool playerInRange = false;
    private float startPlayerInRange = 0;
    private float aimingDuration = 0.5f;
    public override void StartEntity()
    {
        base.StartEntity();

        range = GetComponent<ShootingAbility>().range;
        enemySprite = transform.Find("EnemySprite").GetComponent<SpriteRenderer>();
    }

    private void LookAtPlayer()
    {
        Vector2 lookDir = (player.transform.position - gameObject.transform.position).normalized;
        enemySprite.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);
    }

    public override void UpdateEntity()
    {
        base.UpdateEntity();

        LookAtPlayer();

        // Walk to player
        Vector3 raycastDirection = player.transform.position - transform.position;
        RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));
        if (Vector2.Distance(gameObject.transform.position, player.transform.position) > range || RaycastContainsObstacle(rays))
        {
            WalkToPlayerUpdate();
        }

        // Shoot at player
        else if (Vector2.Distance(gameObject.transform.position, player.transform.position) <= range)
        {
            if (playerInRange && Time.time - startPlayerInRange > aimingDuration)
            {
                if (!RaycastContainsObstacle(rays))
                {
                    GetComponent<ShootingAbility>().TryShootOnce();
                }
            }
            else if (!playerInRange)
            {
                playerInRange = true;
                startPlayerInRange = Time.time;
            }
        }
        else
        {
            playerInRange = false;
        }
    }

    public float GetRange()
    {
        return range;
    }
}
