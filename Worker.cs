using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public TaskList task;

    public ResourceManager RM;

    private ActionList AL;

    private InputManager IM;

    public GameObject targetNode;

    public GameObject unitMesh;
    public Material[] unitMaterials;
    public Material[] highlightedMaterials;

    public NodeManager.ResourceTypes heldResourceType;

    public bool isGathering = false;
    public bool isGatherer = false;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Animator anim;
    private ObjectInfo objInfo;
    private SkinnedMeshRenderer unitRenderer;

    public int heldResource;
    public int maxHeldResource;

    public float distToTarget;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GatherTick());

        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        anim = GetComponent<Animator>();
        objInfo = GetComponent<ObjectInfo>();
        AL = FindObjectOfType<ActionList>();
        RM = FindObjectOfType<ResourceManager>();
        IM = FindObjectOfType<InputManager>();
        unitRenderer = unitMesh.GetComponent<SkinnedMeshRenderer>();

        agent.autoBraking = false;
        agent.stoppingDistance = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //animation control
        if (task == TaskList.Moving || task == TaskList.Gathering || task == TaskList.Delivering)
        {
            anim.SetBool("isWalking", true);
            if (isGathering)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isCollecting", true);
            }
            else
            {
                anim.SetBool("isCollecting", false);
                anim.SetBool("isWalking", true);
            }
        }

        if(task == TaskList.Idle)
        {
            anim.SetBool("isCollecting", false);
            anim.SetBool("isWalking", false);
        }

        if (!agent.pathPending && anim.GetBool("isWalking") == true)
        {
            if (targetNode == null)
            {
                obstacle.enabled = false;
                agent.enabled = true;
                if (agent.remainingDistance <= agent.stoppingDistance + 2.6)
                {
                    if (agent.velocity.sqrMagnitude <= 0.5f)
                    {
                        anim.SetBool("isWalking", false);
                        task = TaskList.Idle;
                        agent.destination = transform.position;
                    }
                }
            }
        }

        if (task == TaskList.Gathering)
        {
            if (targetNode != null)
            {
                distToTarget = Vector3.Distance(targetNode.transform.position, transform.position);

                var targetRotation = Quaternion.LookRotation(targetNode.transform.position - transform.position);

                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);

                if (distToTarget <= 8f)
                {
                    Gather();
                }
            }
        }

        if(task == TaskList.Delivering)
        {
            if (distToTarget <= 15.5f)
            {
                if (heldResourceType == NodeManager.ResourceTypes.Stone)
                {
                    if (RM.stone >= RM.maxStone)
                    {
                        task = TaskList.Idle;
                        isGatherer = false;
                    }
                    else if (RM.stone + heldResource >= RM.maxStone)
                    {
                        int resourceOverflow = (int)RM.maxStone - (int)RM.stone;

                        heldResource -= resourceOverflow;
                        RM.stone = RM.maxStone;
                        RM.collectedStone += heldResource;
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering;
                            agent.destination = targetNode.transform.position;
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false;
                    }
                    else
                    {
                        RM.stone += heldResource;
                        RM.collectedStone += heldResource;
                        heldResource = 0;
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering;
                            agent.destination = targetNode.transform.position;
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false;
                    }
                }
                else if (heldResourceType == NodeManager.ResourceTypes.Iron)
                {
                    if (RM.iron >= RM.maxIron) //Is the stored amount of Iron greater than or equal to the max amount of Iron?
                    {
                        task = TaskList.Idle; //Set the colonist to be idle
                        isGatherer = false; //Set the colonist to not be a gatherer
                    }
                    else if (RM.iron + heldResource >= RM.maxIron) //Is the stored amount of Iron going to exceed the max when the colonist delivers?
                    {
                        int resourceOverflow = (int)RM.maxIron - (int)RM.iron; //How much Iron can be stored before hitting capacity

                        heldResource -= resourceOverflow; //Remove the Iron that can be stored from the colonist
                        RM.iron = RM.maxIron; //Set the stored Iron to equal the maximum
                        RM.collectedIron += heldResource;
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            agent.destination = targetNode.transform.position; //Set the colonist's destination
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false; //Set the colonist to not be a gatherer
                    }
                    else
                    {
                        RM.iron += heldResource; //Add the colonist's Iron to the stored Iron
                        RM.collectedIron += heldResource;
                        heldResource = 0; //Empty the colonist's Iron storage
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering; //Set the colonist to go back to gathering
                            agent.destination = targetNode.transform.position; //Set the colonist's destination
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false; //Set the colonist to not be a gatherer
                    }
                }
                else if(heldResourceType == NodeManager.ResourceTypes.Gold)
                {
                    if(RM.gold >= RM.maxGold)
                    {
                        task = TaskList.Idle;
                        isGatherer = false;
                    }
                    else if(RM.gold + heldResource >= RM.maxGold)
                    {
                        int resourceOverflow = (int)RM.maxGold - (int)RM.gold;

                        heldResource -= resourceOverflow;
                        RM.gold = RM.maxGold;
                        RM.collectedGold += heldResource;
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering;
                            agent.destination = targetNode.transform.position;
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false;
                    }
                    else
                    {
                        RM.gold += heldResource;
                        RM.collectedGold += heldResource;
                        heldResource = 0;
                        if (targetNode != null)
                        {
                            task = TaskList.Gathering;
                            agent.destination = targetNode.transform.position;
                        }
                        else
                        {
                            task = TaskList.Idle;
                        }
                        isGatherer = false;
                    }
                }
            }
        }

        if(targetNode == null && objInfo.target == null && task == TaskList.Gathering)
        {
            isGatherer = false;
            if(heldResource != 0)
            {
                obstacle.enabled = false; //Disable the NavMeshObstacle component
                agent.enabled = true; //Enables the NavMeshAgent component
                isGathering = false;
                if (GetClosestDropOff(IM.playerBuildings) != null)
                {
                    agent.destination = GetClosestDropOff(IM.playerBuildings).transform.position;
                    distToTarget = Vector3.Distance(GetClosestDropOff(IM.playerBuildings).transform.position, transform.position);
                    task = TaskList.Delivering;
                }
                else
                {
                    task = TaskList.Idle;
                }
            }
            else
            {
                task = TaskList.Idle;
            }
        }

        if(heldResource >= maxHeldResource)
        {
            if (isGathering && targetNode != null)
            {
                targetNode.GetComponent<NodeManager>().gatherers--;
            }

            isGathering = false;
            obstacle.enabled = false;
            agent.enabled = true;
            if (GetClosestDropOff(IM.playerBuildings) != null)
            {
                agent.destination = GetClosestDropOff(IM.playerBuildings).transform.position;
                distToTarget = Vector3.Distance(GetClosestDropOff(IM.playerBuildings).transform.position, transform.position);
                task = TaskList.Delivering;
            }
            else
            {
                task = TaskList.Idle;
            }
        }

        if(Input.GetMouseButtonDown(1) && objInfo.isSelected)
        {
            if (!BuildingPlacement.buildingProcess)
            {
                RightClick();
            }
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

        if(Physics.Raycast(ray, out hit, 500))
        {
            if(hit.collider.tag == "Ground")
            {
                heldResource = 0;
                if(isGathering && targetNode != null)
                {
                    targetNode.GetComponent<NodeManager>().gatherers--;
                    isGathering = false;
                    isGatherer = false;
                }

                if(targetNode != null)
                {
                    targetNode = null;
                }

                if (!agent.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                AL.Move(agent, hit, true);
                task = TaskList.Moving;
                obstacle.enabled = false;
                agent.enabled = true;
            }
            else if(hit.collider.tag == "Resource")
            {
                heldResource = 0;
                if (isGathering && targetNode != null)
                {
                    targetNode.GetComponent<NodeManager>().gatherers--;
                    isGathering = false;
                    isGatherer = false;
                }

                if(!agent.enabled)
                {
                    obstacle.enabled = false;
                    agent.enabled = true;
                }
                //AL.Move(agent, hit, false);
                targetNode = hit.collider.gameObject;
                agent.destination = targetNode.transform.position;
                task = TaskList.Gathering;
                AudioManager.Instance.PlayMessageSound("WorkerGather");
            }
        }
    }

    GameObject GetClosestDropOff(List<GameObject> dropOffs)
    {
        GameObject closestDrop = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject targetDrop in dropOffs)
        {
            ObjectInfo buildingInfo = targetDrop.GetComponent<ObjectInfo>();
            if (!buildingInfo.isUnit && buildingInfo.isPlayerObject)
            {
                PlaceableBuilding placeableBuilding = targetDrop.GetComponent<PlaceableBuilding>();
                if (placeableBuilding.isDropOff && placeableBuilding.isPlaced && heldResourceType == placeableBuilding.resourceType)
                {
                    Vector3 direction = targetDrop.transform.position - position;
                    float distance = direction.sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestDrop = targetDrop;
                    }
                }
            }
        }
        return closestDrop;
    }

    public void Gather()
    {
        isGathering = true;

        if(!isGatherer)
        {
            targetNode.GetComponent<NodeManager>().gatherers++;
            isGatherer = true;
        }

        heldResourceType = targetNode.GetComponent<NodeManager>().resourceType;
        obstacle.enabled = true;
        agent.enabled = false;
    }

    IEnumerator GatherTick()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if(isGathering)
            {
                heldResource++;
            }
        }
    }

    public void PlayGatheringSound()
    {
        AudioManager.Instance.PlayRandomWorkerSound(transform.position);
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
