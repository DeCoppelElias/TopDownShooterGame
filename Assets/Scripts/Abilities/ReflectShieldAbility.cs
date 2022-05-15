using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectShieldAbility : MonoBehaviour
{
    public GameObject playerReflectShield;
    public GameObject enemyReflectShield;

    public float duration;
    private GameObject currentShield;
    public void CreateReflectShield()
    {
        if (!GetComponentInChildren<ReflectShield>())
        {
            GameObject reflectShield = playerReflectShield;
            if (GetComponent<Enemy>())
            {
                reflectShield = enemyReflectShield;
            }
            currentShield = Instantiate(reflectShield, transform.position, Quaternion.identity);
            currentShield.transform.SetParent(transform);
            currentShield.transform.localScale = new Vector3(1, 1, 1);
            Destroy(currentShield, duration);
        }
    }
}
