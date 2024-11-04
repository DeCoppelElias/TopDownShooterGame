using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReflectShieldAbility : MonoBehaviour
{
    public GameObject playerReflectShield;
    public GameObject enemyReflectShield;

    public enum ReflectShieldState { Ready, Reflecting, Cooldown}
    [SerializeField]
    public ReflectShieldState reflectShieldState;

    [SerializeField]
    private float reflectShieldDuration = 0.2f;
    public int reflectShieldCooldown = 3;
    private float reflectShieldStart = 0;
    private float lastReflectShield;

    private GameObject currentShield;

    public UnityEvent onPerformed;
    public UnityEvent onReady;

    public Sprite relfectingBulletSprite;

    private void Start()
    {
        GameObject reflectShield = playerReflectShield;
        if (GetComponent<Enemy>())
        {
            reflectShield = enemyReflectShield;
        }
        currentShield = Instantiate(reflectShield, transform.position, Quaternion.identity);
        currentShield.transform.SetParent(transform);
        currentShield.transform.localScale = new Vector3(1, 1, 1);

        currentShield.GetComponent<ReflectShield>().bulletSprite = relfectingBulletSprite;
        currentShield.GetComponent<ReflectShield>().owner = GetComponent<Entity>();

        currentShield.SetActive(false);
    }

    public void EnableReflectShield()
    {
        if (reflectShieldState == ReflectShieldState.Ready)
        {
            currentShield.SetActive(true);
            reflectShieldState = ReflectShieldState.Reflecting;
            reflectShieldStart = Time.time;
        }
    }

    private void Update()
    {
        if (reflectShieldState == ReflectShieldState.Cooldown)
        {
            if (Time.time - lastReflectShield > reflectShieldCooldown)
            {
                reflectShieldState = ReflectShieldState.Ready;

                if (onReady != null)
                {
                    onReady.Invoke();
                }
            }
        }
        if (reflectShieldState == ReflectShieldState.Reflecting)
        {
            if (Time.time - reflectShieldStart > reflectShieldDuration)
            {
                reflectShieldState = ReflectShieldState.Cooldown;
                currentShield.SetActive(false);
                lastReflectShield = Time.time;

                if (onPerformed != null)
                {
                    onPerformed.Invoke();
                }
            }
        }
    }
}
