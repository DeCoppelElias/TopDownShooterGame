using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectShield : MonoBehaviour
{
    public Sprite bulletSprite;
    public string owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() && !collision.GetComponent<Bullet>().hit)
        {
            Bullet oldBullet = collision.GetComponent<Bullet>();
            if (owner != oldBullet.owner)
            {
                Rigidbody2D rbOld = oldBullet.GetComponent<Rigidbody2D>();

                GameObject newBulletGameObject;

                newBulletGameObject = collision.GetComponent<Bullet>().CreateCopy(owner);
                newBulletGameObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;

                Bullet newBullet = newBulletGameObject.GetComponent<Bullet>();
                Rigidbody2D rbNew = newBulletGameObject.GetComponent<Rigidbody2D>();
                if (!oldBullet.reflected)
                {
                    newBullet.damage *= 2;
                    rbNew.AddForce(-rbOld.velocity * 2, ForceMode2D.Impulse);
                    newBullet.reflected = true;
                }
                else
                {
                    rbNew.AddForce(-rbOld.velocity, ForceMode2D.Impulse);
                }

                Destroy(collision.gameObject);
            }
        }
    }
}
