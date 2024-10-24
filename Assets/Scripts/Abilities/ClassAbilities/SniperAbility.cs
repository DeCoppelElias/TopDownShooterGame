using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(ShootingAbility))]
public class SniperAbility : ClassAbility
{
    private GameStateManager gameStateManager;
    private ShootingAbility shootingAbility;

    [SerializeField]
    private int abilityDuration = 3;
    [SerializeField]
    private float effectDuration = 0.5f;

    private void Start()
    {
        this.gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        this.shootingAbility = GetComponent<ShootingAbility>();
    }
    protected override void PerformAbilitySpecific()
    {
        gameStateManager.SlowDownTime(0.05f, effectDuration, abilityDuration);
        shootingAbility.workWithRealTime = true;

        StartCoroutine(PerformActionAfterRealDelay((2 * effectDuration) + abilityDuration, () => shootingAbility.workWithRealTime = false));
    }
    
    private IEnumerator PerformActionAfterRealDelay(float delay, Action action)
    {
        yield return new WaitForSecondsRealtime(delay);

        action();
    }
}
