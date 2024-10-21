using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField]
    private float refreshCooldown = 0.5f;
    [SerializeField]
    private float followSpeed = 5;

    private Vector3 targetPosition;
    private float lastRefresh;

    private void Start()
    {
        lastRefresh = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Time.time - lastRefresh > refreshCooldown)
        {
            lastRefresh = Time.time;

            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, Time.deltaTime * followSpeed);*/
    }
}
