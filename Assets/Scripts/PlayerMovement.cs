using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movement;
    private Vector2 mousePos; 
    public DashAbility.DashingState dashingState = DashAbility.DashingState.Ready;
    public float dashCooldown = 1.5f;
    public float dashSpeed = 5;
    public float dashDuration = 0.09f;
    private float dashTime = 0;
    private float lastAccel = 0;

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Player player = GetComponent<Player>();
        float maxSpeed = player.moveSpeed;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            maxSpeed = gameObject.GetComponent<Player>().moveSpeedShooting;
        }
        if (player.currentSpeed < maxSpeed && Time.time > lastAccel + 0.1)
        {
            player.currentSpeed += player.accelerationSpeed/10;
            lastAccel = Time.time;
        }
        if (player.currentSpeed > maxSpeed)
        {
            player.currentSpeed = maxSpeed;
        }

        Rigidbody2D playerRB = gameObject.GetComponent<Rigidbody2D>();
        //Looking
        Vector2 lookDir = (mousePos - playerRB.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        playerRB.rotation = angle;

        
        if (dashingState == DashAbility.DashingState.Ready && Input.GetKey(KeyCode.LeftShift) && Time.time > dashTime + dashCooldown)
        {
            dashingState = GetComponent<DashAbility>().Dash(lookDir);
            dashTime = Time.time;
        }
        //Dashing
        else if (dashingState == DashAbility.DashingState.Dashing || dashingState == DashAbility.DashingState.Charging)
        {
            dashingState = GetComponent<DashAbility>().Dash(movement);
        }
        //Walking
        else
        {
            playerRB.MovePosition(playerRB.position + movement * player.currentSpeed * Time.fixedDeltaTime);
        }
    }
}
