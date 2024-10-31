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

    private int outOfBoundsCounter = 0;

    private Entity lastDamageSource;

    public float score = 0;
    [SerializeField]
    private float onDeathScore = 100;

    protected AudioManager audioManager;

    private void Start()
    {
        walls = GameObject.Find("Walls").GetComponent<Tilemap>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        emptyHealthBar = transform.Find("EmptyHealthBar");
        healthBar = emptyHealthBar.GetChild(0);

        if (CheckOutOfBounds(this.transform.position)) lastValidPosition = this.transform.position;

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
        // Give on death score to last damage source
        lastDamageSource.GiveScore(onDeathScore);

        // Play death sound
        audioManager.PlayDieSound();

        Destroy(this.gameObject);
    }

    public virtual void UpdateEntity()
    {

    }

    public virtual void StartEntity()
    {

    }

    public virtual void TakeDamage(float amount, Entity source)
    {
        if (amount <= 0) return;

        this.health -= amount;

        lastDamageSource = source;
    }

    public void GiveScore(float score)
    {
        this.score += score;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // An entity comes in contact with a player => player takes damage.
        // An enemy comes in contact with a player => enemy takes damage.
        // An enemy comes in contact with an other enemy => no damage is taken.
        if (collision.transform.CompareTag("Enemy") && this.transform.CompareTag("Enemy")) return;

        Entity entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null && Time.time - lastHit > contactHitCooldown)
        {
            lastHit = Time.time;

            entity.TakeDamage(contactDamage, this);
        }
    }

    public bool CheckOutOfBounds(Vector3 position)
    {
        if (walls.GetTile(Vector3Int.FloorToInt(this.transform.position)) != null) return true;
        if (this.transform.position.x < -9.5f || this.transform.position.x > 9.5) return true;

        return false;
    }

    public void EnforceValidPosition()
    {
        if (CheckOutOfBounds(this.transform.position))
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
                    if (outOfBoundsCounter < 3)
                    {
                        this.transform.position = lastValidPosition;
                        outOfBoundsCounter += 1;
                    }
                    else
                    {
                        Vector3 safePosition = new Vector3(0, 0, 0);
                        GameObject waveManager = GameObject.Find("WaveManager");
                        if (waveManager != null)
                        {
                            safePosition = waveManager.GetComponent<WaveManager>().GetSafePosition();
                        }
                        
                        this.transform.position = safePosition;
                        outOfBoundsCounter = 0;
                    }
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
