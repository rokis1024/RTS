using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BuildingPlacement : MonoBehaviour
{
    private PlaceableBuilding placeableBuilding;
    private GameObject currentBuilding;
    private ButtonSystem BM;

    private bool hasPlaced;
    private bool triggerTimer = false;
    private float timer;
    private ResourceManager RM;

    public static bool buildingProcess = false;

    // Start is called before the first frame update
    void Start()
    {
        RM = FindObjectOfType<ResourceManager>();
        BM = FindObjectOfType<ButtonSystem>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuilding != null && !hasPlaced)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                currentBuilding.transform.position = hitInfo.point;
            }

            if (!triggerTimer)
            {
                timer += Time.deltaTime;
                if (Input.GetMouseButtonDown(0) && !IngameMenu.isPaused && timer >= 0.3f)
                {
                    if (!EventSystem.current.IsPointerOverGameObject() &&IsLegalPosition() && RM.stone >= placeableBuilding.requiredStone && currentBuilding.transform.position.y < 2.5f)
                    {
                        hasPlaced = true;
                        RM.stone -= placeableBuilding.requiredStone;
                        if (RM.maxPopulation <= RM.populationLimit)
                        {
                            RM.maxPopulation += placeableBuilding.populationRate;
                        }
                        currentBuilding.GetComponent<NavMeshObstacle>().enabled = true;
                        currentBuilding.layer = 8;
                        placeableBuilding.PlaceBuilding();
                        BM.UnlockButtons();
                        timer = 0;
                        triggerTimer = true;
                        RM.producedBuildings++;
                        //buildingProcess = false;
                    }
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                Destroy(currentBuilding);
                hasPlaced = false;
                BM.UnlockButtons();
                timer = 0;
                triggerTimer = true;
            }
        }

        if (triggerTimer)  //avoid a bug when LeftClick() method from InputManager.cs is called as soon as the building is placed on a map
        {
            timer += Time.deltaTime;
            if (timer >= 0.15f)
            {
                buildingProcess = false;
                timer = 0;
                triggerTimer = false;
            }
        }
    }

    bool IsLegalPosition()
    {
        if(placeableBuilding.colliders.Count > 0)
        {
            return false;
        }
        return true;
    }

    public void BuildBuilding(GameObject b)
    {
        buildingProcess = true;
        hasPlaced = false;

        triggerTimer = false;
        timer = 0;

        currentBuilding = Instantiate(b);
        placeableBuilding = currentBuilding.GetComponent<PlaceableBuilding>();
        BM.LockButtons();
    }
}
