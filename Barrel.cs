using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Rigidbody rigidBody;
    public bool isBurning = false;
    private bool hasExploded = false;
    private Vector3 originalPos;

    public GameObject[] fire;
    public GameObject explosionEffect;
    public float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBurning)
        {
            timer += Time.deltaTime;
            if(timer >= 3.5f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
            for (int i = 0; i < fire.Length - 1; i++)
            {
                if(fire[i].activeSelf)
                {
                    fire[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9)
        {
            rigidBody.AddForce(col.gameObject.transform.forward * 500f, ForceMode.Force);
        }

        if(col.tag == "Fire")
        {
            if(!isBurning)
            {
                gameObject.layer = 0;
                isBurning = true;
                StartFireEffect();
            }
        }
    }

    public void StartFireEffect()
    {
        for (int i = 0; i < fire.Length - 1; i++)
        {
            fire[i].SetActive(true);
        }
    }

    private void Explode()
    {
        int layerMaskPlayer = 1 << 8;
        Collider[] playerUnits = Physics.OverlapSphere(transform.position, 18f, layerMaskPlayer);

        int layerMaskEnemy = 1 << 9;
        Collider[] enemyUnits = Physics.OverlapSphere(transform.position, 24f, layerMaskEnemy);

        int layerBarrels = 1 << 14;
        Collider[] barrels = Physics.OverlapSphere(transform.position, 16f, layerBarrels);



        if (playerUnits.Length > 0)
        {
            for (int i = 0; i < playerUnits.Length; i++)
            {
                TakeDamage(playerUnits[i]);
            }
        }

        if(enemyUnits.Length > 0)
        {
            for (int i = 0; i < enemyUnits.Length; i++)
            {
                TakeDamage(enemyUnits[i]);
            }
        }

        if(barrels.Length > 0)
        {
            for (int i = 0; i < barrels.Length; i++)
            {
                Barrel barrel = barrels[i].GetComponent<Barrel>();
                if (barrel != null)
                {
                    if (!barrel.isBurning && !barrel.hasExploded)
                    {
                        barrel.isBurning = true;
                        barrel.StartFireEffect();
                    }
                }
            }
        }

        Vector3 explosionPosition = new Vector3(transform.position.x - 1f, transform.position.y + 2f, transform.position.z);
        Instantiate(explosionEffect, explosionPosition, Quaternion.identity);

        GameObject barrelSpawner = new GameObject("BarrelSpawn");
        barrelSpawner.transform.position = transform.position;
        BarrelSpawner spawner = barrelSpawner.AddComponent<BarrelSpawner>();
        if(spawner != null)
        {
            spawner.spawnPos = originalPos;
            spawner.enableSpawn = true;
        }

        AudioManager.Instance.PlayCombatSound("CannonImpact", transform.position);
        Destroy(gameObject, 0.4f);
    }

    private void TakeDamage(Collider collider)
    {
        ObjectInfo unitInfo = collider.gameObject.GetComponent<ObjectInfo>();
        if (unitInfo != null)
        {
            HealthSystem HS = unitInfo.gameObject.GetComponent<HealthSystem>();
            float damage = 0;
            if(unitInfo.objectName != ObjectList.player_Pikeman) //pikemen are immune to explosions
            {
                if (unitInfo.isPlayerObject)
                {
                    damage = 300;
                }
                else
                {
                    damage = 900;
                }
            }

            unitInfo.health -= damage;
            HS.Hit(damage);
            HS.SetMaxHealthValue(unitInfo.maxHealth);
            HS.SetHealthValue(unitInfo.health);
        }
    }
}
