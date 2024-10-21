using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DashAbility))]
public class ChargingMeleeEnemy : Enemy
{
    private DashAbility dashAbility;

    public override void StartEntity()
    {
        base.StartEntity();

        dashAbility = GetComponent<DashAbility>();
    }
    private void FixedUpdate()
    {
        if(Vector2.Distance(gameObject.transform.position, player.transform.position) < dashAbility.GetDashingDistance() / 2f)
        {
            Vector3 dashDirection = player.transform.position - transform.position;
            dashAbility.Dash(dashDirection);
        }
        else
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
        }
    }
}
