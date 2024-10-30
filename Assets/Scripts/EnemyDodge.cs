using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodge : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() is Bullet bullet && bullet.owner.tag == "Player")
        {
            Vector3 bulletDirection = collision.GetComponent<Rigidbody2D>().velocity.normalized;
            gameObject.GetComponentInParent<Enemy>().bulletTrigger = true;
            gameObject.GetComponentInParent<Enemy>().bulletDirection = bulletDirection;
        }
    }
}
