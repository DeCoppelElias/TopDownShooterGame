using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DashAbility : MonoBehaviour
{
    public enum DashingState { Ready, Charging, Dashing, Cooldown };
    public DashingState dashingState = DashingState.Ready;

    public int dashCooldown = 2;
    public float dashDuration = 0.1f;
    public float chargeDuration = 0f;
    public float dashSpeed = 20;
    public float contactDamageIncrease = 5;

    private float currentDashSpeed = 0;

    private float dashStart = 0;
    private float chargeStart = 0;
    private float lastDash = 0;

    private Vector2 dashDirection;

    private Rigidbody2D rb;

    private Entity entity;

    public UnityEvent onPerformed;
    public UnityEvent onReady;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        entity = GetComponent<Entity>();
    }
    public void Dash(Vector2 direction)
    {
        if (dashingState != DashingState.Ready) return;
        if (direction == Vector2.zero) return;

        dashingState = DashingState.Charging;
        this.dashDirection = direction;
        chargeStart = Time.time;
    }

    public bool TryDash(Vector2 direction)
    {
        if (dashingState != DashingState.Ready) return false;
        if (direction == Vector2.zero) return false;

        dashingState = DashingState.Charging;
        this.dashDirection = direction;
        chargeStart = Time.time;
        return true;
    }

    private void Update()
    {
        if (dashingState == DashingState.Cooldown)
        {
            if (Time.time - lastDash > dashCooldown)
            {
                dashingState = DashingState.Ready;

                if (onReady != null)
                {
                    onReady.Invoke();
                }
            }
        }
        else if (dashingState == DashingState.Charging)
        {
            if (Time.time - chargeStart > chargeDuration)
            {
                dashingState = DashingState.Dashing;
                currentDashSpeed = dashSpeed;
                rb.velocity = dashDirection * currentDashSpeed;
                dashStart = Time.time;

                // Increase contact damage
                if (entity != null)
                {
                    entity.contactDamage *= contactDamageIncrease;
                }
            }
        }
        else if (dashingState == DashingState.Dashing)
        {
            if (Time.time - dashStart > dashDuration || rb.velocity == Vector2.zero)
            {
                dashingState = DashingState.Cooldown;
                lastDash = Time.time;

                if (onPerformed != null)
                {
                    onPerformed.Invoke();
                }

                // Decrease contact damage again
                if (entity != null)
                {
                    entity.contactDamage /= contactDamageIncrease;
                }
            }
            else
            {
                rb.velocity = dashDirection * currentDashSpeed;
            }
        }
    }

    public float GetDashingDistance()
    {
        return dashSpeed * dashDuration;
    }
}
