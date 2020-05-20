using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public TaskList task;

    private ActionList AL;

    public bool canAttack;
    private bool targetSelected = false;

    public GameObject unitMesh;
    public Material[] unitMaterials;
    public Material[] highlightedMaterials;

    public GameObject prevTarget;
    public GameObject arrowObject;
    public GameObject arrowLocation;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Animator anim;
    private ObjectInfo objInfo;
    private SkinnedMeshRenderer unitRenderer;

    public float distToTarget;
    public float range;
    public float buildingRange;

    public float requiredIron;
    public float requiredGold;
    public ObjectList requiredBuilding;

    void Start()
    {
        StartCoroutine(ClosestEnemies());
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        objInfo = GetComponent<ObjectInfo>();
        AL = FindObjectOfType<ActionList>();
        unitRenderer = unitMesh.GetComponent<SkinnedMeshRenderer>();

        agent.autoBraking = false;
        agent.stoppingDistance = 1.5f;
    }

    void Update()
    {
        //animation control
        if(task == TaskList.Moving || task == TaskList.Attacking)
        {
            if(canAttack)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", true);
            }
            else
            {
                anim.SetBool("isAttacking", false);
                anim.SetBool("isWalking", true);
            }
        }

        if (task == TaskList.Idle)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", false);
        }

        if (task == TaskList.Attacking && objInfo.target != null) 
        {
            ObjectInfo targetInfo = objInfo.target.GetComponent<ObjectInfo>();
            if (targetInfo.isAlive)
            {
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;

                Vector3 enemyPosition = targetInfo.selectionIndicator.transform.position;
                distToTarget = Vector3.Distance(enemyPosition, transform.position);

                var targetRotation = Quaternion.LookRotation(objInfo.target.transform.position - transform.position);

                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

                if (targetInfo.isUnit)
                {
                    if (distToTarget <= range)
                    {
                        obstacle.enabled = true;
                        agent.enabled = false;
                        canAttack = true;
                    }
                    else
                    {
                        if (agent.pathStatus == NavMeshPathStatus.PathInvalid && !targetSelected)
                        {
                            ClearTarget();
                        }
                        else
                        {
                            obstacle.enabled = false;
                            agent.enabled = true;
                            agent.SetDestination(objInfo.target.transform.position);
                            canAttack = false;
                        }
                    }
                }
                else
                {
                    float buildingRangeTmp;
                    if(targetInfo.objectName == ObjectList.enemyBuild_Castle)
                    {
                        buildingRangeTmp = buildingRange + 6f;
                    }
                    else if(targetInfo.objectName == ObjectList.enemyBuild_House)
                    {
                        buildingRangeTmp = buildingRange + 5;
                    }
                    else
                    {
                        buildingRangeTmp = buildingRange;
                    }

                    if (distToTarget <= buildingRangeTmp)
                    {
                        obstacle.enabled = true;
                        agent.enabled = false;
                        canAttack = true;
                    }
                    else
                    {
                        if (agent.pathStatus == NavMeshPathStatus.PathInvalid && !targetSelected)
                        {
                            ClearTarget();
                        }
                        else
                        {
                            obstacle.enabled = false;
                            agent.enabled = true;
                            agent.SetDestination(objInfo.target.transform.position);
                            canAttack = false;
                        }
                    }
                }
            }
            else
            {
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                agent.destination = transform.position;
                objInfo.target = null;
                if (obstacle.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                canAttack = false;
                anim.SetBool("isAttacking", false);
                task = TaskList.Idle;
                targetSelected = false;
            }
        }

        if(objInfo.target == null)
        {
            if(task == TaskList.Attacking)
            {
                if (obstacle.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                agent.destination = transform.position;
                canAttack = false;
                task = TaskList.Idle;
                targetSelected = false;
            }
        }

        if (!agent.pathPending && (anim.GetBool("isWalking") == true || (anim.GetBool("isAttacking") == true) && task != TaskList.Attacking))
        {
            if (objInfo.target == null)
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
                        task = TaskList.Idle;
                        agent.destination = transform.position;
                        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(1) && objInfo.isSelected)
        {
            obstacle.enabled = false;
            agent.enabled = true;
            RightClick();
        }

        if(objInfo.isSelected)
        {
            OnMouseExit();
        }
    }

    public void RightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500))
        {
            if (hit.collider.tag == "Ground")
            {
                canAttack = false;
                objInfo.target = null;
                AL.Move(agent, hit, true);
                task = TaskList.Moving;
                targetSelected = false;
            }
            else if (hit.collider.tag == "Selectable")
            {
                ObjectInfo targetInfo = hit.collider.GetComponent<ObjectInfo>();
                canAttack = false;

                if (!targetInfo.isPlayerObject && targetInfo.objectName != ObjectList.building_Frontier)
                {
                    objInfo.target = targetInfo.gameObject;
                    AL.Move(agent, hit, false);
                    task = TaskList.Attacking;
                    targetSelected = true;
                    AudioManager.Instance.PlayMessageSound("Attack");
                }
            }
        }
    }

    public void Attack()
    {
        if (canAttack)
        {
            if (objInfo.target != null)
            {
                AL.Attack(objInfo.target, objInfo);
                if (objInfo.objectName != ObjectList.player_Crossbowman)
                {
                    AudioManager.Instance.PlayRandomCombatSound(transform.position);
                }
                else
                {
                    AudioManager.Instance.PlayCombatSound("ArrowImpact", objInfo.target.transform.position);
                }
            }
            else
            {
                canAttack = false;
            }
        }
    }

    public void ShootArrow()
    {
        if (canAttack)
        {
            if (objInfo.target != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(objInfo.target.transform.position - transform.position);
                GameObject arrowObj = Instantiate(arrowObject, arrowLocation.transform.position, arrowLocation.transform.rotation);


                Arrow arrow = arrowObj.GetComponent<Arrow>();

                if(arrow != null)
                {
                    arrow.SetTarget(objInfo.target, this);
                }
                AudioManager.Instance.PlayCombatSound("BowRelease1", transform.position);
                AudioManager.Instance.PlayCombatSound("BowRelease2", transform.position);

            }
            else
            {
                canAttack = false;
            }
        }
    }

    private void ClearTarget()
    {
        objInfo.target = null;
        obstacle.enabled = false;
        agent.enabled = true;
        agent.destination = transform.position;
        canAttack = false;
        task = TaskList.Idle;
        targetSelected = false;
    }

    private IEnumerator ClosestEnemies()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.4f);

            if(task != TaskList.Moving && !targetSelected && objInfo.isAlive)
            {
                int layerMask = 1 << 9;
                Collider[] enemies = Physics.OverlapSphere(transform.position, 32, layerMask);
                float closestDistance = Mathf.Infinity;

                int i = 0;

                while (i < enemies.Length)
                {
                    if (enemies[i] != null)
                    {
                        ObjectInfo targetInfo = enemies[i].gameObject.GetComponent<ObjectInfo>();
                        if (!targetInfo.isPlayerObject && targetInfo.isAlive)
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
