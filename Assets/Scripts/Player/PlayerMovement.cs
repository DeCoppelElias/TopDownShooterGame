using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    private float currentMoveSpeed;
    private Vector2 currentMoveDirection = Vector2.zero;

    private Vector2 currentLookInput = Vector2.zero;

    private Player player;
    private Rigidbody2D playerRB;
    private PlayerInput playerInput;

    private DashAbility dashAbility;
    private ShootingAbility shootAbility;

    private SpriteRenderer spriteRenderer;

    private bool knockback = false;
    private Vector3 knockbackDirection;
    private float knockbackSpeed = 10;
    private float knockbackStart = 0;
    private float knockbackDuration = 1;

    [SerializeField] private Camera customCamera;

    private void Start()
    {
        this.player = this.GetComponent<Player>();
        this.playerRB = this.GetComponent<Rigidbody2D>();
        this.playerInput = this.GetComponent<PlayerInput>();

        this.dashAbility = this.GetComponent<DashAbility>();
        this.shootAbility = this.GetComponent<ShootingAbility>();

        this.currentMoveSpeed = player.moveSpeed;

        this.spriteRenderer = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
    }

    private void Move()
    {
        // Don't move if dashing
        if (dashAbility != null && (dashAbility.dashingState == DashAbility.DashingState.Dashing || dashAbility.dashingState == DashAbility.DashingState.Charging)) return;
        
        // If knockback, move in knockback direction
        if (knockback)
        {
            if (Time.time - knockbackStart > knockbackDuration || playerRB.velocity == Vector2.zero)
            {
                knockback = false;
                playerRB.velocity = Vector2.zero;
            }
            else
            {
                playerRB.velocity = knockbackDirection * knockbackSpeed;
            }
        }
        else
        {
            // Slow down if shooting
            if (shootAbility != null && shootAbility.shooting)
            {
                this.currentMoveSpeed = shootAbility.shootingMoveSpeed;
            }
            else
            {
                this.currentMoveSpeed = player.moveSpeed;
            }

            playerRB.velocity = currentMoveDirection * currentMoveSpeed;
        }
    }

    public void Look()
    {
        // Handle input depending on controller or mouse input
        if (Gamepad.current != null && Gamepad.current.enabled && playerInput.currentControlScheme == "Gamepad" && currentLookInput != Vector2.zero)
        {
            spriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, currentLookInput);
        }
        else if (Mouse.current != null && Mouse.current.enabled && playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldMousePosition = (customCamera != null) ? customCamera.ScreenToWorldPoint(mousePosition) : Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 lookDirection = (worldMousePosition - transform.position);
            spriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        }
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.currentMoveDirection = moveDirection.normalized;
    }
    public void SetLookInput(Vector3 lookInput)
    {
        this.currentLookInput = lookInput;
    }

    public void ApplyKnockBack(Vector3 knockbackDirection, float knockbackSpeed, float knockbackRange)
    {
        this.knockbackDirection = knockbackDirection;
        this.knockbackSpeed = knockbackSpeed;
        this.knockbackDuration = knockbackRange / knockbackSpeed;
        this.knockbackStart = Time.time;
        playerRB.velocity = knockbackDirection * knockbackSpeed;
        this.knockback = true;
    }
}
