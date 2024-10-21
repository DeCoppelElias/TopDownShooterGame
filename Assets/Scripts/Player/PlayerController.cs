using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player player;
    private Rigidbody2D playerRB;

    private PlayerMovement playerMovement;

    private DashAbility dashAbility;
    private ShootingAbility shootAbility;
    private ReflectShieldAbility reflectAbility;

    private void Start()
    {
        this.player = this.GetComponent<Player>();
        this.playerRB = this.GetComponent<Rigidbody2D>();

        this.playerMovement = this.GetComponent<PlayerMovement>();

        this.dashAbility = this.GetComponent<DashAbility>();
        this.shootAbility = this.GetComponent<ShootingAbility>();
        this.reflectAbility = this.GetComponent<ReflectShieldAbility>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (dashAbility == null) return;

        if (context.performed)
        {
            Vector3 moveDir = playerRB.velocity.normalized;
            dashAbility.Dash(moveDir);
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (shootAbility == null) return;

        if (context.performed)
        {
            shootAbility.StartShooting();
        }

        if (context.canceled)
        {
            shootAbility.StopShooting();
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (playerMovement == null) return;

        Vector2 lookInput = context.ReadValue<Vector2>();
        playerMovement.SetLookInput(lookInput);
    }

    public void Reflect(InputAction.CallbackContext context)
    {
        if (reflectAbility == null) return;

        if (context.performed)
        {
            reflectAbility.EnableReflectShield();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (playerMovement == null) return;

        Vector2 movementInput = Vector2.zero;
        if (context.performed)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            movementInput = Vector2.zero;
        }

        playerMovement.SetMoveDirection(movementInput);
    }
}
