using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ResourceManager : MonoBehaviour
{
    public float stone; 
    public float maxStone; 
    public float collectedStone;
    public float iron;
    public float maxIron; 
    public float collectedIron;
    public float gold; 
    public float maxGold; 
    public float collectedGold;
    public float population; 
    public float maxPopulation; 
    public float populationLimit;
    public float populationOverflow;

    InputManager IM;
    //----------------------------------
    [Header("Score Panel")]
    public int producedUnits;
    public int lostUnits;
    public int producedBuildings;
    public int lostBuildings;
    //--
    public int producedEnemies;
    public int killedEnemies;
    public int killedBuildings;
    //----------------------------------
    [Header("Display")]
    public Text stoneDisp; //The Text object that displays the ice values
    public Text ironDisp; //The Text object that displays the iron values
    public Text goldDisp; //The Text object that displays the gold values
    public Text populationDisp; //The Text object that displays the population values

    [Header("ScoreDisplay")]
    public Text[] collected;
    public Text[] playerProduction;
    public Text[] enemyProduction;
    public Text titleDisplay;

    void Start()
    {
        producedUnits = producedBuildings = producedEnemies = 0;
        lostUnits = lostBuildings = killedEnemies = killedBuildings = 0;

        IM = FindObjectOfType<InputManager>();
        if (GameManager.Instance.GetDifficulty() == 0)
        {
            stone = iron = gold = 240;
            collectedStone = collectedIron = collectedGold = 240;
        }
        else if (GameManager.Instance.GetDifficulty() == 1)
        {
            stone = iron = gold = 160;
            collectedStone = collectedIron = collectedGold = 160;
        }
        else
        {
            stone = iron = gold = 80;
            collectedStone = collectedIron = collectedGold = 80;
        }
    }

    // Update is called once per frame
    void Update()
    {
        stoneDisp.text = "" + stone; //Displays the current stone out of the max stone
        ironDisp.text = "" + iron; //Displays the current iron out of the max iron
        goldDisp.text = "" + gold; //Displays the current gold out of the max gold
        populationDisp.text = "" + population + "/" + maxPopulation; //Displays the current population out of the max population

        PopulationCap();

        if (Input.GetKey(KeyCode.F1) && Input.GetKey(KeyCode.C))
        {
            if ((stone + 15) < maxStone && (iron + 15) < maxIron && (gold + 15) < maxGold)
            {
                stone += 150;
                iron += 150;
                gold += 150;
            }
        }
    }

    public void CountPopulationOverflow()
    {
        populationOverflow = 0;
        foreach(GameObject building in IM.playerBuildings)
        {
            PlaceableBuilding buildingInfo = building.GetComponent<PlaceableBuilding>();
            if(buildingInfo.populationRate > 0)
            {
                populationOverflow += buildingInfo.populationRate;
            }
        }
    }

    private void PopulationCap() //should have used Mathf.Clamp ;(
    {
        if(maxPopulation > populationLimit)
        {
            maxPopulation = populationLimit;
        }
        if(maxPopulation < 0)
        {
            maxPopulation = 0;
        }
        if(population < 0)
        {
            population = 0;
        }
    }

    public void ScoresOverview()
    {
        if(collected.Length >= 3)
        {
            collected[0].text = "Stone: " + collectedStone;
            collected[1].text = "Iron: " + collectedIron;
            collected[2].text = "Gold: " + collectedGold;
        }

        if(playerProduction.Length >= 4)
        {
            playerProduction[0].text = "Produced: " + producedUnits;
            playerProduction[1].text = "Lost: " + lostUnits;
            playerProduction[2].text = "Produced: " + producedBuildings;
            playerProduction[3].text = "Lost: " + lostBuildings;
        }

        if(enemyProduction.Length >= 3)
        {
            enemyProduction[0].text = "Produced: " + producedEnemies;
            enemyProduction[1].text = "Lost: " + killedEnemies;
            enemyProduction[2].text = "Lost: " + killedBuildings;
        }
    }
}
