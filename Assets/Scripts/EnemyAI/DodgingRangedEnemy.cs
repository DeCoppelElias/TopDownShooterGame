using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DashAbility))]
public class DodgingRangedEnemy : RangedEnemy
{
    private DashAbility dashAbility;

    public override void StartEntity()
    {
        base.StartEntity();

        dashAbility = GetComponent<DashAbility>();
    }

    public override void UpdateEntity()
    {
        base.UpdateEntity();

        Vector2 lookDir = player.transform.position - gameObject.transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        gameObject.GetComponent<Rigidbody2D>().rotation = angle;

        Vector3 raycastDirection = player.transform.position - transform.position;
        RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));

        if (bulletTrigger)
        {
            int randomint = Random.Range(1, 4);
            Vector3 dashDirection = (Quaternion.Euler(0, 0, 90) * bulletDirection).normalized;
            if (randomint == 1)
            {
                dashDirection = (Quaternion.Euler(0, 0, -90) * bulletDirection).normalized;
            }
            dashAbility.Dash(dashDirection);
            bulletTrigger = false;
        }
        else if (Vector2.Distance(gameObject.transform.position, player.transform.position) > GetRange() || RaycastContainsWall(rays))
        {
            WalkToPlayer();
        }
    }
}
