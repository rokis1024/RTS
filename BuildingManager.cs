using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] buildings;
    public GameObject[] units;

    private ResourceManager RM;
    private BuildingPlacement buildingPlacement;
    private UnitSpawning unitSpawning;
    private GUIManager GUI;


    
    private void Start()
    {
        //-----------------TWO CLASSES-----------------------------
        buildingPlacement = GetComponent<BuildingPlacement>(); //initialize building placement
        unitSpawning = GetComponent<UnitSpawning>(); //initialize units spawning
        //---------------------------------------------------------

        //-------------------------------------------------------
        RM = FindObjectOfType<ResourceManager>();
        GUI = FindObjectOfType<GUIManager>();
        //-------------------------------------------------------
    }

    //-------------------------------------------------------
    //--------------BUILD STRUCTURES METHODS-----------------
    //-------------------------------------------------------
    public void BuildCastle()
    {
        if (buildings[0].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[0]);
        }
    }
    public void BuildMine()
    {
        if (buildings[1].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[1]);
        }
    }
    public void BuildSmeltery()
    {
        if (buildings[2].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[2]);
        }
    }
    public void BuildStonemason()
    {
        if (buildings[3].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[3]);
        }
    }
    public void BuildFarm()
    {
        if (buildings[4].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[4]);
        }
    }
    public void BuildBlacksmith()
    {
        if (buildings[5].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[5]);
        }
    }
    public void BuildShootingRange()
    {
        if (buildings[6].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[6]);
        }
    }
    public void BuildBarracks()
    {
        if (buildings[7].GetComponent<PlaceableBuilding>().requiredStone <= RM.stone)
        {
            buildingPlacement.BuildBuilding(buildings[7]);
        }
    }

    public void DestroyBuilding(GameObject currentBuilding)
    {
        if (currentBuilding != null)
        {
            ObjectInfo buildingInfo = currentBuilding.GetComponent<ObjectInfo>();
            if (buildingInfo.isAlive)
            {
                buildingInfo.health = 0;
                RM.stone += currentBuilding.GetComponent<PlaceableBuilding>().destroyStone;
            }
        }
    }

    public void RepairBuilding(GameObject currentBuilding)
    {
        if(currentBuilding != null)
        {
            ObjectInfo buildingInfo = currentBuilding.GetComponent<ObjectInfo>();
            PlaceableBuilding building = currentBuilding.GetComponent<PlaceableBuilding>();
            if(buildingInfo.isAlive && buildingInfo.health < buildingInfo.maxHealth && RM.stone >= building.repairStone)
            {
                if ((buildingInfo.health + 1000) > buildingInfo.maxHealth)
                {
                    float overflow = buildingInfo.maxHealth - buildingInfo.health;
                    buildingInfo.health = buildingInfo.maxHealth;
                    RM.stone -= Mathf.Round(overflow * building.repairStone / 1000f);
                }
                else
                {
                    buildingInfo.health += 1000;
                    RM.stone -= building.repairStone;
                }
                AudioManager.Instance.PlayWorkerSound("RepairBuilding", currentBuilding.transform.position);
            }

            if (buildingInfo.objectName == ObjectList.playerBuild_Castle)
            {
                GUI.UpdateCastleHP();
            }
        }
    }

    //------------------------------------------------------SPAWN
    public void SpawnRecruit()
    {
        unitSpawning.SpawnUnit(units[1]);
    }
    public void SpawnWorker()
    {
        unitSpawning.SpawnUnit(units[0]);
    }
    public void SpawnSoldier()
    {
        unitSpawning.SpawnUnit(units[2]);
    }
    public void SpawnMacefighter()
    {
        unitSpawning.SpawnUnit(units[3]);
    }
    public void SpawnCrossbowman()
    {
        unitSpawning.SpawnUnit(units[4]);
    }
    public void SpawnPikeman()
    {
        unitSpawning.SpawnUnit(units[5]);
    }
}
