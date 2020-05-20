using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitSpawning : MonoBehaviour
{
    private Unit unit;
    private GameObject currentUnit;
    private InputManager IM;

    private ResourceManager RM;

    // Start is called before the first frame update
    void Start()
    {
        RM = FindObjectOfType<ResourceManager>();
        IM = FindObjectOfType<InputManager>();
    }

    public void SpawnUnit(GameObject u)
    {
        if (IM.primaryObject != null)
        {
            ObjectInfo castleInfo = IM.primaryObject.GetComponent<ObjectInfo>();
            if (!castleInfo.isUnit)
            {
                PlaceableBuilding castle = castleInfo.gameObject.GetComponent<PlaceableBuilding>();
                if (u.GetComponent<ObjectInfo>().objectName != ObjectList.player_Worker)
                {
                    unit = u.GetComponent<Unit>();
                    if (unit.requiredGold <= RM.gold && unit.requiredIron <= RM.iron
                        && RM.population < RM.maxPopulation)
                    {
                        if (CheckBuilding(unit.requiredBuilding))
                        {
                            Vector3 newPos = IM.primaryObject.transform.position - new Vector3(16, 0, 16);
                            Vector3 newPos2 = castle.flagPoint.transform.position;
                            currentUnit = Instantiate(u, newPos, IM.primaryObject.transform.rotation);
                            //----------------------------------------
                            RM.population++;
                            RM.iron -= unit.requiredIron;
                            RM.gold -= unit.requiredGold;
                            //----------------------------------------
                            currentUnit.GetComponent<NavMeshAgent>().SetDestination(newPos2);
                            unit.task = TaskList.Moving;
                            IM.playerUnits.Add(currentUnit);
                            RM.producedUnits++;
                            AudioManager.Instance.PlayWorkerSound("SummonTroops", castle.transform.position);
                        }
                    }
                }
                else
                {
                    if (RM.population < RM.maxPopulation)
                    {
                        Vector3 newPos = IM.primaryObject.transform.position - new Vector3(16, 0, 16);
                        Vector3 newPos2 = castle.flagPoint.transform.position;
                        currentUnit = Instantiate(u, newPos, IM.primaryObject.transform.rotation);
                        //--------------------------------------------
                        RM.population++;
                        //--------------------------------------------
                        currentUnit.GetComponent<NavMeshAgent>().SetDestination(newPos2);
                        currentUnit.GetComponent<Worker>().task = TaskList.Moving;
                        IM.playerUnits.Add(currentUnit);
                        RM.producedUnits++;
                        AudioManager.Instance.PlayWorkerSound("SummonTroops", castle.transform.position);
                    }
                }
            }
        }
    }

    public bool CheckBuilding(ObjectList buildingName)
    {
        List<GameObject> buildings = IM.playerBuildings;
        foreach (GameObject building in buildings)
        {
            ObjectInfo buildInfo = building.GetComponent<ObjectInfo>();
            if (buildInfo.isPlayerObject && !buildInfo.isUnit)
            {
                if (buildInfo.objectName == buildingName)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
