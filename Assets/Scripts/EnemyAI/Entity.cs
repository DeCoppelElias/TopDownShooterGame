using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Entity : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
    public float damage = 10;
    public float moveSpeed = 3;

    public float contactDamage = 10;
    public float contactHitCooldown = 1f;

    private float lastHit = 0;

    private Transform healthBar;
    private Transform emptyHealthBar;

    private Tilemap walls;

    public Vector3 lastValidPosition;
    private float updateValidPositionCooldown = 2f;
    private float lastValidPositionUpdate = 0;

    [SerializeField]
    private bool outOfBounds = false;
    private float allowedOutOfBoundsDuration = 1f;
    private float outOfBoundsStart = 0;

    private void Start()
    {
        walls = GameObject.Find("Walls").GetComponent<Tilemap>();

        emptyHealthBar = transform.Find("EmptyHealthBar");
        healthBar = emptyHealthBar.GetChild(0);

        lastValidPosition = this.transform.position;

        StartEntity();
    }
    private void Update()
    {
        float scale = health / maxHealth;
        healthBar.localScale = new Vector3(scale,1,1);

        EnforceValidPosition();

        UpdateEntity();

        if (health <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath()
    {
        Destroy(this.gameObject);
    }

    public virtual void UpdateEntity()
    {

    }

    public virtual void StartEntity()
    {

    }

    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        this.health -= amount;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // An entity comes in contact with a player => player takes damage.
        // An enemy comes in contact with a player => enemy takes damage.
        // An enemy comes in contact with an other enemy => no damage is taken.
        if (collision.transform.CompareTag("Player") || (collision.transform.CompareTag("Enemy") && this.transform.CompareTag("Player")))
        {
            if (Time.time - lastHit > contactHitCooldown)
            {
                lastHit = Time.time;
                Entity entity = collision.transform.GetComponent<Entity>();
                if (entity == null) throw new System.Exception("Collision has tag: " + collision.transform.tag + " but lacks Entity component.");

                entity.TakeDamage(contactDamage);
            }
        }
    }

    public void EnforceValidPosition()
    {
        if (walls.GetTile(Vector3Int.FloorToInt(this.transform.position)) != null)
        {
            if (!outOfBounds)
            {
                outOfBounds = true;
                outOfBoundsStart = Time.time;
            }
            else
            {
                if (Time.time - outOfBoundsStart > allowedOutOfBoundsDuration)
                {
                    this.transform.position = lastValidPosition;
                    this.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

                    outOfBounds = false;
                }
            }
        }
        else
        {
            outOfBounds = false;

            if (Time.time - lastValidPositionUpdate > updateValidPositionCooldown)
            {
                lastValidPosition = this.transform.position;
                lastValidPositionUpdate = Time.time;
            }
        }
    }
}
