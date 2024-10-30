using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReflectShieldAbility))]
public class ReflectRangedEnemy : RangedEnemy
{
    private ReflectShieldAbility reflectShieldAbility; 
    public override void StartEntity()
    {
        base.StartEntity();

        reflectShieldAbility = GetComponent<ReflectShieldAbility>();
    }
    public override void UpdateEntity()
    {
        base.UpdateEntity();

        if (bulletTrigger) reflectShieldAbility.EnableReflectShield();
        else if (bulletTrigger)
        {
            bulletTrigger = false;
        }
    }
}
