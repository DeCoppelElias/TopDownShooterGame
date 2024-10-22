using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitMeleeEnemy : MeleeEnemy
{
    public float splitAmount = 0;
    public override void UpdateEntity()
    {
        base.UpdateEntity();

        if (health <= 0)
        {
            GetComponent<SplitAbility>().Split((player.transform.position - transform.position).normalized);
        }
    }
}
