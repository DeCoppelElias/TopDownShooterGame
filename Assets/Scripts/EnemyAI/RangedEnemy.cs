using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public float lastShot = 0;
    private float range;
    public override void StartEntity()
    {
        range = GetComponent<ShootingAbility>().range;
    }
    private void FixedUpdate()
    {
        Vector2 lookDir = player.transform.position - gameObject.transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        gameObject.GetComponent<Rigidbody2D>().rotation = angle;

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
                Vector3 raycastDirection = player.transform.position - transform.position;
                RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));
                if (!RaycastContainsWall(rays) && Time.time - lastShot > 1 / attackSpeed)
                {
                    GetComponent<ShootingAbility>().Shoot();
                    lastShot = Time.time;
                }
            }
        }
    }

    public float GetRange()
    {
        return range;
    }
}
