using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(DashAbility))]
public class DodgingMeleeEnemy : MeleeEnemy
{
    private DashAbility dashAbility;

    public override void StartEntity()
    {
        base.StartEntity();

        dashAbility = GetComponent<DashAbility>();
    }
    private void FixedUpdate()
    {
        if (bulletTrigger)
        {
            int randomint = Random.Range(1, 4);
            Vector3 dashDirection = (Quaternion.Euler(0, 0, 120) * bulletDirection).normalized;
            if (randomint == 1)
            {
                dashDirection = (Quaternion.Euler(0, 0, -120) * bulletDirection).normalized;
            }
            dashAbility.Dash(dashDirection);
            bulletTrigger = false;
        }
        else
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
        }
    }
}
