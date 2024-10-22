using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Player : Entity
{
    public Class playerClass;
    [SerializeField]
    private Class defaultPlayerClass = null;

    [SerializeField]
    private float invulnerableDuration = 0.5f;
    private float invulnerableStart;

    private ShootingAbility shootingAbility;
    private ReflectShieldAbility reflectShieldAbility;

    public GameObject bulletWall;

    public UnityEvent onHitEvent;
    public UnityEvent onDeath;

    public bool isPVP = false;

    public bool blue = true;

    public override void StartEntity()
    {
        shootingAbility = GetComponent<ShootingAbility>();
        reflectShieldAbility = GetComponent<ReflectShieldAbility>();

        if (playerClass == null)
        {
            playerClass = defaultPlayerClass;
        }
        ApplyClass(playerClass, blue);
    }

    public void ApplyClass(Class playerClass, bool blue)
    {
        this.playerClass = playerClass;

        SpriteRenderer renderer = this.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            if (blue) renderer.sprite = playerClass.blueSprite;
            else renderer.sprite = playerClass.redSprite;
        }

        if (!isPVP) this.maxHealth = playerClass.maxHealth;
        else this.maxHealth = playerClass.pvpMaxHealth;

        Transform healthbar = this.transform.Find("EmptyHealthBar");
        healthbar.localScale = new Vector3(1 + ((this.maxHealth - 100) / 500f), 1, 1);

        this.health = this.maxHealth;

        this.damage = playerClass.damage;

        this.moveSpeed = playerClass.normalMoveSpeed;

        if (!isPVP) this.invulnerableDuration = playerClass.invulnerableDuration;

        this.contactDamage = playerClass.contactDamage;
        this.contactHitCooldown = playerClass.contactHitCooldown;

        if (shootingAbility != null)
        {
            shootingAbility.attackCooldown = playerClass.attackCooldown;

            shootingAbility.range = playerClass.range;
            shootingAbility.pierce = playerClass.pierce;
            shootingAbility.totalSplit = playerClass.totalSplit;
            shootingAbility.totalFan = playerClass.totalFan;
            shootingAbility.bulletSize = playerClass.bulletSize;
            shootingAbility.bulletSpeed = playerClass.bulletSpeed;

            shootingAbility.splitOnHit = playerClass.splitOnHit;
            shootingAbility.splitAmount = playerClass.splitAmount;
            shootingAbility.splitRange = playerClass.splitRange;
            shootingAbility.splitBulletSize = playerClass.splitBulletSize;
            shootingAbility.splitBulletSpeed = playerClass.splitBulletSpeed;
            shootingAbility.splitDamagePercentage = playerClass.splitDamagePercentage;

            shootingAbility.shootingMoveSpeed = playerClass.shootingMoveSpeed;
        }

        DashAbility dashAbility = GetComponent<DashAbility>();
        if (dashAbility != null)
        {
            dashAbility.dashCooldown = playerClass.dashCooldown;
            dashAbility.dashDuration = playerClass.dashDuration;
            dashAbility.chargeDuration = playerClass.chargeDuration;
            dashAbility.dashSpeed = playerClass.dashSpeed;
        }
    }

    public override void TakeDamage(float amount)
    {
        if (amount <= 0) return;
        if (Time.time - invulnerableStart < invulnerableDuration) return;

        this.health -= amount;

        invulnerableStart = Time.time;

        if (onHitEvent != null) onHitEvent.Invoke();
    }

    public override void OnDeath()
    {
        onDeath.Invoke();
    }
}
