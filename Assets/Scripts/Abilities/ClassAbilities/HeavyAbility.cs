using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(ShootingAbility), typeof(Player), typeof(PlayerMovement))]
public class HeavyAbility : ClassAbility
{
    private ShootingAbility shootingAbility;
    private Player player;
    private PlayerMovement playerMovement;
    private CameraManager cameraManager;
    protected override void PerformAbilitySpecific()
    {
        shootingAbility.ShootBullet(10, 2, 5, 10, player.damage * 2);

        // Add knockback after shooting
        Vector3 knockbackDirection = (this.transform.position - shootingAbility.firePoint.position).normalized;
        playerMovement.ApplyKnockBack(knockbackDirection, 8, 1.5f);

        // Add screen shake after use
        if (this.cameraManager != null) cameraManager.ShakeScreen();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.shootingAbility = GetComponent<ShootingAbility>();
        this.player = GetComponent<Player>();
        this.playerMovement = GetComponent<PlayerMovement>();
        GameObject cameraManagerObj = GameObject.Find("CameraManager");
        if (cameraManagerObj != null) this.cameraManager = cameraManagerObj.GetComponent<CameraManager>();
    }
}
