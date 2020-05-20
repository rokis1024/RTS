using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSystem : MonoBehaviour
{
    private Text ObjInfo;
    private ResourceManager RM;
    private BuildingManager BM;
    private UnitSpawning US;

    private bool workerSelected = false;
    private bool castleSelected = false;
    private GameObject selectedStructure;

    private Button btn;
    private Button btn2;
    private Button btn3;
    private Button btn4;
    private Button btn5;
    private Button btn6;
    private Button btn7;
    private Button btn8;
    private Button btn9;
    public Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        //---------------------BUTTONS---------------------------
        buttons = new Button[9];
        buttons = AddButtons();
        //-------------------------------------------------------

        //-------------------------------------------------------
        ObjInfo = GameObject.Find("ObjInfo").GetComponent<Text>();
        RM = FindObjectOfType<ResourceManager>();
        BM = FindObjectOfType<BuildingManager>();
        US = FindObjectOfType<UnitSpawning>();
        //-------------------------------------------------------
    }

    private void Update()
    {
        if (!IngameMenu.isPaused && !BuildingPlacement.buildingProcess)
        {
            if (castleSelected)
            {
                for (int i = 1; i < BM.units.Length; i++)
                {
                    Unit unit = BM.units[i].GetComponent<Unit>();
                    if (RM.iron >= unit.requiredIron && RM.gold >= unit.requiredGold && US.CheckBuilding(unit.requiredBuilding) && RM.population < RM.maxPopulation)
                    {
                        buttons[i].interactable = true;
                    }
                    else
                    {
                        buttons[i].interactable = false;
                    }
                }

                if (RM.maxPopulation <= RM.population)
                {
                    buttons[0].interactable = false;
                }
                else
                {
                    buttons[0].interactable = true;
                }

                if (selectedStructure != null)
                {
                    PlaceableBuilding building = selectedStructure.GetComponent<PlaceableBuilding>();
                    ObjectInfo buildInfo = selectedStructure.GetComponent<ObjectInfo>();
                    if (RM.stone >= building.repairStone && buildInfo.health < buildInfo.maxHealth)
                    {
                        buttons[6].interactable = true;
                    }
                    else
                    {
                        buttons[6].interactable = false;
                    }
                }
            } 
            else if (workerSelected)
            {
                for (int i = 0; i < BM.buildings.Length; i++)
                {
                    PlaceableBuilding building = BM.buildings[i].GetComponent<PlaceableBuilding>();
                    if (RM.stone >= building.requiredStone)
                    {
                        buttons[i].interactable = true;
                    }
                    else
                    {
                        buttons[i].interactable = false;
                    }
                }
            }
            else
            {
                if(selectedStructure != null)
                {
                    PlaceableBuilding building = selectedStructure.GetComponent<PlaceableBuilding>();
                    ObjectInfo buildInfo = selectedStructure.GetComponent<ObjectInfo>();
                    if(RM.stone >= building.repairStone && buildInfo.health < buildInfo.maxHealth)
                    {
                        buttons[1].interactable = true;
                    }
                    else
                    {
                        buttons[1].interactable = false;
                    }
                }
            }
        }

    }

    //-----------------------------------------------------------
    //----------------------------ACTION PANELS------------------
    //-----------------------------------------------------------
    public void CastleActionPanel(GameObject currentCastle)
    {
        selectedStructure = currentCastle;

        workerSelected = false;
        castleSelected = true;
        UnlockButtons();

        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }

        btn.onClick.AddListener(BM.SpawnWorker);
        btn2.onClick.AddListener(BM.SpawnRecruit);
        btn3.onClick.AddListener(BM.SpawnSoldier);
        btn4.onClick.AddListener(BM.SpawnMacefighter);
        btn5.onClick.AddListener(BM.SpawnCrossbowman);
        btn6.onClick.AddListener(BM.SpawnPikeman);
        btn7.onClick.AddListener(() => BM.RepairBuilding(selectedStructure));
    }

    public void WorkerActionPanel()
    {
        castleSelected = false;
        workerSelected = true;
        UnlockButtons();

        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }

        btn.onClick.AddListener(BM.BuildCastle);
        btn2.onClick.AddListener(BM.BuildMine);
        btn3.onClick.AddListener(BM.BuildSmeltery);
        btn4.onClick.AddListener(BM.BuildStonemason);
        btn5.onClick.AddListener(BM.BuildFarm);
        btn6.onClick.AddListener(BM.BuildBlacksmith);
        btn7.onClick.AddListener(BM.BuildShootingRange);
        btn8.onClick.AddListener(BM.BuildBarracks);
    }

    public void BuildingActionPanel(GameObject currentBuilding)
    {
        selectedStructure = currentBuilding;

        workerSelected = false;
        castleSelected = false;
        UnlockButtons();

        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }

        btn.onClick.AddListener(() => BM.DestroyBuilding(currentBuilding));
        btn2.onClick.AddListener(() => BM.RepairBuilding(currentBuilding));

        for (int i = 0; i < 2; i++)
        {
            buttons[i].GetComponent<UIButton>().SetButton(i + 6, false);
        }

    }

    public Button[] AddButtons()
    {
        Button[] buttons = new Button[9];
        btn = GameObject.Find("Button").GetComponent<Button>();
        btn2 = GameObject.Find("Button2").GetComponent<Button>();
        btn3 = GameObject.Find("Button3").GetComponent<Button>();
        btn4 = GameObject.Find("Button4").GetComponent<Button>();
        btn5 = GameObject.Find("Button5").GetComponent<Button>();
        btn6 = GameObject.Find("Button6").GetComponent<Button>();
        btn7 = GameObject.Find("Button7").GetComponent<Button>();
        btn8 = GameObject.Find("Button8").GetComponent<Button>();
        btn9 = GameObject.Find("Button9").GetComponent<Button>();

        buttons[0] = btn;
        buttons[1] = btn2;
        buttons[2] = btn3;
        buttons[3] = btn4;
        buttons[4] = btn5;
        buttons[5] = btn6;
        buttons[6] = btn7;
        buttons[7] = btn8;
        buttons[8] = btn9;
        return buttons;
    }

    public void LockButtons()
    {
        if (buttons.Length > 0)
        {
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
        }
    }

    public void UnlockButtons()
    {
        if (buttons.Length > 0)
        {
            foreach (Button button in buttons)
            {
                button.interactable = true;
            }
        }
    }

    //-----------------------------------------
    //-----------BUTTON LOGIC------------------
    //-----------------------------------------
    public void ShowText(int index, bool worker)
    {
        if (worker)
        {
            PlaceableBuilding structure = BM.buildings[index].GetComponent<PlaceableBuilding>();
            ObjectInfo structureInfo = BM.buildings[index].GetComponent<ObjectInfo>();
            ObjInfo.text = structureInfo.GUIname + "\nStone: " + structure.requiredStone + "\n" + structureInfo.description;
        }
        else
        {
            Unit unitInfo;
            ObjectInfo unitObject;

            if (index == 0)
            {
                ObjInfo.text = "Worker    \nGold: " + 0 + "\nIron: " + 0 + "\nBuilding: " + "Castle";
            }
            else if (index > 0 && index < 6)
            {
                unitInfo = BM.units[index].GetComponent<Unit>();
                unitObject = BM.units[index].GetComponent<ObjectInfo>();
                ObjInfo.text = unitObject.GUIname + "\nGold: " + unitInfo.requiredGold + "\nIron: " + unitInfo.requiredIron + "\nBuilding: " + unitObject.description;
            }
            else if (index == 6)
            {
                if (selectedStructure != null)
                {
                    ObjInfo.text = "Demolish selected building \nStone: +" + selectedStructure.GetComponent<PlaceableBuilding>().destroyStone;
                }
            }
            else
            {
                if (selectedStructure != null)
                {
                    ObjInfo.text = "Repair selected building\n(+1000 Health Points) \nStone: " + selectedStructure.GetComponent<PlaceableBuilding>().repairStone;
                }
            }
        }
    }

    public void ClearText()
    {
        ObjInfo.text = "";
    }

    public void WorkerEventEntries()
    {
        for (int i = 0; i < BM.buildings.Length; i++)
        {
            buttons[i].GetComponent<UIButton>().SetButton(i, true);
        }
    }

    public void CastleEventEntries()
    {
        for (int i = 0; i < BM.units.Length + 1; i++)
        {
            if (i != 6)
            {
                buttons[i].GetComponent<UIButton>().SetButton(i, false);
            }
            else
            {
                buttons[i].GetComponent<UIButton>().SetButton(i+1, false);
            }
        }
    }

}
