using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingMeleeEnemy : Enemy
{
    private DashAbility.DashingState dashingState = DashAbility.DashingState.Ready;
    private Vector3 dashDirection;
    public float dashCooldown = 0;
    private float lastUse = 0;
    private void FixedUpdate()
    {
        if(Vector2.Distance(gameObject.transform.position, player.transform.position) < GetComponent<DashAbility>().dashSpeed * GetComponent<DashAbility>().dashDuration / 2 && Time.time > lastUse + dashCooldown && dashingState == DashAbility.DashingState.Ready)
        {
            dashDirection = player.transform.position - transform.position;
            dashingState = GetComponent<DashAbility>().Dash(dashDirection);
            lastUse = Time.time;
        }
        else if(dashingState == DashAbility.DashingState.Charging || dashingState == DashAbility.DashingState.Dashing)
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
