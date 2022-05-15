using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectShield : MonoBehaviour
{
    public GameObject playerBullet;
    public GameObject enemyBullet;
    public enum ShieldType {Player,Enemy};
    public ShieldType shieldType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>() && !collision.GetComponent<Bullet>().hit)
        {
            Bullet oldBullet = collision.GetComponent<Bullet>();
            if (shieldType.ToString() != oldBullet.owner)
            {
                Rigidbody2D rbOld = oldBullet.GetComponent<Rigidbody2D>();

                GameObject newBulletGameObject;

                if (shieldType == ShieldType.Player)
                {
                    newBulletGameObject = collision.GetComponent<Bullet>().CreateCopy("Player");
                }
                else
                {
                    newBulletGameObject = collision.GetComponent<Bullet>().CreateCopy("Enemy");
                }
                newBulletGameObject.GetComponent<Bullet>().damage *= 2;

                /*if (shieldType == ShieldType.Player)
                {
                    newBulletGameObject = Instantiate(playerBullet, collision.transform.position, newRotation, collision.transform.parent);
                    newBulletGameObject.GetComponent<Bullet>().owner = "Player";
                }
                else
                {
                    newBulletGameObject = Instantiate(enemyBullet, collision.transform.position, newRotation, collision.transform.parent);
                    newBulletGameObject.GetComponent<Bullet>().owner = "Enemy";
                }

                newBulletGameObject.GetComponent<Bullet>().transform.localScale = oldBullet.transform.localScale;
                newBulletGameObject.GetComponent<Bullet>().createTime = Time.time;
                newBulletGameObject.GetComponent<Bullet>().damage = oldBullet.damage * 2;
                newBulletGameObject.GetComponent<Bullet>().pierce = oldBullet.pierce;
                newBulletGameObject.GetComponent<Bullet>().airTime = oldBullet.airTime;

                newBulletGameObject.GetComponent<Bullet>().splitAmount = oldBullet.splitAmount;
                newBulletGameObject.GetComponent<Bullet>().splitBulletSize = oldBullet.splitBulletSize;
                newBulletGameObject.GetComponent<Bullet>().splitBulletSpeed = oldBullet.splitBulletSpeed;
                newBulletGameObject.GetComponent<Bullet>().splitOnHit = oldBullet.splitOnHit;
                newBulletGameObject.GetComponent<Bullet>().splitRange = oldBullet.splitRange;*/

                Rigidbody2D rbNew = newBulletGameObject.GetComponent<Rigidbody2D>();
                rbNew.AddForce(-rbOld.velocity*2, ForceMode2D.Impulse);

                Destroy(collision.gameObject);
            }
        }
    }
}
