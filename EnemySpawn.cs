using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    private ResourceFrontier[] resourceFrontier;
    private ResourceManager RM;
    private GameObject[] spawnedUnits;
    public GameObject[] enemyUnits;

    public bool canSummon = false;
    public float spawningTime;
    public int limit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnUnit());

        spawnedUnits = new GameObject[enemyUnits.Length];
        resourceFrontier = FindObjectsOfType<ResourceFrontier>();
        RM = FindObjectOfType<ResourceManager>();
    }

    IEnumerator SpawnUnit()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawningTime);

            if (limit > 0)
            {
                foreach (ResourceFrontier rf in resourceFrontier)
                {
                    if (!rf.isConquered && !rf.isNeutral)
                    {
                        canSummon = true;
                    }
                }

                if (canSummon && gameObject.GetComponent<ObjectInfo>().isAlive)
                {
                    Vector3 newPos = gameObject.transform.position - new Vector3(16, 0, 16);
                    Vector3 newPos2 = gameObject.transform.position - new Vector3(17.5f, 0, 17.5f);

                    spawnedUnits[0] = Instantiate(enemyUnits[0], newPos, this.gameObject.transform.rotation);
                    spawnedUnits[0].GetComponent<RandomPoint>().wanderRadius = Random.Range(30.0f, 80.0f);
                    spawnedUnits[0].GetComponent<Enemy>().conquerTime = Random.Range(120.0f, 400.0f);

                    if (enemyUnits.Length >= 2)
                    {
                        spawnedUnits[1] = Instantiate(enemyUnits[1], newPos2, this.gameObject.transform.rotation);
                        spawnedUnits[1].GetComponent<Enemy>().conquerTime = Random.Range(50.0f, 300.0f);
                    }

                    canSummon = false;
                    RM.producedEnemies += enemyUnits.Length;
                    limit--;
                }
            }
        }
    }
}
