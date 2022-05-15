using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    public enum DashingState { Ready, Charging, Dashing};
    public DashingState dashingState;
    public float dashDuration = 0.09f;
    public float chargeDuration = 0.5f;
    private float dashTime = 0;
    private float chargeTime = 0;
    public float dashSpeed = 6;
    public DashingState Dash(Vector3 dashDirection)
    {
        if (dashingState == DashingState.Ready)
        {
            dashingState = DashingState.Charging;
            chargeTime = Time.time;
        }
        else if (dashingState == DashingState.Charging)
        {
            if (Time.time > chargeTime + chargeDuration)
            {
                dashingState = DashingState.Dashing;
                dashTime = Time.time;
            }
        }
        else if (dashingState == DashingState.Dashing)
        {
            transform.position += dashSpeed * dashDirection * Time.fixedDeltaTime;
            if (Time.time > dashTime + dashDuration)
            {
                dashingState = DashingState.Ready;
            }
        }
        return dashingState;
    }
}
