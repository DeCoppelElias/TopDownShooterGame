using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingRangedEnemy : RangedEnemy
{
    public DashAbility.DashingState dashingState = DashAbility.DashingState.Ready;
    public Vector3 dashDirection;
    public float dashCooldown = 0;
    private float lastUse;

    private void FixedUpdate()
    {
        Vector2 lookDir = player.transform.position - gameObject.transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        gameObject.GetComponent<Rigidbody2D>().rotation = angle;

        Vector3 raycastDirection = player.transform.position - transform.position;
        RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, raycastDirection, Vector2.Distance(transform.position, player.transform.position));

        if (bulletTrigger && dashingState == DashAbility.DashingState.Ready && Time.time > lastUse + dashCooldown)
        {
            int randomint = Random.Range(1, 4);
            dashDirection = (Quaternion.Euler(0, 0, 90) * bulletDirection).normalized;
            if (randomint == 1)
            {
                dashDirection = (Quaternion.Euler(0, 0, -90) * bulletDirection).normalized;
            }
            dashingState = GetComponent<DashAbility>().Dash(dashDirection);
            bulletTrigger = false;
            lastUse = Time.time;
        }
        else if(bulletTrigger && Time.time <= lastUse + dashCooldown)
        {
            bulletTrigger = false;
        }
        else if(dashingState == DashAbility.DashingState.Dashing || dashingState == DashAbility.DashingState.Charging)
        {
            dashingState = GetComponent<DashAbility>().Dash(dashDirection);
        }
        else if (Vector2.Distance(gameObject.transform.position, player.transform.position) > GetRange() || RaycastContainsWall(rays))
        {
            WalkToPlayer();
        }
    }
}
