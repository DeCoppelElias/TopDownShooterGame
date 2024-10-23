using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShootingAbility))]
public class SniperAbility : ClassAbility
{
    private GameStateManager gameStateManager;
    private ShootingAbility shootingAbility;

    private void Start()
    {
        this.gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        this.shootingAbility = GetComponent<ShootingAbility>();
    }
    public override void PerformAbility()
    {
        gameStateManager.ToSlowmo(0.05f);

        shootingAbility.workWithRealTime = true;

        StartCoroutine(StopSlowmoAfterDelay(3));
    }

    private IEnumerator StopSlowmoAfterDelay(int delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        shootingAbility.workWithRealTime = false;

        gameStateManager.ToRunning();
    }
}
