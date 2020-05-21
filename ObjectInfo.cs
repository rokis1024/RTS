using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectInfo : MonoBehaviour
{
    public GameObject selectionIndicator;
    public GameObject target;
    public GameObject strikePosition;
    private ResourceManager RM;
    private InputManager IM;

    public bool isSelected = false; //is selected?
    public bool isPrimary = false; //is a primary unit?
    public bool isUnit; //Is the object a unit?
    public bool isPlayerObject; //Does the player own this object?
    public bool isAlive = true; //is currently alive?

    public ObjectList objectName; //The object's name
    public string GUIname; //name displayed on game's GUI

    public float health; //The object's current health
    public float maxHealth; //The object's max health
    public float atk; //The object's physical attack
    public float def; //The object's physical defense
    public DamageType damageType; //object's damage type

    public enum Ranks { None, Recruit, Elite, Boss } //All of the available ranks

    public Ranks rank; //Creates the rank enum for this object

    public string description; //object's requirements (text)

    void Start()
    {
        RM = FindObjectOfType<ResourceManager>();
        IM = FindObjectOfType<InputManager>();
        if (isUnit)
        {
            StartCoroutine(SpecialAbilities());
        }
    }

    void Update()
    {
        selectionIndicator.SetActive(isSelected);

        if (health <= 0) //Is the object's health less than or equal to zero?
        {

            GameObject[] units = GameObject.FindGameObjectsWithTag("Selectable"); //Retrieves all selectable objects

            foreach (GameObject unit in units) //For each selectable object
            {
                ObjectInfo unitInfo = unit.GetComponent<ObjectInfo>();
                if (unitInfo.isUnit) //Is the object a unit?
                {
                    if (unitInfo.target == this.gameObject) //Is the unit targeting this object?
                    {
                        unitInfo.target = null; //Removes the unit's reference to this object as the target
                    }
                }
            }

            if (isUnit) //if object is a unit
            {
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                if (agent.isActiveAndEnabled)
                {
                    agent.isStopped = true;
                    agent.angularSpeed = 0f;
                }

                if(isPlayerObject && isAlive)
                {
                    RM.population--;
                    isAlive = false;

                    if(objectName == ObjectList.player_Worker)
                    {
                        Worker worker = GetComponent<Worker>();
                        worker.isGathering = false;
                        if(worker.isGatherer && worker.targetNode != null)
                        {
                            worker.targetNode.GetComponent<NodeManager>().gatherers--;

                            worker.targetNode = null;
                            worker.isGatherer = false;
                        }
                    }

                    IM.playerUnits.Remove(this.gameObject);
                    RM.lostUnits++;
                    AudioManager.Instance.PlayCombatSound("HumanDie", transform.position);
                }

                if(!isPlayerObject && isAlive)
                {
                    isAlive = false;
                    RM.killedEnemies++;
                    AudioManager.Instance.PlayCombatSound("HumanDie", transform.position);
                }

                GetComponent<Animator>().SetBool("isDead", true);
            }
            else //if object is a building
            {
                if(isPlayerObject && isAlive)
                {
                    RM.CountPopulationOverflow();
                    if (RM.populationOverflow <= RM.populationLimit)
                    {
                        RM.maxPopulation -= this.gameObject.GetComponent<PlaceableBuilding>().populationRate;
                    }
                    else
                    {
                        RM.populationOverflow -= this.gameObject.GetComponent<PlaceableBuilding>().populationRate;
                    }
                    isAlive = false;
                    IM.playerBuildings.Remove(this.gameObject);
                    RM.lostBuildings++;
                    AudioManager.Instance.PlayCombatSound("BuildingDestroy", transform.position);
                }

                if(!isPlayerObject && isAlive)
                {
                    isAlive = false;
                    RM.killedBuildings++;
                    AudioManager.Instance.PlayCombatSound("BuildingDestroy", transform.position);
                }
            }

            health = 0;
            Destroy(gameObject, 1.5f); //Destroy this object
        }
    }

    private IEnumerator SpecialAbilities() //unique abilities for each unit of the game, function called every sec.
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if (isAlive)
            {
                if (isPlayerObject)
                {
                    if (objectName == ObjectList.player_Recruit)
                    {
                        RecruitAbility();
                    }
                    else if (objectName == ObjectList.player_Soldier)
                    {
                        SoldierAbility();
                    }
                    else if (objectName == ObjectList.player_Macefighter)
                    {
                        MacefighterAbility();
                    }
                }
                else
                {
                    if(objectName == ObjectList.enemy_Maceman)
                    {
                        MacemanAbility();
                    }
                    else if(objectName == ObjectList.enemy_Boss)
                    {
                        BossAbility();
                    }
                }
            }
        }
    }

    private void RecruitAbility()
    {
        if(health < maxHealth / 2.5f)
        {
            if(damageType != DamageType.Pierce)
            {
                damageType = DamageType.Pierce;
            }
        }
    }

    private void SoldierAbility()
    {
        if((health + 3) < maxHealth)
        {
            health += 3;
        }
        else
        {
            health = maxHealth;
        }
    }

    private void MacefighterAbility()
    {
        if (health < maxHealth / 3f)
        {
            if (def != 100)
            {
                def = 100;
            }
        }
    }

    private void MacemanAbility()
    {
        if (health < maxHealth / 2.5f)
        {
            if (rank != Ranks.Elite)
            {
                rank = Ranks.Elite;
            }
        }
    }

    private void BossAbility()
    {
        if ((health + 4) < maxHealth)
        {
            health += 4;
        }
        else
        {
            health = maxHealth;
        }
    }

    public void PlayFootstepsSound() //play random footstep sound during walking animation
    {
        AudioManager.Instance.PlayFootstepSound(transform.position);
    }
}
