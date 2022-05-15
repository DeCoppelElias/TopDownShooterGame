using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgingMeleeEnemy : MeleeEnemy
{
    private DashAbility.DashingState dashingState = DashAbility.DashingState.Ready;
    public Vector3 dashDirection;
    public float dashCooldown = 0;
    private float lastUse = 0;
    private void FixedUpdate()
    {
        if (bulletTrigger && dashingState == DashAbility.DashingState.Ready && Time.time > lastUse + dashCooldown)
        {
            int randomint = Random.Range(1, 4);
            dashDirection = (Quaternion.Euler(0, 0, 120) * bulletDirection).normalized;
            if (randomint == 1)
            {
                dashDirection = (Quaternion.Euler(0, 0, -120) * bulletDirection).normalized;
            }
            dashingState = GetComponent<DashAbility>().Dash(dashDirection);
            bulletTrigger = false;
            lastUse = Time.time;
        }
        else if (bulletTrigger && Time.time <= lastUse + dashCooldown)
        {
            bulletTrigger = false;
        }
        else if (dashingState == DashAbility.DashingState.Charging || dashingState == DashAbility.DashingState.Dashing)
        {
            dashingState = GetComponent<DashAbility>().Dash(dashDirection);
        }
        else
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
        }
    }
}
