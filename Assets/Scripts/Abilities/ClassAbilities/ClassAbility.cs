using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public abstract class ClassAbility : MonoBehaviour
{
    [SerializeField]
    private float cooldown = 2;
    private float lastUse = 0;

    public void PerformAbility()
    {
        if (Time.time - lastUse > cooldown)
        {
            lastUse = Time.time;
            PerformAbilitySpecific();
        }
    }
    protected abstract void PerformAbilitySpecific();
}
