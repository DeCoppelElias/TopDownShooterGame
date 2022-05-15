using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private void FixedUpdate()
    {
        WalkToPlayer();
    }
}
