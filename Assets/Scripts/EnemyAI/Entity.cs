using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
    public float damage = 10;
    private Transform healthBar;
    private Transform emptyHealthBar;

    private void Start()
    {
        emptyHealthBar = transform.Find("EmptyHealthBar");
        healthBar = emptyHealthBar.GetChild(0);
        StartEntity();
    }
    private void Update()
    {
        float scale = health / maxHealth;
        emptyHealthBar.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
        healthBar.localScale = new Vector3(scale,1,1);
        UpdateEntity();
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void UpdateEntity()
    {

    }

    public virtual void StartEntity()
    {

    }
}
