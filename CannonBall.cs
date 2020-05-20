using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public bool isPlayer;
    public GameObject explosionEffect;
    public ObjectInfo towerInfo;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBall());
    }

    public void SetTarget(bool teamValue, ObjectInfo objInfo)
    {
        isPlayer = teamValue;
        towerInfo = objInfo;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (isPlayer)
        {
            if (collider.gameObject.layer == 9)
            {
                TakeDamage(collider);
                Destroy(gameObject, 0.2f);
            }
        }
        else
        {
            if (collider.gameObject.layer == 8)
            {
                TakeDamage(collider);
                Destroy(gameObject, 0.2f);
            }
        }

    }

    private IEnumerator DestroyBall()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            GetComponent<ParticleSystem>().Play();
            Destroy(gameObject, 1.0f);
        }
    }

    private void TakeDamage(Collider collider)
    {
        ObjectInfo unitInfo = collider.gameObject.GetComponent<ObjectInfo>();
        if (unitInfo != null)
        {
            HealthSystem HS = unitInfo.gameObject.GetComponent<HealthSystem>();
            if (towerInfo != null)
            {
                if (towerInfo.damageType == DamageType.Projectile)
                {
                    if (unitInfo.objectName != ObjectList.enemy_Soldier) //Bandit Warrior is immune to projectiles
                    {
                        unitInfo.health -= towerInfo.atk;
                        HS.Hit(towerInfo.atk);
                        HS.SetMaxHealthValue(unitInfo.maxHealth);
                        HS.SetHealthValue(unitInfo.health);
                    }
                    else
                    {
                        HS.Hit(0f);
                        HS.SetMaxHealthValue(unitInfo.maxHealth);
                        HS.SetHealthValue(unitInfo.health);
                    }
                }
            }
        }
        GetComponent<ParticleSystem>().Play();
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        AudioManager.Instance.PlayCombatSound("CannonImpact", transform.position);
    }
}
