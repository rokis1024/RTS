using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public bool enableSpawn = false;
    private float timer = 0f;
    private bool hasSpawned = false;
    public Vector3 spawnPos;
    public GameObject barrel;

    // Update is called once per frame
    void Update()
    {
        if (enableSpawn)
        {
            if(barrel == null)
            {
                barrel = GameObject.Find("MovingBarrels").GetComponent<BarrelSpawner>().barrel;
            }

            if (!hasSpawned)
            {
                timer += Time.deltaTime;
                if (timer >= 62f)
                {
                    if (barrel != null)
                    {
                        Instantiate(barrel, spawnPos, Quaternion.identity);
                        hasSpawned = true;
                        Destroy(gameObject, 5f);
                    }
                }
            }
        }
    }
}
