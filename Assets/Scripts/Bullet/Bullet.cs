using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float pierce;
    public float damage;
    public float airTime;
    public float createTime;
    public bool hit = false;

    public bool splitOnHit = false;
    public float splitAmount = 0;
    public float splitRange = 1;
    public float splitBulletSize = 0.5f;
    public float splitBulletSpeed = 6;
    public float splitDamagePercentage = 0.5f;

    public string owner;

    private void Update()
    {
        if(!hit && Time.time > createTime + airTime)
        {
            BulletMiss();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == "Enemy" && collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.TakeDamage(damage);
            pierce--;
            if (pierce == 0)
            {
                BulletHit();
            }
        }
        else if (owner == "Player" && collision.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            pierce--;
            if (pierce == 0)
            {
                BulletHit();
            }
        }
        else if (collision.tag == "ReflectWall")
        {
            Tilemap tileMap = collision.gameObject.GetComponent<Tilemap>();
            CustomWallTile tile = (CustomWallTile)tileMap.GetTile(Vector3Int.FloorToInt(transform.position));
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // the vector that we want to measure an angle from
            Vector3 referenceForward = Vector3.right; /* some vector that is not Vector3.up */
            /*if (tile.getTileState(Vector3Int.FloorToInt(transform.position)) == CustomWallTile.TileState.Vertical)
            {
                referenceForward = Vector3.down;
            }*/
            // the vector perpendicular to referenceForward (90 degrees clockwise)
            // (used to determine if angle is positive or negative)
            Vector3 referenceRight = Quaternion.Euler(0, 0, 90) * referenceForward;
            // the vector of interest
            Vector3 newDirection = rb.velocity.normalized; /* some vector that we're interested in */
            // Get the angle in degrees between 0 and 180
            float angle = Vector3.Angle(newDirection, referenceForward);
            // Determine if the degree value should be negative.  Here, a positive value
            // from the dot product means that our vector is on the right of the reference vector   
            // whereas a negative value means we're on the left.
            float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
            float finalAngle = sign * angle;
            float rotateAngle = -(2 * finalAngle);

            Vector3 reflectVelocity = (Quaternion.Euler(0, 0, rotateAngle) * rb.velocity);
            GameObject newBullet = CreateCopy(owner);
            newBullet.transform.position += reflectVelocity.normalized *0.3f;
            newBullet.GetComponent<Rigidbody2D>().AddForce(reflectVelocity, ForceMode2D.Impulse);
            Destroy(this.gameObject);
        }
        else if (collision.tag == "Wall")
        {
            BulletHit();
        }
    }

    public void BulletHit()
    {
        if (splitOnHit)
        {
            Split();
        }

        hit = true;
        Destroy(gameObject);
    }

    public void BulletMiss()
    {
        Split();
        Destroy(gameObject);
    }
    public void Split()
    {
        if (splitAmount > 0)
        {
            float angleInterval = 360 / splitAmount;
            float currentAngle = 0;
            for (int i = 0; i < splitAmount; i++)
            {
                CreateSplitBullet(currentAngle);
                currentAngle += angleInterval;
            }
        }
    }
    public void CreateSplitBullet(float currentAngle)
    {
        Vector3 vector = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
        GameObject bullet = Instantiate(gameObject, transform.position + vector/3, Quaternion.identity,transform.parent);
        bullet.transform.localScale = new Vector3(splitBulletSize, splitBulletSize,1);

        bullet.GetComponent<Bullet>().pierce = 1;
        bullet.GetComponent<Bullet>().damage = damage * splitDamagePercentage;
        bullet.GetComponent<Bullet>().owner = owner;
        bullet.GetComponent<Bullet>().airTime = splitRange / splitBulletSpeed;
        bullet.GetComponent<Bullet>().createTime = Time.time;

        bullet.GetComponent<Bullet>().splitAmount = 0;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        rb.AddForce(vector * splitBulletSpeed, ForceMode2D.Impulse);
    }

    public GameObject CreateCopy(string owner)
    {
        GameObject newBulletGameObject = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        newBulletGameObject.GetComponent<Bullet>().createTime = Time.time;
        if (owner == "Player")
        {
            newBulletGameObject.GetComponent<Bullet>().owner = owner;
        }
        else
        {
            newBulletGameObject.GetComponent<Bullet>().owner = owner;
        }
        return newBulletGameObject;
    }
}
 