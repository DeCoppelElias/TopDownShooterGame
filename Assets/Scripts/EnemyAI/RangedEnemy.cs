using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public float lastShot = 0;
    private float range;
    private SpriteRenderer enemySprite;

    private bool playerInRange = false;
    private float startPlayerInRange = 0;
    private float aimingDuration = 0.5f;
    public override void StartEntity()
    {
        range = GetComponent<ShootingAbility>().range;
        enemySprite = transform.Find("EnemySprite").GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        Vector2 lookDir = (player.transform.position - gameObject.transform.position).normalized;
        enemySprite.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);

        Vector3 raycastDirection = player.transform.position - transform.position;
        RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));
        if (Vector2.Distance(gameObject.transform.position, player.transform.position) > range || RaycastContainsWall(rays))
        {
            WalkToPlayer();
        }
    }

    public override void UpdateEntity()
    {
        if (Time.timeScale > 0)
        {
            if (Vector2.Distance(gameObject.transform.position, player.transform.position) <= range)
            {
                if (playerInRange && Time.time - startPlayerInRange > aimingDuration)
                {
                    Vector3 raycastDirection = player.transform.position - transform.position;
                    RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));
                    if (!RaycastContainsWall(rays) && Time.time - lastShot > 1 / attackSpeed)
                    {
                        GetComponent<ShootingAbility>().TryShootOnce();
                        lastShot = Time.time;
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
    }

    public float GetRange()
    {
        return range;
    }
}
