using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject MainCastle;
    public TaskList task;

    private ActionList AL;

    public bool canAttack;
    public bool isScout;
    private bool toConquer = false;

    public GameObject unitMesh;
    public Material[] unitMaterials;
    public Material[] highlightedMaterials;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Animator anim;
    private ObjectInfo objInfo;
    private ResourceFrontier[] frontiers;
    private SkinnedMeshRenderer unitRenderer;

    public float distToTarget;
    public float range;
    public float buildingRange;
    private float timer = 0f;
    public float conquerTime;
    private float pendingTimer = 0f;

    void Start()
    {
        StartCoroutine(ClosestEnemies());
        frontiers = FindObjectsOfType<ResourceFrontier>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        objInfo = GetComponent<ObjectInfo>();
        AL = FindObjectOfType<ActionList>();
        unitRenderer = unitMesh.GetComponent<SkinnedMeshRenderer>();

        MainCastle = GameObject.Find("playerBuild_Castle");

        if (GameManager.Instance.GetDifficulty() == 1)
        {
            objInfo.maxHealth = Mathf.Round(objInfo.maxHealth *= 1.05f);
            objInfo.health = objInfo.maxHealth;
            objInfo.atk = Mathf.Round(objInfo.atk *= 1.15f);
        }
        else if(GameManager.Instance.GetDifficulty() == 2)
        {
            objInfo.maxHealth = Mathf.Round(objInfo.maxHealth *= 1.1f);
            objInfo.health = objInfo.maxHealth;
            objInfo.atk = Mathf.Round(objInfo.atk *= 1.3f);
        }
    }

    void Update()
    {
        if (task == TaskList.Attacking && objInfo.target != null)
        {
            if (objInfo.target.GetComponent<ObjectInfo>().isAlive)
            {
                Vector3 enemyPosition = objInfo.target.GetComponent<ObjectInfo>().selectionIndicator.transform.position;
                distToTarget = Vector3.Distance(enemyPosition, transform.position);

                var targetRotation = Quaternion.LookRotation(objInfo.target.transform.position - transform.position);

                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

                if (objInfo.target.GetComponent<ObjectInfo>().isUnit)
                {
                    if (distToTarget <= range)
                    {
                        anim.SetBool("isWalking", false);
                        canAttack = true;

                        anim.SetBool("isAttacking", true);

                        if (!obstacle.enabled && agent.enabled)
                        {
                            obstacle.enabled = true;
                            agent.enabled = false;
                        }
                    }
                    else
                    {
                        if (obstacle.enabled)
                        {
                            obstacle.enabled = false;
                            agent.enabled = true;
                        }
                        anim.SetBool("isAttacking", false);
                        anim.SetBool("isWalking", true);
                        agent.SetDestination(objInfo.target.transform.position);
                        pendingTimer = 0f;
                        canAttack = false;
                    }
                }
                else
                {
                    if (distToTarget <= buildingRange + 3.6)
                    {
                        anim.SetBool("isWalking", false);
                        canAttack = true;

                        anim.SetBool("isAttacking", true);

                        if (!obstacle.enabled && agent.enabled)
                        {
                            obstacle.enabled = true;
                            agent.enabled = false;
                        }
                    }
                    else
                    {
                        if (obstacle.enabled)
                        {
                            obstacle.enabled = false;
                            agent.enabled = true;
                        }
                        anim.SetBool("isAttacking", false);
                        anim.SetBool("isWalking", true);
                        agent.SetDestination(objInfo.target.transform.position);
                        pendingTimer = 0f;
                        canAttack = false;
                    }
                }
            }
            else
            {
                objInfo.target = null;
                if (obstacle.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                anim.SetBool("isAttacking", false);
                canAttack = false;
                task = TaskList.Idle;
            }
        }

        if (objInfo.target == null)
        {
            if (task == TaskList.Attacking)
            {
                if (obstacle.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                anim.SetBool("isAttacking", false);
                canAttack = false;
                task = TaskList.Idle;
            }
        }

        if (agent.pathPending)
        {
            pendingTimer += Time.deltaTime;
            if (pendingTimer >= 4f)
            {
                agent.destination = transform.position;
            }
        }

        if ((task == TaskList.Idle || task == TaskList.Moving) && objInfo.target == null)
        {
            if (isScout && !toConquer)
            {
                timer += Time.deltaTime;
                if (timer >= conquerTime)
                {
                    Conquer();
                    timer = 0;
                }
            }

            if(!isScout && !toConquer && conquerTime != 0)
            {
                timer += Time.deltaTime;
                if(timer >= conquerTime)
                {
                    AttackCastle();
                    timer = 0;
                }
            }
        }

        if (!agent.pathPending && (anim.GetBool("isWalking") == true || (anim.GetBool("isAttacking") == true) && task != TaskList.Attacking))
        {
            if (obstacle.enabled)
            {
                obstacle.enabled = false;
                agent.enabled = true;
            }
            if (agent.remainingDistance <= agent.stoppingDistance + 2.6f)
            {
                if (agent.velocity.sqrMagnitude <= 0.5f)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isAttacking", false);
                    task = TaskList.Idle;
                    agent.destination = transform.position;
                    toConquer = false;
                }
            }
        }

        if(objInfo.isSelected)
        {
            OnMouseExit();
        }
    }

    public void Conquer()
    {
        foreach(ResourceFrontier rf in frontiers)
        {
            if(rf != null)
            {
                if(rf.isConquered || rf.isNeutral)
                {
                    if (!agent.enabled)
                    {
                        obstacle.enabled = false;
                        agent.enabled = true;
                    }
                    anim.SetBool("isWalking", true);
                    task = TaskList.Moving;
                    agent.SetDestination(rf.gameObject.transform.position - new Vector3(Random.Range(10.0f, 15.0f), 0, Random.Range(10.0f, 15.0f)));
                    pendingTimer = 0f;
                    toConquer = true;
                }
            }
        }
    }

    public void AttackCastle()
    {
        if (MainCastle != null && MainCastle.GetComponent<ObjectInfo>().isAlive)
        {
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(MainCastle.transform.position);
            pendingTimer = 0f;
            task = TaskList.Moving;
            anim.SetBool("isWalking", true);
            toConquer = true;
        }
    }

    public void Attack()
    {
        if (canAttack)
        {
            if (objInfo.target != null)
            {
                AL.Attack(objInfo.target, objInfo);
                AudioManager.Instance.PlayRandomCombatSound(transform.position);
            }
            else
            {
                canAttack = false;
            }
        }
    }

    private IEnumerator ClosestEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f);

            if (objInfo.isAlive)
            {
                int layerMask = 1 << 8;
                Collider[] enemies = Physics.OverlapSphere(transform.position, 32, layerMask);
                float closestDistance = Mathf.Infinity;

                int i = 0;

                while (i < enemies.Length)
                {
                    if (enemies[i] != null)
                    {
                        ObjectInfo targetInfo = enemies[i].gameObject.GetComponent<ObjectInfo>();
                        if (targetInfo.isPlayerObject && targetInfo.isAlive)
                        {
                            Vector3 direction = enemies[i].gameObject.transform.position - transform.position;
                            float distance = direction.sqrMagnitude;

                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                objInfo.target = enemies[i].gameObject;
                                task = TaskList.Attacking;
                            }
                        }
                    }
                    i++;
                }
            }
        }
    }

    void OnMouseOver()
    {
        if (!objInfo.isSelected)
        {
            unitRenderer.materials = highlightedMaterials;
        }
    }

    void OnMouseExit()
    {
        unitRenderer.materials = unitMaterials;
    }

}
