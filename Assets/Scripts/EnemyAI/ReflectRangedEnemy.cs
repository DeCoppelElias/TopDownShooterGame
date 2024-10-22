using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectRangedEnemy : RangedEnemy
{
    public float lastUse = 0;
    public float cooldown = 5;
    public override void UpdateEntity()
    {
        base.UpdateEntity();

        if (Time.timeScale > 0)
        {
            if (Vector2.Distance(gameObject.transform.position, player.transform.position) <= GetRange())
            {
                Vector3 raycastDirection = player.transform.position - transform.position;
                RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));
                if (!RaycastContainsWall(rays) && Time.time - lastShot > 1 / attackSpeed)
                {
                    GetComponent<ShootingAbility>().TryShootOnce();
                    lastShot = Time.time;
                }
            }
            if (bulletTrigger && Time.time > lastUse + cooldown)
            {
                GetComponent<ReflectShieldAbility>().EnableReflectShield();
                lastUse = Time.time;
            }
            else if (bulletTrigger)
            {
                bulletTrigger = false;
            }
        }
    }
}
