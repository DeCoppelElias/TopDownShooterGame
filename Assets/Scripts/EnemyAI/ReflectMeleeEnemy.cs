using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectMeleeEnemy : MeleeEnemy
{
    public float cooldown = 5;
    public float lastUse = 0;

    public override void UpdateEntity()
    {
        base.UpdateEntity();

        if (bulletTrigger && Time.time > lastUse + cooldown)
        {
            GetComponent<ReflectShieldAbility>().EnableReflectShield();
            lastUse = Time.time;
        }
        else if (bulletTrigger)
        {
            bulletTrigger = false;
        }
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, step);
    }
}
