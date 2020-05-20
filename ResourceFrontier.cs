using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFrontier : MonoBehaviour
{
    public GameObject blueFlag;
    public GameObject redFlag;
    public GameObject cannonSpawn;
    public GameObject cannonBall;
    public GameObject target;
    private int conqueredCount;

    public bool isConquered = false;
    public bool isNeutral;

    private ObjectInfo objInfo;
    private ResourceManager RM;
    private GUIManager GUI;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateStatus());
        StartCoroutine(Attack());

        objInfo = GetComponent<ObjectInfo>();
        RM = FindObjectOfType<ResourceManager>();
        GUI = FindObjectOfType<GUIManager>();
    }

    IEnumerator UpdateStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            int layerMaskPlayer = 1 << 8;
            Collider[] playerUnits = Physics.OverlapSphere(transform.position, 32, layerMaskPlayer);

            int layerMaskEnemy = 1 << 9;
            Collider[] enemyUnits = Physics.OverlapSphere(transform.position, 32, layerMaskEnemy);

            if (playerUnits.Length == 0 && enemyUnits.Length == 0 || playerUnits.Length == enemyUnits.Length)
            {
                isNeutral = true;
            }
            else
            {
                isNeutral = false;
            }

            if ((playerUnits.Length > enemyUnits.Length) && !isNeutral)
            {
                isConquered = true;
                if ((RM.stone + 3) <= RM.maxStone)
                {
                    RM.stone += 3;
                    RM.collectedStone += 3;
                }

                if ((RM.iron + 3) <= RM.maxIron)
                {
                    RM.iron += 3;
                    RM.collectedIron += 3;
                }

                if ((RM.gold + 3) <= RM.maxGold)
                {
                    RM.gold += 3;
                    RM.collectedGold += 3;
                }
            }
            else
            {
                isConquered = false;
            }

            blueFlag.SetActive(isConquered && !isNeutral);
            redFlag.SetActive(!isConquered && !isNeutral);

            conqueredCount = GUI.UpdateFrontierCount();
        }
    }

    public IEnumerator Attack()
    {
        while(true)
        {
            yield return new WaitForSeconds(10f);

            int layerMaskPlayer = 1 << 8;
            Collider[] playerUnits = Physics.OverlapSphere(transform.position, 40, layerMaskPlayer);

            int layerMaskEnemy = 1 << 9;
            Collider[] enemyUnits = Physics.OverlapSphere(transform.position, 40, layerMaskEnemy);

            if (!isNeutral)
            {
                if (isConquered)
                {
                    if (enemyUnits.Length > 0)
                    {
                        int i = 0;

                        while (i < enemyUnits.Length)
                        {
                            if (enemyUnits[i] != null)
                            {
                                ObjectInfo targetInfo = enemyUnits[i].gameObject.GetComponent<ObjectInfo>();

                                if (!targetInfo.isPlayerObject && targetInfo.isAlive)
                                {
                                    target = enemyUnits[i].gameObject;
                                    ShotCannon(target, true);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                }
                else
                {
                    if (playerUnits.Length > 0)
                    {
                        int i = 0;

                        while (i < playerUnits.Length)
                        {
                            if (playerUnits[i] != null)
                            {
                                ObjectInfo targetInfo = playerUnits[i].gameObject.GetComponent<ObjectInfo>();

                                if (targetInfo.isPlayerObject && targetInfo.isAlive)
                                {
                                    target = playerUnits[i].gameObject;
                                    ShotCannon(target, false);
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                }
            }
        }
    }

    public void ShotCannon(GameObject target, bool value)
    {
        if(target != null)
        {
            cannonSpawn.transform.rotation = Quaternion.LookRotation(target.transform.position - cannonSpawn.transform.position);

            GameObject cannon = Instantiate(cannonBall, cannonSpawn.transform.position, Quaternion.identity);
            Rigidbody rb = cannon.AddComponent<Rigidbody>();
            //rb.useGravity = true;

            CannonBall ball = cannon.GetComponent<CannonBall>();
            if(ball != null)
            {
                ball.SetTarget(value, objInfo);
            }

            rb.velocity = CannonVelocity(target, 73f);
            rb.AddTorque(new Vector3(0, 0, 1000), ForceMode.Force);
            AudioManager.Instance.PlayCombatSound("CannonShot", transform.position);
        }
    }

    public Vector3 CannonVelocity(GameObject target, float angle)
    {
        var dir = target.transform.position - cannonSpawn.transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        var dist = dir.magnitude;  // get horizontal distance
        var a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }
}
